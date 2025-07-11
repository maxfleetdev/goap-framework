using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class StockDataRegistry
{
    private static AddressableRegistry<StockData, int> internalRegistry = new();
    private static readonly string ADDRESSABLE_LABEL = "stockdata";

    /// <summary>
    /// Called when all Addressables within InternalRegistry is loaded
    /// </summary>
    public static event Action OnLoaded
    {
        add { internalRegistry.OnLoaded += value; }
        remove { internalRegistry.OnLoaded -= value; }
    }

    #region Registry Setup

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void ResetDatabase()
    {
        internalRegistry.Reset();
    }

    /// <summary>
    /// Loads all AssetReferences of StockData from the Addressable group with the "stockdata" label
    /// </summary>
    public static void LoadAll(Action onComplete = null)
    {
        internalRegistry.LoadAll(ADDRESSABLE_LABEL, prefab =>
        {
            var data = prefab;
            if (data == null)
            {
                Debug.LogWarning($"[StockDataRegistry] Missing StockData with: {prefab.StockName})");
                return default;
            }

            return data.StockID;
        }, onComplete);
    }

    #endregion

    #region Registry Queries

    /// <summary>
    /// Returns the StockData associated with a given StockData ID
    /// </summary>
    public static Task<StockData> GetStockDataAsync(int stockID)
    {
        return internalRegistry.GetAsync(stockID);
    }

    /// <summary>
    /// Returns true if the StockData associated with a given StockData ID exists
    /// </summary>
    public static bool IsStockDataLoaded(int stockID)
    {
        if (!internalRegistry.IsInitialised)
        {
            return false;
        }
        return internalRegistry.GetIDExists(stockID);
    }

    /// <summary>
    /// Returns all StockData loaded within this Registry
    /// </summary>
    /// <returns></returns>
    public static Task<List<StockData>> GetAllLoadedDataAsync()
    {
        if (!internalRegistry.IsInitialised)
        {
            return null;
        }
        return internalRegistry.GetAllLoadedDataAsync();
    }

    #endregion
}