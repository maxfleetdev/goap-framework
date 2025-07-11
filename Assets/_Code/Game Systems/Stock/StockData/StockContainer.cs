/// <summary>
/// Container class for holding information on a boxes content and amount
/// </summary>
public class StockContainer
{
    // Variables
    private StockData containerContent;
    private int containerAmount;
    private int maxAmount;

    // Properties
    public StockData ContainerContent
    {
        get => containerContent; 
        private set => containerContent = value;
    }
    public bool IsEmpty
    {
        get => containerAmount == 0;
    }
    public bool IsFull
    {
        get => containerAmount >= maxAmount;
    }

    public StockContainer(StockData item, int amount)
    {
        containerContent = item;
        containerAmount = amount;
        maxAmount = item.StockPerBox;
    }


    /// <summary>
    /// Increments Stock's Container Amount
    /// </summary>
    public void AddStock()
    {
        containerAmount++;
    }

    /// <summary>
    /// Decrements Stock's Container Amount
    /// </summary>
    public bool RemoveStock()
    {
        containerAmount--;
        // Make StockData null and ready to set
        if (containerAmount == 0)
        {
            containerContent = null;
            return true;
        }

        return false;
    }

    public void SetStockData(StockData item)
    {
        if (containerContent == null)
        {
            containerContent = item;
            maxAmount = item.StockPerBox;
        }
    }
}