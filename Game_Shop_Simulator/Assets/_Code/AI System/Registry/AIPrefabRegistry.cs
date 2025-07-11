using System;
using System.Threading.Tasks;
using UnityEngine;

public static class AIPrefabRegistry
{
    private static AddressableRegistry<GameObject, int> internalRegistry = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void ResetDatabase()
    {
        internalRegistry.Reset();
    }

    /// <summary>
    /// Loads all AI prefabs from the Addressable group with the "ai" label.
    /// </summary>
    public static void LoadAll(Action onComplete = null)
    {
        internalRegistry.LoadAll("ai", prefab =>
        {
            var data = prefab.GetComponent<AIProfileProvider>().ProfileData;
            if (data == null)
            {
                Debug.LogWarning($"[AIPrefabRegistry] Missing ObjectID on prefab: {prefab.name}");
                return default;
            }

            return data.ObjectID;
        }, onComplete);
    }

    /// <summary>
    /// Gets the prefab associated with a given AI ID. (change to better name/method)
    /// </summary>
    public static Task<GameObject> GetPrefabAsync(int ai_id)
    {
        return internalRegistry.GetAsync(ai_id);
    }
}
