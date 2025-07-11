using UnityEngine;

/// <summary>
/// Data structure for a shelf which saves the Shelf's Category, Size Profile and Root ShelfObject
/// </summary>
public readonly struct Shelf
{
    /// <summary>
    /// The category of Stock this Shelf can contain
    /// </summary>
    public StockCategory ShelfCategory { get; }
    /// <summary>
    /// ScriptableObject which contains data regarding Shelf Size and collider bounds
    /// </summary>
    public ShelfSizeProfile ShelfProfile { get; }
    /// <summary>
    /// The main ShelfObject which is rooted to this ShelfRow
    /// </summary>
    public ShelfObject RootShelfObject { get; }

    public Shelf(StockCategory category, ShelfSizeProfile shelf_profile, ShelfObject root)
    {
        ShelfCategory = category;
        ShelfProfile = shelf_profile;
        RootShelfObject = root;
    }

    /// <summary>
    /// Returns the Bound Size contained within current ShelfProfile
    /// </summary>
    /// <returns></returns>
    public Vector3 GetBoundSize() => ShelfProfile != null ? ShelfProfile.BoundSize : Vector3.zero;

    /// <summary>
    /// Returns the enum ShelfSize contained with current ShelfProfile
    /// </summary>
    /// <returns></returns>
    public ShelfSize GetShelfSize() => ShelfProfile != null ? ShelfProfile.Size : ShelfSize.Medium;
}