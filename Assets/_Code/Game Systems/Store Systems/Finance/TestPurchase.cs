using UnityEngine;

public class TestPurchase : MonoBehaviour
{
    [SerializeField] private StockData stockData;
    [Space]
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform spawnLocation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StockBoxItem box = boxPrefab.GetComponent<StockBoxItem>();
            if (box == null)
            {
                return;
            }

            if (StoreFinanceEvents.TryPurchase(stockData.PerBoxPrice))
            {
                GameObject prefab = Instantiate(boxPrefab, spawnLocation.position, Quaternion.identity);
                prefab.GetComponent<StockBoxItem>().SetItem(stockData);
            }
        }
    }
}
