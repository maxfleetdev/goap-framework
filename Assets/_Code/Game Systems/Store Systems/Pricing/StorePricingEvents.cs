using System;

/// <summary>
/// Events which interact with PricingManager class
/// </summary>
public static class StorePricingEvents
{
    public static event Action<int, float> OnSetPrice;
    public static event Func<int, float> OnGetPrice;

    /// <summary>
    /// Sets new price of given StockID
    /// </summary>
    /// <param name="stock_id"></param>
    /// <param name="price"></param>
    public static void SetPrice(int stock_id, float price)
    {
        OnSetPrice?.Invoke(stock_id, price);
    }

    /// <summary>
    /// Returns the price of a given StockID
    /// </summary>
    /// <param name="stock_id"></param>
    /// <returns>Price of Stock</returns>
    public static float GetPrice(int stock_id)
    {
        if (OnGetPrice != null)
        {
            foreach (var request in OnGetPrice.GetInvocationList())
            {
                if (request is Func<int, float> handler)
                {
                    return handler.Invoke(stock_id);
                }
            }
        }
        return -1;
    }
}