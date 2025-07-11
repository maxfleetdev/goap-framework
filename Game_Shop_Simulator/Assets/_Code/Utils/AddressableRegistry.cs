using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class AddressableRegistry<TAsset, TKey> where TAsset : UnityEngine.Object
{
    private readonly Dictionary<TKey, AssetReference> referenceMap = new();
    private readonly Dictionary<TKey, TAsset> loadedAssets = new();
    private bool isInitialised = false;

    public bool IsInitialised => isInitialised;
    public event Action OnLoaded;

    public void Reset()
    {
        referenceMap.Clear();
        loadedAssets.Clear();
        isInitialised = false;
    }

    public void LoadAll(string label, Func<TAsset, TKey> keySelector, Action onComplete = null)
    {
        if (isInitialised)
        {
            onComplete?.Invoke();
            return;
        }

        Addressables.LoadResourceLocationsAsync(label).Completed += locationHandle =>
        {
            if (locationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Logger.Warn(this.GetType().Name, $"Failed to get locations for label '{label}'");
                onComplete?.Invoke();
                return;
            }

            var locations = locationHandle.Result;
            int loadedCount = 0;

            foreach (var location in locations)
            {
                Addressables.LoadAssetAsync<TAsset>(location).Completed += assetHandle =>
                {
                    if (assetHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        var asset = assetHandle.Result;
                        var key = keySelector(asset);

                        if (!Equals(key, default(TKey)) && !referenceMap.ContainsKey(key))
                        {
                            referenceMap[key] = new AssetReference(location.PrimaryKey);
                        }
                    }

                    loadedCount++;
                    if (loadedCount >= locations.Count)
                    {
                        isInitialised = true;
                        onComplete?.Invoke();
                        OnLoaded?.Invoke();

                        Logger.Loaded(this.GetType().Name, referenceMap.Count, label);
                    }
                };
            }
        };
    }

    public async Task<TAsset> GetAsync(TKey key)
    {
        if (loadedAssets.TryGetValue(key, out var cached))
            return cached;

        if (!referenceMap.TryGetValue(key, out var reference))
        {
            Logger.Warn(this.GetType().Name, $"Key {key} not found in registry.");
            return null;
        }

        var handle = reference.LoadAssetAsync<TAsset>();
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            loadedAssets[key] = handle.Result;
            return handle.Result;
        }

        Logger.Warn(this.GetType().Name, $"Failed to load asset for key {key}");
        return null;
    }

    public int GetCount()
    {
        return referenceMap.Count;
    }

    public bool GetIDExists(TKey check)
    {
        return referenceMap.ContainsKey(check);
    }

    public List<TKey> GetAllIDs()
    {
        List<TKey> ids = new();
        foreach (var id in referenceMap.Keys)
        {
            ids.Add(id);
        }
        return ids;
    }

    public async Task<List<TAsset>> GetAllLoadedDataAsync()
    {
        var tasks = new List<Task<TAsset>>();
        var keys = GetAllIDs();

        foreach (var key in keys)
        {
            tasks.Add(GetAsync(key));
        }

        var results = await Task.WhenAll(tasks);
        return new List<TAsset>(results.Where(asset => asset != null));
    }
}