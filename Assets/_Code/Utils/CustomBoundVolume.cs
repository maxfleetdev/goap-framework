using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Creates a custom Bound using a minimum and maximum positon for sizing, along with a parent Transform for Rotation and Scaling
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class CustomBoundVolume : MonoBehaviour
{
    [SerializeField] private Transform minBound;
    [SerializeField] private Transform maxBound;
    [SerializeField] private bool useParent = true;

    // Only used if useParent is true
    [HideIf("useParent")]
    [SerializeField] private Transform rootTransform;

    private BoxCollider boxCollider;
    private Vector3 localMin;
    private Vector3 localMax;
    private Transform root;
    
    /// <summary>
    /// The root Transform of this bound. Used for calculating conversions and rotations
    /// </summary>
    public Transform Root
    {
        get => root;
        private set => root = value;
    }

    #region Cache Bounds

    private void Awake()
    {
        CacheBound();
    }

    /// <summary>
    /// Stores variables for use in BuildingManager and overlap checks
    /// </summary>
    private void CacheBound()
    {
        root = (useParent || rootTransform == null) ? transform.parent : rootTransform;
        boxCollider = GetComponent<BoxCollider>();
    }

    #endregion

    #region Local Space

    // Local Space
    public Vector3 LocalCenter
    {
        get
        {
            Vector3 local_min = Root.InverseTransformPoint(minBound.position);
            Vector3 local_max = Root.InverseTransformPoint(maxBound.position);
            return (local_min + local_max) * 0.5f;
        }
    }
    public Vector3 LocalSize
    {
        get
        {
            Vector3 localMin = Root.InverseTransformPoint(minBound.position);
            Vector3 localMax = Root.InverseTransformPoint(maxBound.position);
            
            return new Vector3(
                Mathf.Abs(localMax.x - localMin.x),
                Mathf.Abs(localMax.y - localMin.y),
                Mathf.Abs(localMax.z - localMin.z)
            );
        }
    }

    #endregion

    #region World Space

    /// <summary>
    /// Center of this bound Transformed from local space to world space using TransformPoint
    /// </summary>
    public Vector3 WorldCenter => Root.TransformPoint(LocalCenter);
    /// <summary>
    /// Extents of this bound by scaling the LocalSize with the Reference's LossyScale
    /// </summary>
    public Vector3 WorldExtents => Vector3.Scale(LocalSize * 0.5f, Root.lossyScale);
    /// <summary>
    /// The Rotation of the Reference
    /// </summary>
    public Quaternion Rotation => Root.rotation;

    #endregion

    #region Apply Bounds

    /// <summary>
    /// Applies CustomBound sizing to attached BoxCollider - used to block future Placeable Prefabs
    /// </summary>
    public void ApplyBounds()
    {
        if (boxCollider == null)
        {
            Debug.LogWarning("CustomBoundVolume: No BoxCollider attached!");
            return;
        }
        
        // Calculate Bound sizing
        Vector3 localMin = Root.InverseTransformPoint(minBound.position);
        Vector3 localMax = Root.InverseTransformPoint(maxBound.position);
        Vector3 localCenter = (localMin + localMax) * 0.5f;
        Vector3 localSize = new Vector3(
            Mathf.Abs(localMax.x - localMin.x),
            Mathf.Abs(localMax.y - localMin.y),
            Mathf.Abs(localMax.z - localMin.z)
        );
        
        // Assign Bound to BoxCollider
        boxCollider.center = localCenter;
        boxCollider.size = localSize;
        
        // Assign BoxCollider variables
        boxCollider.isTrigger = true;
    }

    #endregion

    #region Gizmo Drawning
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (minBound == null || maxBound == null || Root == null)
        {
            return;
        }

        BoxOverlapCheck.DrawOverapCustomBounds(this, Color.green);
    }

#endif
    #endregion
}