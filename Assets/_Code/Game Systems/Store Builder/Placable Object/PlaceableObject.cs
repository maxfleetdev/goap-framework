using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// ScriptableObject representing data of a Placeable Prefab. Contains metadata and checks for Orientation, NavMesh and Overlapping
/// </summary>
[CreateAssetMenu(menuName = "GSS/Placeable Object")]
public class PlaceableObject : ScriptableObject
{
    [Header("Object Data")]
    [Tooltip("Name of object for GUI and logging")]
    [SerializeField] private string objectName;
    
    [Tooltip("Used with addressables. Must be unique")]
    [SerializeField] private int objectID = 0;
    

    [Header("Meta Data")]
    [Tooltip("Price of this object to place")]
    [SerializeField] private float objectPrice = 0f;

    [Tooltip("Determines if object updates navmesh on placement")]
    [SerializeField] private bool isTraversable = false;


    [Header("Placement Rules")]
    [EnumFlags] [SerializeField] private BuildOrientation orientation;
    [SerializeField] private bool isStackable = false;
    
    [Tooltip("Placement angle tolerance (degrees)")]
    [Range(1f, 45f)] [SerializeField] private float toleranceAngle = 25f;

    [Tooltip("Allows prefab to overlap with any other object")]
    [SerializeField] private bool isOverlappable = false;

    [SerializeField] private bool hasExtendedBounds = false;
    
    private PlacementRule buildRule;

    #region Properties

    /// <summary>
    /// This objects name (used for GUI/debug)
    /// </summary>
    public string ObjectName
    {
        get => objectName;
        private set => objectName = value;
    }
    /// <summary>
    /// Unique ID for this object. Used for getting addressable prefab with this ID attached
    /// </summary>
    public int ObjectID
    {
        get => objectID;
        private set => objectID = value;
    }
    /// <summary>
    /// Price of this object when placing
    /// </summary>
    public float ObjectPrice
    {
        get => objectPrice;
        private set => objectPrice = value;
    }
    /// <summary>
    /// Whether this object can effect NavMesh Graph
    /// </summary>
    public bool IsTraversable
    {
        get => isTraversable;
        private set => isTraversable = value;
    }
    /// <summary>
    /// The orientation this object can be placed (Floor, Ceiling etc)
    /// </summary>
    public BuildOrientation Orientation
    {
        get => orientation;
        private set => orientation = value;
    }
    /// <summary>
    /// Whether this object can be placed ontop of another GameObject
    /// </summary>
    public bool IsStackable
    {
        get => isStackable;
        private set => isStackable = value;
    }
    /// <summary>
    /// Class used for checking if a given raycast is a valid orientation for this object
    /// </summary>
    public PlacementRule BuildRule
    {
        get
        {
            if (buildRule == null)
            {
                Initialise();
            }
            return buildRule;
        }
        private set => buildRule = value;
    }
    /// <summary>
    /// Whether this object can overlap with any object
    /// </summary>
    public bool IsOverlappable
    {
        get => isOverlappable;
        private set => isOverlappable = value;
    }
    /// <summary>
    /// Whether this object has CustomBoundVolume attached to further block placement near this object
    /// </summary>
    public bool HasExtendedBounds
    {
        get => hasExtendedBounds;
        private set => hasExtendedBounds = value;
    }

    #endregion

    /// <summary>
    /// Called on addressable load to fill variables before runtime
    /// </summary>
    public void Initialise()
    {
        buildRule = new PlacementRule(orientation, toleranceAngle);
    }

    /// <summary>
    /// Called when this object is physically placed
    /// </summary>
    public bool IsPlaceable()
    {
        // Try Purchase Object
        if (!StoreFinanceEvents.TryPurchase(objectPrice))
        {
            // Not enough funds
            return false;
        }
        return true;
    }
}