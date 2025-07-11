using System;

public static class ItemVisualiserHandler
{
    public static event Action<IUsable> OnItemCollected;
    public static event Action OnItemDropped;

    public static void ItemCollected(IUsable obj)
    {
        OnItemCollected?.Invoke(obj);
    }

    public static void ItemDropped()
    {
        OnItemDropped?.Invoke();
    }
}