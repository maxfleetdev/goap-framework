using UnityEngine;

/// <summary>
/// ScriptableObject used to store static metadata about a sellable item
/// </summary>
[CreateAssetMenu(menuName = "GSS/Stock Item")]
public class StockData : ScriptableObject
{
    [Header("Stock Metadata")]
    [SerializeField] private string stockName = "";
    [SerializeField] [Min(1)] private float stockRRP = 0f;
    [SerializeField] private StockCategory category = StockCategory.Game;
    [SerializeField] [Min(1)] private int stockID = 0;

    [Header("Stock Inventory Data")]
    [SerializeField][Min(1)] private int stockPerBox = 0;
    [SerializeField] private StockSize size = StockSize.Small;

    #region Properties

    /// <summary>
    /// Retail name for this stock
    /// </summary>
    public string StockName
    {
        get => stockName;
        private set => stockName = value;
    }
    /// <summary>
    /// The recommended retail price for this stock
    /// </summary>
    public float StockRRP
    {
        get => stockRRP;
        private set => stockRRP = value;
    }
    /// <summary>
    /// The category type for this stock
    /// </summary>
    public StockCategory Category
    {
        get => category;
        private set => category = value;
    }
    /// <summary>
    /// Unique ID for this stock (used for registry and getting data)
    /// </summary>
    public int StockID
    {
        get => stockID;
        private set => stockID = value;
    }
    /// <summary>
    /// Amount of stock which arrives in a delivered box (maybe change)
    /// </summary>
    public int StockPerBox
    {
        get => stockPerBox;
        private set => stockPerBox = value;
    }
    /// <summary>
    /// Amount of spaces this takes up on a shelf (change in rework)
    /// </summary>
    public StockSize Size
    {
        get => size;
        private set => size = value;
    }
    /// <summary>
    /// Base cost of purchasing stock (StockPerBox * StockRRP * 0.75)
    /// </summary>
    public float PerBoxPrice
    {
        get => stockPerBox * stockRRP * 0.75f;
    }

    #endregion
}