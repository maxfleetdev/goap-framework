using System;

public static class StoreFinanceEvents
{
    public static event Func<float, bool> OnTryPurchase;
    public static event Action<float> OnSellItem;
    public static event Action<float> OnBalanceUpdated;

    public static bool TryPurchase(float amount)
    {
        if (OnTryPurchase != null)
        {
            return OnTryPurchase.Invoke(amount);
        }
        return false;
    }

    public static void SellItem(float amount)
    {
        OnSellItem?.Invoke(amount);
    }

    public static void BalanceUpdated(float amount)
    {
        OnBalanceUpdated?.Invoke(amount);
    }
}