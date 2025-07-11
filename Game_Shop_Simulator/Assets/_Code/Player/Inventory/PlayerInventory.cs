using UnityEngine;

namespace GSS.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private InputData inputData;

        private ICollectable heldObject = null;

        #region Startup

        private void OnEnable()
        {
            if (!inputData)
            {
                Debug.LogWarning("No InputData found || " + this.name);
                this.enabled = false;
                return;
            }

            inputData.Initialise();
            inputData.OnDropEvent += RemoveItem;
            InventoryHandler.OnAddCollectable += AddItemToInventory;
        }

        private void OnDisable()
        {
            if (!inputData)
            {
                return;
            }

            inputData.OnDropEvent -= RemoveItem;
            inputData.Cleanup();
            InventoryHandler.OnAddCollectable -= AddItemToInventory;
        }

        #endregion

        private bool AddItemToInventory(ICollectable item)
        {
            if (heldObject != null)
            {
                return false;
            }

            heldObject = item;
            item.Collect();
            return true;
        }

        private void RemoveItem()
        {
            if (heldObject == null)
            {
                return;
            }

            heldObject.Drop();
            heldObject = null;
        }
    }
}