using UnityEngine;

/// <summary>
/// Uses IInteractable and ICollectable to communicate with PlayerRaycaster
/// </summary>
[RequireComponent(typeof(ItemUseHandler))]
public class CollectObject : MonoBehaviour, IInteractable, ICollectable
{
    private ItemUseHandler useHandler;
    private IUsable visualItem;

    private void Awake()
    {
        useHandler = GetComponent<ItemUseHandler>();
        visualItem = GetComponent<IUsable>();
    }

    #region IInteractable

    public void OnInteract()
    {
        InventoryHandler.AddToInventory(this);
    }

    #endregion

    #region ICollectable

    public void Collect()
    {
        useHandler.OnItemPickup();
        ItemVisualiserHandler.ItemCollected(visualItem);
    }

    public void Drop()
    {
        useHandler.OnItemDrop();
        ItemVisualiserHandler.ItemDropped();
    }

    #endregion
}