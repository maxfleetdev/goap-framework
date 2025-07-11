using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class BuildableAssetRegistry
{
    private static AddressableRegistry<GameObject, int> internalRegistry = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void ResetDatabase()
    {
        internalRegistry.Reset();
    }

    /// <summary>
    /// Loads all PlaceableObject prefabs from the Addressable group with the "placeable" label.
    /// </summary>
    public static void LoadAll(Action onComplete = null)
    {
        internalRegistry.LoadAll("placeable", prefab =>
        {
            var data = prefab.GetComponent<IProvidable<PlaceableObject>>().Get();
            if (data == null)
            {
                Debug.LogWarning($"[PlacementObjectRegistry] Missing BuildObjectData on prefab: {prefab.name}");
                return default;
            }

            data.Initialise();
            return data.ObjectID;
        }, onComplete);
    }

    /// <summary>
    /// Gets the prefab associated with a given BuildableObject ID.
    /// </summary>
    public static Task<GameObject> GetPrefabAsync(int buildableID)
    {
        return internalRegistry.GetAsync(buildableID);
    }

    public static int GetCount() => internalRegistry.GetCount();

    public static bool IDExists(int id) => internalRegistry.GetIDExists(id);

    /// <summary>
    /// Returns a list of ObjectID's from the Registry. Optional sorting parameter
    /// </summary>
    /// <param name="sorted"></param>
    /// <returns></returns>
    public static List<int> GetAllIDs(bool sorted = true)
    {
        List<int> ids = internalRegistry.GetAllIDs();

        if (sorted)
        {
            ids.Sort();
        }

        return ids;
    }
}