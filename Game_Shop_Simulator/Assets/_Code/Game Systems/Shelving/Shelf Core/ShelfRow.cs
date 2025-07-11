using UnityEngine;

/// <summary>
/// Main Shelf class which stores the current StockData and it's current stock level within a row. Used with IInteractable
/// for player interaction with this specific row
/// </summary>
public class ShelfRow : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform stockHoldPoint;
    
    private int itemAmount = 0;
    private int maxStock = 1;
    private int stockID = -1;

    private StockData shelfItem = null;
    private StockVisualiser visualItem = null;
    private GameObject visualObject = null;
    private IShelf shelfObject = null;

    private BoxCollider rowCollider = null;

    private ShelfSizeProfile shelfProfile;
    private StockCategory category = StockCategory.Any;

    #region Properties

    /// <summary>
    /// Current StockItem being held within this row
    /// </summary>
    public StockData ShelfItem
    {
        get => shelfItem;
        private set => shelfItem = value;
    }
    /// <summary>
    /// Cached StockItem ID for this row
    /// </summary>
    public int StockID
    {
        get => stockID;
        private set => stockID = value;
    }
    /// <summary>
    /// The current Bounds of this rows BoxCollider
    /// </summary>
    public Bounds RowBounds
    {
        get
        {
            if (rowCollider == null)
            {
                rowCollider = GetComponent<BoxCollider>();
            }
            return rowCollider.bounds;
        }
    }
    /// <summary>
    /// The Transform the StockPrefab is pooled to
    /// </summary>
    public Transform StockHoldPoint
    {
        get => stockHoldPoint == null ? transform : stockHoldPoint;
    }
    public ShelfSizeProfile ShelfProfile
    {
        get
        {
            if (shelfProfile == null)
            {
                shelfProfile = GetComponentInParent<ShelfObject>().ShelfProfile;
            }
            return shelfProfile;
        }
    }

    #endregion

    #region Startup

    public void OnInteract() { }

    public void InitialiseRow(Shelf shelf)
    {
        // Cache parented ShelfObject
        if (shelfObject == null)
        {
            shelfObject = shelf.RootShelfObject;
        }

        // Save root Shelf variables
        category = shelf.ShelfCategory;
        shelfProfile = shelf.ShelfProfile;

        // Cache collider attached to this row
        if (rowCollider == null)
        {
            rowCollider = GetComponent<BoxCollider>();
        }
    }

    #endregion

    #region Adding/Removing Stock

    /// <summary>
    /// Called by BoxItem to directly interact with a specific row. Returns a task when async task is completed
    /// </summary>
    /// <param name="stock"></param>
    /// <returns></returns>
    public async void AddStock(StockData stock)
    {
        // NEW STOCK ADDED
        // Set shelf to current stock item
        if (itemAmount == 0)
        {
            // Get pooled stockitem prefab
            int previous_id = StockID;
            shelfItem = stock;
            stockID = shelfItem.StockID;
            visualObject = await ShelfStockPoolEvent.GetStockItem(stockID);
            if (visualObject == null)
            {
                return;
            }

            // Set prefab position/rotation
            visualObject.transform.position = stockHoldPoint.position;
            visualObject.transform.rotation = stockHoldPoint.rotation;

            // Get stock visualiser
            visualItem = visualObject.GetComponent<StockVisualiser>();
            
            // Initialise Prefab first item
            visualItem.ResetStock(shelfProfile.Size);
            maxStock = visualItem.StockCount;
            itemAmount++;

            // Update ShelfObject
            shelfObject.RowItemChanged(this, previous_id);
        }

        // ADD TO CURRENT STOCK
        // Increment stock amount to shelf
        else if (itemAmount < maxStock && stock == shelfItem)
        {
            if (visualItem != null)
            {
                visualItem.EnableStock();
                itemAmount++;
            }
        }
    }

    /// <summary>
    /// Called by BoxItem to directly interact with a specific row
    /// </summary>
    /// <param name="stock"></param>
    /// <returns></returns>
    public void RemoveStock()
    {
        // Decrease stock amount
        itemAmount--;
        
        // STOCK IS EMPTY
        if (itemAmount == 0)
        {
            // Hold temporarily and return to pool
            GameObject temp = visualObject;
            ShelfStockPoolEvent.ReturnToPool(shelfItem.StockID, temp);

            // Reset shelving variables for new stock
            int previous_id = shelfItem.StockID;
            visualItem = null;
            shelfItem = null;
            visualObject = null;
            stockID = -1;

            shelfObject.RowItemChanged(this, previous_id);

            return;
        }

        // Visually remove stock from prefab
        if (visualItem != null)
        {
            visualItem.DisableStock();
        }
    }

    #endregion

    #region Shelf Checks

    public bool CanAddStock(StockData item)
    {
        // Check if given item's category is the same as this shelf
        if (category != StockCategory.Any && category != item.Category)
        {
            return false;
        }

        // Check if row has enough space to add
        if ((itemAmount + 1) <= maxStock && (shelfItem == item || shelfItem == null))
        {
            return true;
        }
        return false;
    }
    public bool CanRemoveStock() => itemAmount > 0 && shelfItem != null;
    public bool IsRowEmpty() => itemAmount == 0;

    #endregion
}