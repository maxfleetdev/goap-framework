using UnityEngine;

/// <summary>
/// Static ScriptableObject which contains information for creating StockPrefabs to conform to specific a ShelfSize and it's Collider Bound Size
/// </summary>
[CreateAssetMenu(menuName = "GSS/Shelf Size")]
public class ShelfSizeProfile : ScriptableObject
{
    [SerializeField] private ShelfSize size;            // Enum for determining Shelf Size (Small, Medium, Large etc)
    [SerializeField] private Vector3 boundSize;         // Size of collider bounds (BoxCollider)
    [SerializeField] private Vector3 origin;

    public ShelfSize Size
    {
        get => size;
    }
    public Vector3 BoundSize
    {
        get => boundSize;
    }
    public Vector3 Origin
    {
        get => origin;
    }
}