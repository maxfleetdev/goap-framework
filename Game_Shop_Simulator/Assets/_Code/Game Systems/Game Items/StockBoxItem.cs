using UnityEngine;

/// <summary>
/// Class using IUsable for removing and adding stock to a row
/// </summary>
public class StockBoxItem : MonoBehaviour, IUsable
{
    private StockContainer stockContainer;
    private StockData currentItem;

    private InputData inputData;
    private bool isHeld = false;

    #region Runtime

    private void Awake()
    {
        isHeld = false;
    }

    public void SetItem(StockData item)
    {
        stockContainer = new StockContainer(item, item.StockPerBox);
        currentItem = item;
        transform.name = $"Box of {currentItem.StockName} Stock";
    }

    #endregion

    #region IUsable Methods

    public void SubscribeToInput(InputData input)
    {
        if (input == null)
        {
            return;
        }

        isHeld = true;
        inputData = input;
        inputData.OnPlaceEvent += UseItem;
        inputData.OnRemoveEvent += TakeFromShelf;
    }

    public void UnsubscribeFromInput()
    {
        if (inputData == null)
        {
            return;
        }

        isHeld = false;
        inputData.OnPlaceEvent -= UseItem;
        inputData.OnRemoveEvent -= TakeFromShelf;
        inputData = null;
    }

    /// <summary>
    /// Removes stock from StockContainer and adds stock to Shelf
    /// </summary>
    public void UseItem()
    {
        // If for some reason its not held
        if (!isHeld)
        {
            Logger.Warn(typeof(StockBoxItem).Name, $"Trying to use whilst not held!");
            return;
        }

        // Get shelf from raycast
        ShelfRow shelf = RaycastHandler.GetShelfRow();
        if (shelf == null)
        {
            return;
        }

        // Check we can remove from stock container & add stock to shelf
        if (stockContainer.IsEmpty || !shelf.CanAddStock(currentItem))
        {
            return;
        }

        // Add stock to shelf & remove from stock container
        bool emptied = stockContainer.RemoveStock();
        shelf.AddStock(currentItem);
        if (emptied)
        {
            currentItem = null;
            transform.name = $"Empty Box";
        }
    }

    #endregion

    #region Use Logic

    /// <summary>
    /// Adds stock into StockContainer and removes from the Shelf
    /// </summary>
    private void TakeFromShelf()
    {
        // If for some reason its not held
        if (!isHeld)
        {
            Logger.Warn(typeof(StockBoxItem).Name, $"Trying to use whilst not held!");
            return;
        }

        // Get shelf from raycast
        ShelfRow shelf = RaycastHandler.GetShelfRow();
        if (shelf == null || (shelf.ShelfItem != currentItem && stockContainer.ContainerContent != null))
        {
            return;
        }

        // Do not add more stock if container is full
        if (stockContainer.IsFull)
        {
            return;
        }

        // Check Shelf has space/Same item as StockContainer
        if (stockContainer.IsEmpty)
        {
            stockContainer.SetStockData(shelf.ShelfItem);
            currentItem = shelf.ShelfItem;
            transform.name = $"Box of {currentItem.StockName} Stock";           // just for visuals
        }

        stockContainer.AddStock();
        shelf.RemoveStock();
    }

    #endregion
}