using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class which stores current and future shelves dynamically using events. Shelves are available via nearest or FILO
/// </summary>
public static class ShelfDatabase
{
    private static List<IShelf> shelvesMap = new List<IShelf>();

    #region Reset Registery

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void ResetRegistery()
    {
        shelvesMap.Clear();
    }

    #endregion

    #region Register Shelves

    /// <summary>
    /// Called when shelf is created
    /// </summary>
    /// <param name="shelf"></param>
    public static void RegisterShelf(IShelf shelf)
    {
        if (shelvesMap.Contains(shelf))
        {
            Debug.Log("Shelf already exists?");
            return;
        }
        shelvesMap.Add(shelf);
    }

    /// <summary>
    /// Called when shelf is removed
    /// </summary>
    /// <param name="shelf"></param>
    public static void UnregisterShelf(IShelf shelf)
    {
        if (!shelvesMap.Contains(shelf))
        {
            return;
        }
        shelvesMap.Remove(shelf);
    }

    #endregion

    #region Register Queries

    /// <summary>
    /// Returns the first shelf with a given StockID
    /// </summary>
    /// <param name="stockID"></param>
    /// <returns>First found shelf</returns>
    public static IShelf FindShelfWithItem(int stockID)
    {
        foreach (var shelf in shelvesMap)
        {
            if (!shelf.ShelfHasItem(stockID))
            {
                continue;
            }
            return shelf;
        }
        return null;
    }

    /// <summary>
    /// Finds a shelf with a specific StockID and closest with a given position
    /// </summary>
    /// <param name="stockID"></param>
    /// <param name="position"></param>
    /// <returns>Shelf of closest distance</returns>
    public static IShelf FindShelfWithItem(int stockID, Vector3 position)
    {
        float distance = Mathf.Infinity;
        IShelf closest_shelf = null;
        
        foreach (var shelf in shelvesMap)
        {
            if (!shelf.ShelfHasItem(stockID))
            {
                continue;
            }

            if (Vector3.Distance(position, shelf.GetShelfPosition()) < distance)
            {
                distance = Vector3.Distance(position, shelf.GetShelfPosition());
                closest_shelf = shelf;
            }
        }
        return closest_shelf;
    }

    /// <summary>
    /// Returns all shelves within this database
    /// </summary>
    /// <returns>List of all IShelf's stored</returns>
    public static List<IShelf> GetAllShelves()
    {
        return new List<IShelf>(shelvesMap);
    }

    #endregion
}