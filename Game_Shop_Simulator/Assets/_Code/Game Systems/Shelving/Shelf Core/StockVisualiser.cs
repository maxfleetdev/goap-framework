using UnityEngine;

public class StockVisualiser : MonoBehaviour
{
    [SerializeField] private GameObject smallShelfStock;
    [SerializeField] private GameObject mediumShelfStock;
    [SerializeField] private GameObject largeShelfStock;

    private GameObject currentStock = null;
    private MeshRenderer[] childMeshes = null;
    
    private int currentIndex = 0;
    private int stockCount = 0;

    /// <summary>
    /// The amount of stock that can be held on a shelf for this sized prefab
    /// </summary>
    public int StockCount
    {
        get => stockCount;
    }

    #region Start/Reset

    /// <summary>
    /// Caches variables and toggles correct sized prefab
    /// </summary>
    /// <param name="size"></param>
    public void ResetStock(ShelfSize size)
    {
        switch (size)
        {
            case ShelfSize.Small:
                currentStock = smallShelfStock;
                mediumShelfStock.SetActive(false);
                largeShelfStock.SetActive(false);
                break;
            
            case ShelfSize.Medium:
                currentStock = mediumShelfStock;
                smallShelfStock.SetActive(false);
                largeShelfStock.SetActive(false);
                break;
            
            case ShelfSize.Large:
                currentStock = largeShelfStock;
                smallShelfStock.SetActive(false);
                mediumShelfStock.SetActive(false);
                break;
        }

        currentStock.SetActive(true);
        GetMeshes();
        EnableStock();
    }

    private void GetMeshes()
    {
        childMeshes = currentStock.GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in childMeshes)
        {
            mesh.enabled = false;
        }
        currentIndex = 0;
        stockCount = childMeshes.Length;
    }

    #endregion

    #region Enable/Disable Mesh

    /// <summary>
    /// Enables the next stock in the mesh index
    /// </summary>
    public void EnableStock()
    {
        // Get all meshes if prefab
        if (childMeshes == null)
        {
            GetMeshes();
        }

        // Stay in bounds
        currentIndex = Mathf.Min(currentIndex, stockCount - 1);
        childMeshes[currentIndex].enabled = true;
        currentIndex++;
    }

    /// <summary>
    /// Disables the last mesh in the index
    /// </summary>
    public void DisableStock()
    {
        if (currentIndex == 0)
        {
            return;
        }

        // Stay in bounds
        currentIndex--;
        currentIndex = Mathf.Max(0, currentIndex);
        childMeshes[currentIndex].enabled = false;
    }

    #endregion
}