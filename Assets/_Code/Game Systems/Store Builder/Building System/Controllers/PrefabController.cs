using System.Threading.Tasks;
using UnityEngine;

namespace GSS.Building
{
    /// <summary>
    /// Called when an object needs to be created or removed using their given PooledType. 
    /// Takes a given prefab to create at a certain point or remove from its Pool.
    /// </summary>
    public static class PrefabController
    {
        /// <summary>
        /// Creates a Pooled Prefab using it's ID at a provided position
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="data"></param>
        /// <param name="ghostTransform"></param>
        public async static Task Place(PooledType pooled_type, Transform set_transform, int object_id)
        {
            // Invalid Object to create
            if (set_transform == null || object_id == -1)
            {
                return;
            }

            // Awaits given PooledType to create
            GameObject placed = await GetPooledObject(pooled_type, object_id);

            // No pooled object was created
            if (placed == null)
            {
                return;
            }

            // Call Provider that this Prefab has been placed
            var provider = placed.GetComponent<PlaceableObjectProvider>();
            provider?.PrefabPlaced();
            
            // Calculate and apply bounds
            CustomBoundVolume bound_volume = placed.GetComponentInChildren<CustomBoundVolume>();
            bound_volume?.ApplyBounds();

            // Set pooled object's location to set transform
            placed.transform.SetPositionAndRotation(set_transform.transform.position, set_transform.transform.rotation);
        }

        /// <summary>
        /// Removes a given object from a Pool (if found)
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="object_id"></param>
        public static void Remove(PooledType pooled_type, Transform transform, int object_id)
        {
            // Invalid Object to remove
            if (transform == null || object_id == -1)
            {
                return;
            }

            // Call Provider that this Prefab has been removed
            var provider = transform.GetComponent<PlaceableObjectProvider>();
            provider?.PrefabRemoved();

            // Remove from given pool
            ReturnPooledObject(pooled_type, object_id, transform.gameObject);
        }

        #region PooledType Get/Return

        // Returns the created/enabled pooled object
        private static async Task<GameObject> GetPooledObject(PooledType type, int id)
        {
            switch (type)
            {
                case PooledType.Buildable: 
                    return await BuildPoolEvents.GetBuildable(id);
                
                default: 
                    return null;
            }
        }

        // Removes the pooled object
        private static void ReturnPooledObject(PooledType type, int id, GameObject obj)
        {
            switch (type)
            {
                case PooledType.Buildable: 
                    BuildPoolEvents.ReturnBuildable(id, obj); 
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}