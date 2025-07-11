using UnityEngine;

/// <summary>
/// Main pool class, using GenericPool, which enables and disables pooled objects. Subscribes to PoolingEvents for input
/// </summary>
public class PoolManager : MonoBehaviour, IManageable
{
    // Root pooled objects
    private GameObject poolObjects;

    // Stock/Shelf Objects
    private GenericPool<GameObject> stockPool;
    private GameObject stockParent;

    // Buildable Objects
    private GenericPool<GameObject> buildablePool;
    private GameObject buildableParent;

    // Item Objects
    private GenericPool<GameObject> aiPool;
    private GameObject aiParent;

    public void Initialise()
    {
        // Create GameObjects to organise pooled objects
        poolObjects = new GameObject("PooledObjects");
        stockParent = new GameObject("StockPool");
        buildableParent = new GameObject("BuildPool");
        aiParent = new GameObject("AIPool");

        // SHELF STOCK POOLING
        stockParent.transform.parent = poolObjects.transform;
        stockPool = new GenericPool<GameObject>(async id => 
        {
            GameObject prefab = await StockPrefabRegistry.GetPrefabAsync(id);
            if (prefab == null)
            {
                return null;
            }
            return GameObject.Instantiate(prefab, stockParent.transform);
        });
        
        // BUILDABLE POOLING
        buildableParent.transform.parent = poolObjects.transform;
        buildablePool = new GenericPool<GameObject>(async id =>
        {
            GameObject prefab = await BuildableAssetRegistry.GetPrefabAsync(id);
            if (prefab == null)
            {
                return null;
            }
            return GameObject.Instantiate(prefab, buildableParent.transform);
        });

        // AI POOLING
        aiParent.transform.parent = poolObjects.transform;
        aiPool = new GenericPool<GameObject>(async id =>
        {
            GameObject prefab = await AIPrefabRegistry.GetPrefabAsync(id);
            if (prefab == null)
            {
                return null;
            }
            return GameObject.Instantiate(prefab, aiParent.transform);
        });

        // Subscribe to event systems
        ShelfStockPoolEvent.OnGetStockItem += stockPool.GetAsync;
        ShelfStockPoolEvent.OnReturnToPool += stockPool.Return;

        BuildPoolEvents.OnGetBuildable += buildablePool.GetAsync;
        BuildPoolEvents.OnReturnBuildable += buildablePool.Return;

        AIPoolEvents.OnGetAI += aiPool.GetAsync;
        AIPoolEvents.OnReturnToPool += aiPool.Return;
    }

    public void Terminate()
    {
        // Clear pool objects
        GameObject.Destroy(poolObjects);

        // Unsubscribe to event systems
        ShelfStockPoolEvent.OnGetStockItem -= stockPool.GetAsync;
        ShelfStockPoolEvent.OnReturnToPool -= stockPool.Return;

        BuildPoolEvents.OnGetBuildable -= buildablePool.GetAsync;
        BuildPoolEvents.OnReturnBuildable -= buildablePool.Return;

        AIPoolEvents.OnGetAI -= aiPool.GetAsync;
        AIPoolEvents.OnReturnToPool -= aiPool.Return;

        // Clear pools
        if (stockPool != null)
        {
            stockPool.Clear();
        }
        if (buildablePool != null)
        {
            buildablePool.Clear();
        }
        if (aiPool != null)
        {
            aiPool.Clear();
        }
    }
}