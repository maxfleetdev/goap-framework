using UnityEngine;

/// <summary>
/// Creates a physical GameObject from a given placable object prefab. Uses pooling for creating objects. 
/// Makes a barebone GameObject with a pivot and child is the prefab.
/// </summary>
public static class GhostObjectBuilder
{
    private static readonly string GHOST_TAG = "Ghost";
    private static readonly string CUSTOM_BOUND_TAG = "ExtendedOverlap";
    private static readonly string GHOST_LAYER = "Ignore Raycast";

    /// <summary>
    /// Returns a complete Ghost object used for displaying a prefabs future position
    /// </summary>
    /// <param name="originalPrefab"></param>
    /// <param name="ghostMaterial"></param>
    /// <returns>Ghost GameObject</returns>
    public static GameObject BuildGhost(GameObject originalPrefab, Material ghostMaterial)
    {
        GameObject pivot = new GameObject(originalPrefab.name + "_GhostPivot");
        GameObject ghost = GameObject.Instantiate(originalPrefab, pivot.transform);

        RemoveColliders(ghost);
        RemoveRigidbodies(ghost);
        RemoveScripts(ghost);

        ApplyGhostMaterial(ghost, ghostMaterial);
        ApplyPivotPoint(ghost, pivot, originalPrefab);

        return pivot;
    }

    private static void RemoveColliders(GameObject obj)
    {
        foreach (var col in obj.GetComponentsInChildren<Collider>())
        {
            // Skip CustomBoundVolume Collider (used later using ApplyBound)
            if (col.tag == CUSTOM_BOUND_TAG)
            {
                continue;
            }
            GameObject.Destroy(col);
        }
    }

    private static void RemoveRigidbodies(GameObject obj)
    {
        foreach (var rb in obj.GetComponentsInChildren<Rigidbody>())
        {
            GameObject.Destroy(rb);
        }
    }

    private static void RemoveScripts(GameObject obj)
    {
        var components = obj.GetComponentsInChildren<MonoBehaviour>();

        foreach (var comp in components)
        {
            if (comp is PlaceableObjectProvider || comp is CustomBoundVolume)
            {
                continue;
            }
            GameObject.Destroy(comp);
        }
    }

    private static void ApplyGhostMaterial(GameObject obj, Material ghostMaterial)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.material = ghostMaterial;
        }
    }

    private static void ApplyPivotPoint(GameObject ghost, GameObject pivot, GameObject original)
    {
        MeshRenderer renderer = ghost.GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.localBounds;
            Vector3 offset = bounds.center - new Vector3(0, bounds.extents.y, 0);
            Vector3 scale = original.transform.localScale;
            offset = Vector3.Scale(offset, scale);
            ghost.transform.localPosition = -offset;
        }

        pivot.tag = GHOST_TAG;
        pivot.layer = LayerMask.NameToLayer(GHOST_LAYER);
    }
}