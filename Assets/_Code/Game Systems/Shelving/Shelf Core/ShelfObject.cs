using System.Collections.Generic;
using UnityEngine;
using GSS.AI;

/// <summary>
/// Main class which maps all children rows and their currently active StockData
/// </summary>
[RequireComponent(typeof(ShelfBrowseZone))]
public class ShelfObject : MonoBehaviour, IShelf, IPlaceable
{
    [SerializeField] private ShelfSizeProfile shelfProfile;
    [SerializeField] private StockCategory category = StockCategory.Any;

    private Dictionary<int, List<ShelfRow>> shelfRowMap = new();
    private ShelfBrowseZone browseZone;

    public ShelfSizeProfile ShelfProfile
    {
        get => shelfProfile;
    }

    #region Shelf Creation

    private void Start()
    {
        InitialiseRows();
    }

    public void OnPlaced()
    {
        ShelfCreation();
    }

    public void OnRemoved()
    {
        ShelfRemoval();
    }

    #endregion

    #region Shelf Register

    private void InitialiseRows()
    {
        browseZone = GetComponent<ShelfBrowseZone>();
        shelfRowMap.Clear();
        Shelf shelf = new Shelf(category, shelfProfile, this);

        // Get all shelf rows in children objects
        foreach (var row in GetComponentsInChildren<ShelfRow>())
        {
            row.InitialiseRow(shelf);
            int stockID = row.StockID;

            // StockID exists, add to StockRow List
            if (shelfRowMap.ContainsKey(stockID))
            {
                shelfRowMap[stockID].Add(row);
                continue;
            }

            // Create new StockID & List of Rows
            shelfRowMap.Add(stockID, new List<ShelfRow>());
            shelfRowMap[stockID].Add(row);
        }
    }

    public void ShelfCreation()
    {
        browseZone = GetComponent<ShelfBrowseZone>();
        if (browseZone == null)
        {
            Debug.LogWarning("Shelf has no BrowseZone Attached!");
            return;
        }
        browseZone.Initialise();

        ShelfDatabase.RegisterShelf(this);
    }

    public void ShelfRemoval()
    {
        if (browseZone == null)
        {
            return;
        }

        ShelfDatabase.UnregisterShelf(this);
    }

    #endregion

    #region Row Events

    public void RowItemChanged(ShelfRow row, int previousID)
    {
        // Remove old entry
        if (shelfRowMap.ContainsKey(previousID))
        {
            shelfRowMap[previousID].Remove(row);

            if (shelfRowMap[previousID].Count == 0)
            {
                shelfRowMap.Remove(previousID);
            }
        }

        // Only add row if valid stockID
        int stockID = row.StockID;
        if (stockID == 0)
        {
            return;
        }

        if (!shelfRowMap.ContainsKey(stockID))
        {
            shelfRowMap[stockID] = new List<ShelfRow>();
        }
        shelfRowMap[stockID].Add(row);
    }

    public bool TryTakeItem(int stockID)
    {
        if (!shelfRowMap.ContainsKey(stockID))
        {
            return false;
        }

        var rows = shelfRowMap[stockID];
        foreach (var row in rows)
        {
            // Ensure the row is not only non-empty, but also still holds the requested item
            if (row.StockID == stockID && row.CanRemoveStock())
            {
                row.RemoveStock();
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Shelf Queries

    public bool ShelfHasItem(int stockID)
    {
        return shelfRowMap.ContainsKey(stockID);
    }

    public Vector3 GetShelfPosition()
    {
        return transform.position;
    }

    public bool BrowsePointAvailable()
    {
        return browseZone.IsSpotAvailable();
    }

    public bool GetBrowsePoint(AIAgent agent, out Vector3 position)
    {
        return browseZone.TryAssignSpot(agent, out position);
    }

    public void ReleaseBrowsePoint(AIAgent agent)
    {
        browseZone.ReleaseSpot(agent);
    }

    public bool IsShelfEmpty()
    {
        foreach (var rows in shelfRowMap.Values)
        {
            if (rows.Count == 0)
            {
                continue;
            }

            foreach (ShelfRow row in rows)
            {
                if (!row.IsRowEmpty())
                {
                    return false;
                }
            }
        }
        return true;
    }

    #endregion
}