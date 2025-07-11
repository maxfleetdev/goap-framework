using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Registry for loading shelving stock prefabs
/// </summary>
public static class StockPrefabRegistry
{
    private static AddressableRegistry<GameObject, int> internalRegistry = new();
    private static readonly string ADDRESSABLE_LABEL = "stockprefab";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void ResetDatabase()
    {
        internalRegistry.Reset();
    }

    /// <summary>
    /// Loads all StockPrefabs from the Addressable group with the "stock" label.
    /// </summary>
    public static void LoadAll(Action onComplete = null)
    {
        internalRegistry.LoadAll(ADDRESSABLE_LABEL, prefab =>
        {
            var data = prefab.GetComponent<IProvidable<int>>();
            if (data == null)
            {
                Debug.LogWarning($"[StockAssetRegistry] Missing StockItemData on prefab: {prefab.name}");
                return default;
            }

            return data.Get();
        }, onComplete);
    }

    /// <summary>
    /// Gets the prefab associated with a given StockItem ID.
    /// </summary>
    public static Task<GameObject> GetPrefabAsync(int stockID)
    {
        return internalRegistry.GetAsync(stockID);
    }
}