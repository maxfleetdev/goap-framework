using UnityEngine;

public class ItemVisualiser : MonoBehaviour
{
    [SerializeField] private GameObject boxModel;

    private void OnEnable()
    {
        ItemVisualiserHandler.OnItemCollected += ItemPickedUp;
        ItemVisualiserHandler.OnItemDropped += ItemDropped;
    }

    private void OnDisable()
    {
        ItemVisualiserHandler.OnItemCollected -= ItemPickedUp;
        ItemVisualiserHandler.OnItemDropped -= ItemDropped;
    }

    private void ItemPickedUp(IUsable obj)
    {
        boxModel.SetActive(true);
    }

    private void ItemDropped()
    {
        boxModel.SetActive(false);
    }
}