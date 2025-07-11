using UnityEngine;

/// <summary>
/// Monobehaviour for linking a ScriptableObject PlaceableObject within an Addressable Prefab
/// </summary>
public class PlaceableObjectProvider : MonoBehaviour, IProvidable<PlaceableObject>
{
    [SerializeField] private PlaceableObject objectData;
    
    public PlaceableObject Get() => objectData;

    /// <summary>
    /// Calls IPlaceable interface when this Prefab has been placed
    /// </summary>
    public void PrefabPlaced()
    {
        foreach (IPlaceable placeable in GetComponentsInChildren<IPlaceable>())
        {
            placeable.OnPlaced();
        }
    }

    /// <summary>
    /// Calls IPlaceable interface when this Prefab has been removed
    /// </summary>
    public void PrefabRemoved()
    {
        foreach (IPlaceable placeable in GetComponentsInChildren<IPlaceable>())
        {
            placeable.OnRemoved();
        }
    }
}