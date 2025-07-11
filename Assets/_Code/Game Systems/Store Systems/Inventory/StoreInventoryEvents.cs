using System;

/// <summary>
/// Events which interact with InventoryManager class
/// </summary>
public static class StoreInventoryEvents
{
    public static event Action<int, int> OnStockAdded;
    public static event Action<int, int> OnStockRemoved;
    public static event Func<int, int> OnGetStockAmount;

    /// <summary>
    /// Increments stock to StoreInventory with given ID and Amount
    /// </summary>
    /// <param name="stockID"></param>
    /// <param name="amount"></param>
    public static void AddStock(int stockID, int amount)
    {
        OnStockAdded?.Invoke(stockID, amount);
    }

    /// <summary>
    /// Decrements stock to StoreInventory with given ID and Amount.
    /// </summary>
    /// <param name="stockID"></param>
    /// <param name="amount"></param>
    public static void RemoveStock(int stockID, int amount)
    {
        OnStockRemoved?.Invoke(stockID, amount);
    }

    /// <summary>
    /// Returns the total stock of a given StockID
    /// </summary>
    /// <param name="stockID"></param>
    /// <returns>Amount of Stock</returns>
    public static int GetStockAmount(int stockID)
    {
        if (OnGetStockAmount != null)
        {
            foreach (var request in OnGetStockAmount.GetInvocationList())
            {
                if (request is Func<int, int> handler)
                {
                    return handler.Invoke(stockID);
                }
            }
        }
        return -1;
    }
}
