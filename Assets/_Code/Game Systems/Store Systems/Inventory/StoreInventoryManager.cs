using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ManagerInitialiser))]
public class StoreInventoryManager : MonoBehaviour, IManageable
{
    private Dictionary<int, int> storeInventory = new Dictionary<int, int>();

    #region Initialise Process

    public void Initialise()
    {
        InitialiseInventory();
    }

    private void InitialiseInventory()
    {
        storeInventory.Clear();
        StoreInventoryEvents.OnStockAdded += AddStock;
        StoreInventoryEvents.OnStockRemoved += RemoveStock;
        StoreInventoryEvents.OnGetStockAmount += GetStock;
    }

    #endregion

    #region Termination Process

    public void Terminate()
    {
        StoreInventoryEvents.OnStockAdded -= AddStock;
        StoreInventoryEvents.OnStockRemoved -= RemoveStock;
        StoreInventoryEvents.OnGetStockAmount -= GetStock;
    }

    #endregion

    #region Inventory Logic

    private void AddStock(int stock_id, int amount)
    {
        // Cannot remove negatives
        amount = Mathf.Max(0, amount);

        // Add new object if not within dictionary
        if (!storeInventory.ContainsKey(stock_id))
        {
            storeInventory.Add(stock_id, 0);
        }
        
        // Add amount to dictionary object
        storeInventory[stock_id] += amount;

        Debug.Log($"Added: ID{stock_id} + {amount}");
    }

    private void RemoveStock(int stock_id, int amount)
    {
        // Cannot remove stock that doesn't exist
        if (!storeInventory.ContainsKey(stock_id))
        {
            return;
        }

        // Cannot remove negatives
        amount = Mathf.Max(0, amount);

        // Cannot remove more than currently stocked
        if (storeInventory[stock_id] - amount < 0)
        {
            Debug.Log($"Not enough stock to remove ID{stock_id}");
            return;
        }

        // Remove from current amount
        storeInventory[stock_id] -= amount;
        
        // No more stock left
        if (storeInventory[stock_id] <= 0)
        {
            storeInventory.Remove(stock_id);
        }
        Debug.Log($"Removed: ID{stock_id} + {amount}");
    }

    private int GetStock(int stock_id)
    {
        if (storeInventory.ContainsKey(stock_id))
        {
            return storeInventory[stock_id];
        }
        Debug.LogWarning($"ID:{stock_id} does not exist within StockInventory");
        return -1;
    }

    #endregion
}