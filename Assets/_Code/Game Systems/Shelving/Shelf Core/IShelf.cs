using UnityEngine;
using GSS.AI;

/// <summary>
/// Generic Interface for all shelving objects
/// </summary>
public interface IShelf
{
    // Called on enable/disable (maybe change to private)
    public void ShelfCreation();
    public void ShelfRemoval();

    // Public Events
    public void RowItemChanged(ShelfRow row, int previousID);
    public bool TryTakeItem(int stockID);

    // Public Queries
    public bool ShelfHasItem(int stockID);
    public Vector3 GetShelfPosition();

    #region Browse Point Queries

    /// <summary>
    /// Returns true if any browsing points are free
    /// </summary>
    /// <returns>Boolean of availability</returns>
    public bool BrowsePointAvailable();
    
    /// <summary>
    /// Returns true if a point is available and the assigned point position
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="position"></param>
    /// <returns>Boolean if available, position of assigned spot</returns>
    public bool GetBrowsePoint(AIAgent agent, out Vector3 position);

    /// <summary>
    /// Releases the agents browsing spot
    /// </summary>
    /// <param name="agent"></param>
    public void ReleaseBrowsePoint(AIAgent agent);

    #endregion
}