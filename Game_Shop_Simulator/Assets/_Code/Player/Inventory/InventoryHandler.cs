using System;

public static class InventoryHandler
{
    public static event Func<ICollectable, bool> OnAddCollectable;

    /// <summary>
    /// Returns boolean if adding to Inventory was successful
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Whether item was added</returns>
    public static bool AddToInventory(ICollectable item)
    {
        if (OnAddCollectable != null)
        {
            foreach (var request in OnAddCollectable.GetInvocationList())
            {
                if (request is Func<ICollectable, bool> handler)
                {
                    return handler.Invoke(item);
                }
            }
        }

        return false;
    }
}