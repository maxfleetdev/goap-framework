using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ManagerInitialiser))]
public class StorePricingManager : MonoBehaviour, IManageable
{
    private Dictionary<int, float> stockPrices = new Dictionary<int, float>();

    private bool isInitialising = false;

    #region Initialise Process

    public void Initialise()
    {
        isInitialising = false;
        InitialiseEvents();
    }

    private void InitialiseEvents()
    {
        StorePricingEvents.OnGetPrice += GetPrice;
        StorePricingEvents.OnSetPrice += SetPrice;
        StockDataRegistry.OnLoaded += InitialisePrices;
    }

    private async void InitialisePrices()
    {
        // Cannot multi-initialise
        if (isInitialising)
        {
            return;
        }

        // Start initialisation process
        isInitialising = true;
        StockDataRegistry.OnLoaded -= InitialisePrices;

        // Get all LoadedData from Loaded Registry
        List<StockData> all_stock = await StockDataRegistry.GetAllLoadedDataAsync();
        foreach (StockData item in all_stock)
        {
            stockPrices[item.StockID] = item.StockRRP;
        }

        Logger.Loaded(this.GetType().Name, stockPrices.Count, "StockData Prices");
    }

    #endregion

    #region Termination Process

    public void Terminate()
    {
        StorePricingEvents.OnGetPrice -= GetPrice;
        StorePricingEvents.OnSetPrice -= SetPrice;
    }

    #endregion

    #region Pricing Logic

    private void SetPrice(int stock_id, float price)
    {
        if (stockPrices.ContainsKey(stock_id))
        {
            // Price cannot be lower than 0
            stockPrices[stock_id] = Mathf.Max(0, price);
            Debug.Log($"Price of ID{stock_id} set to {stockPrices[stock_id]}");
        }
    }

    private float GetPrice(int stock_id)
    {
        return stockPrices.TryGetValue(stock_id, out float price) ? price : -1;
    }

    #endregion
}