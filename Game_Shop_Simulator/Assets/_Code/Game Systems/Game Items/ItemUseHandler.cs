using UnityEngine;

/// <summary>
/// Generic class for interacting with a IUsable interface class
/// </summary>
[RequireComponent(typeof(IUsable))]
public class ItemUseHandler : MonoBehaviour
{
    [SerializeField] private InputData inputData;

    private bool isHeld;
    private IUsable usableItem;

    private MeshRenderer objMeshRenderer;
    private Rigidbody objRigidBody;
    private Collider objCollider;

    #region Runtime

    private void Awake()
    {
        usableItem = GetComponent<IUsable>();
        isHeld = false;

        // Object Visuals
        objMeshRenderer = GetComponentInChildren<MeshRenderer>();
        objRigidBody = GetComponentInChildren<Rigidbody>();
        objCollider = GetComponentInChildren<Collider>();
    }

    private void OnDestroy()
    {
        if (usableItem != null)
        {
            usableItem.UnsubscribeFromInput();
        }
    }

    #endregion

    #region Collectable Methods

    /// <summary>
    /// Calls IUsable when successfully picked up by Player
    /// </summary>
    public void OnItemPickup()
    {
        if (isHeld)
        {
            return;
        }

        isHeld = true;
        if (inputData != null && usableItem != null)
        {
            usableItem.SubscribeToInput(inputData);
        }

        // Hides object in World
        ToggleVisibility(false);
    }

    /// <summary>
    /// Calls IUsable when successfully dropped by Player
    /// </summary>
    public void OnItemDrop()
    {
        if (!isHeld)
            return;

        isHeld = false;
        if (inputData != null && usableItem != null)
        {
            usableItem.UnsubscribeFromInput();
        }

        // Shows object in World + new position
        SetObjectTransform();
        ToggleVisibility(true);
    }

    #endregion

    #region Object Visuals

    private void ToggleVisibility(bool toggle)
    {
        objRigidBody.isKinematic = !toggle;
        objCollider.enabled = toggle;
        objMeshRenderer.enabled = toggle;
    }

    private void SetObjectTransform()
    {
        // Get Players Transform
        Transform player_transform = PlayerHandler.GetPlayerTransform();
        if (player_transform == null)
        {
            return;
        }

        // Set Position of Rigidbody
        Vector3 position = player_transform.position + player_transform.forward * 1.75f;
        position.y += 0.75f;
        objRigidBody.position = position;

        // Set Rotation of Rigidbody
        Quaternion forward = Quaternion.LookRotation(player_transform.forward);
        objRigidBody.rotation = forward;
    }

    #endregion
}