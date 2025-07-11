using UnityEngine;

public static class BoxOverlapCheck
{
    private static readonly float EXTENT_MULTIPLIER = 0.97f;        // Used for adding leeway for intersecting objects

    private static readonly int MAX_CHECKS = 3;         // Maximum objects stored when using overlap

    private static Collider[] allHits = new Collider[MAX_CHECKS];           // Stored overlaps with limit of MAX_CHECKS 

    #region MeshRenderer OverlapBox

    /// <summary>
    /// Returns true if a current object is intersecting with a collider. Uses meshrenderer for calculating bounds
    /// </summary>
    /// <param name="mesh_renderer"></param>
    /// <returns>True if overlapping</returns>
    public static bool IsObjectOverlapping(MeshRenderer mesh_renderer, string[] ignore_tags = null)
    {
        return OverlapCheck(mesh_renderer, ignore_tags);
    }

    private static bool OverlapCheck(MeshRenderer mesh_renderer, string[] ignore_tags = null)
    {
        Bounds mesh_bounds = mesh_renderer.localBounds;
        Transform mesh_transform = mesh_renderer.transform;

        Vector3 center = mesh_transform.TransformPoint(mesh_bounds.center);
        Vector3 half_extents = Vector3.Scale(mesh_bounds.size / 2, mesh_transform.lossyScale) * EXTENT_MULTIPLIER;
        Quaternion rotation = mesh_transform.rotation;

        int count = Physics.OverlapBoxNonAlloc(center, half_extents, allHits, rotation);

        // Loop through each object
        for (int i = 0; i < count; i++)
        {
            // Store current overlapping object
            var hit = allHits[i];

            // Hit is nothing
            if (hit == null)
            {
                continue;
            }

            // Overlapping object is within ignore tags
            if (ignore_tags != null && System.Array.Exists(ignore_tags, tag => hit.CompareTag(tag)))
            {
                continue;
            }

            // Overlap was found
            return true;
        }

        // No overlap was found
        return false;
    }

    #endregion

    #region Bound OverlapBox

    /// <summary>
    /// Returns true if a current object is overlapping with another collider. 
    /// Uses an objects bound for calculation and OverlapBox size
    /// </summary>
    /// <param name="mesh_renderer"></param>
    /// <returns>True if overlapping</returns>
    public static bool IsObjectOverlapping(Bounds bounds, Transform transform, string[] ignore_tags = null)
    {
        return OverlapCheck(bounds, transform, ignore_tags);
    }

    private static bool OverlapCheck(Bounds bounds, Transform transform, string[] ignore_tags = null)
    {
        Vector3 center = transform.TransformPoint(bounds.center);
        Vector3 extents = Vector3.Scale(bounds.size * 0.5f, transform.lossyScale) * EXTENT_MULTIPLIER;
        Quaternion rotation = transform.rotation;

        int count = Physics.OverlapBoxNonAlloc(center, extents, allHits, rotation);

        for (int i = 0; i < count; i++)
        {
            var hit = allHits[i];

            if (hit == null)
            {
                continue;

            }

            if (ignore_tags != null && IsIgnored(hit, ignore_tags))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private static bool IsIgnored(Collider hit, string[] ignore_tags)
    {
        if (ignore_tags == null)
            return false;

        for (int i = 0; i < ignore_tags.Length; i++)
        {
            if (hit.CompareTag(ignore_tags[i]))
                return true;
        }

        return false;
    }

    #endregion

    #region Custom Bounds

    /// <summary>
    /// Returns true if a CustomBoundVolume is overlapping with another object.
    /// </summary>
    /// <param name="volume"></param>
    /// <param name="ignored_tags"></param>
    /// <returns></returns>
    public static bool IsCustomBoundOverlapping(CustomBoundVolume volume, string[] ignored_tags = null)
    {
        if (volume == null)
        {
            return false;
        }
        return CustomBoundOverlapCheck(volume, ignored_tags);
    }

    public static bool CustomBoundOverlapCheck(CustomBoundVolume volume, string[] ignored_tags = null)
    {
        Vector3 center = volume.WorldCenter;
        Vector3 extents = volume.WorldExtents * 0.97f;
        Quaternion rotation = volume.Rotation;

        int count = Physics.OverlapBoxNonAlloc(center, extents, allHits, rotation);
        for (int i = 0; i < count; i++)
        {
            var hit = allHits[i];
            if (hit == null) continue;
            if (ignored_tags != null && System.Array.Exists(ignored_tags, t => hit.CompareTag(t))) continue;
            return true;
        }

        return false;
    }


    #endregion

    #region Gizmo Tools
#if UNITY_EDITOR
    /// <summary>
    /// Creates a wirebox gizmo for displaying a given MeshRenderer and it's OverlapBox bounds
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="color"></param>
    public static void DrawOverlapMesh(MeshRenderer renderer, Color color)
    {
        if (renderer == null)
        {
            return;
        }

        Bounds bounds = renderer.localBounds;
        Transform mesh_transform = renderer.transform;

        Vector3 center = mesh_transform.TransformPoint(bounds.center);
        Vector3 half_extents = Vector3.Scale(bounds.size * 0.5f, mesh_transform.lossyScale) * EXTENT_MULTIPLIER;
        Quaternion rotation = mesh_transform.rotation;

        Gizmos.color = color;
        Matrix4x4 old_matrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, half_extents * 2);
        Gizmos.matrix = old_matrix;
    }

    /// <summary>
    /// Creates a wirebox gizmo for displaying a given Bound and Transform
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="color"></param>
    public static void DrawOverlapBounds(Bounds bounds, Transform transform, Color color)
    {
        if (bounds == null)
        {
            return;
        }

        Vector3 center = transform.TransformPoint(bounds.center);
        Vector3 half_extents = Vector3.Scale(bounds.size * 0.5f, transform.lossyScale) * EXTENT_MULTIPLIER;
        Quaternion rotation = transform.rotation;

        Gizmos.color = color;
        Matrix4x4 old_matrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, half_extents * 2);
        Gizmos.matrix = old_matrix;
    }

    public static void DrawOverapCustomBounds(CustomBoundVolume volume, Color color)
    {
        if (volume == null)
        {
            return;
        }

        Vector3 center = volume.WorldCenter;
        Vector3 size = volume.WorldExtents * 2f;
        Quaternion rotation = volume.Rotation;

        Gizmos.color = color;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = oldMatrix;
    }
#endif
    #endregion
}