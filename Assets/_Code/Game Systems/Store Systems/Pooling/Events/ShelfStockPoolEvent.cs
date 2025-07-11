using System;
using System.Threading.Tasks;
using UnityEngine;

public static class ShelfStockPoolEvent
{
    public static event Action<int, GameObject> OnReturnToPool;
    public static event Func<int, Task<GameObject>> OnGetStockItem;

    /// <summary>
    /// Instantiates or Enables a pooled StockItem prefab with a given StockID
    /// </summary>
    /// <param name="stockID"></param>
    /// <returns>StockItem Prefab</returns>
    public static async Task<GameObject> GetStockItem(int stockID)
    {
        if (OnGetStockItem != null)
        {
            return await OnGetStockItem.Invoke(stockID);
        }
        return null;
    }

    /// <summary>
    /// Enqueues a given object for future use
    /// </summary>
    /// <param name="stockID"></param>
    /// <param name="obj"></param>
    public static void ReturnToPool(int stockID, GameObject obj)
    {
        OnReturnToPool?.Invoke(stockID, obj);
    }
}