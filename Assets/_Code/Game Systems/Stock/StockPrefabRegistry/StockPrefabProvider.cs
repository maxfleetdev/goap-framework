using UnityEngine;

public class StockPrefabProvider : MonoBehaviour, IProvidable<int>
{
    [SerializeField] private StockData stockData;

    /// <summary>
    /// Returns the StockID of the attached StockData
    /// </summary>
    /// <returns>StockID Value</returns>
    public int Get() => stockData == null ? -1 : stockData.StockID;
}
