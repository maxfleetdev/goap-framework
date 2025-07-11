using UnityEngine;

namespace GSS.Player
{
    public class PlayerRaycaster : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private InputData inputData;
        
        [Header("Raycast Settings")]
        [SerializeField] private float rayLength = 2f;
        [SerializeField] private Transform rayOrigin;
        [SerializeField] private LayerMask ignoreLayer;
        [SerializeField] private string interactTag = "Interact";

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
            inputData.OnPickupEvent += StartRaycast;
            RaycastHandler.OnRaycast += GetRaycastObject;
        }

        private void OnDisable()
        {
            if (!inputData)
            {
                return;
            }

            inputData.OnPickupEvent -= StartRaycast;
            inputData.Cleanup();
            RaycastHandler.OnRaycast -= GetRaycastObject;
        }

        #endregion

        #region Raycast Logic

        private void StartRaycast()
        {
            // Setup Ray
            Vector3 forward = rayOrigin.TransformDirection(Vector3.forward);
            Ray ray = new Ray(rayOrigin.position, forward);
            RaycastHit hit;

            // Check Ray for any objects (minus player layer)
            if (Physics.Raycast(ray, out hit, rayLength, ~ignoreLayer))
            {
                IInteractable interactable = CheckRayHit(hit);
                if (interactable != null)
                {
                    interactable.OnInteract();
                }
            }
        }

        private IInteractable CheckRayHit(RaycastHit hit)
        {
            Transform hit_obj = hit.transform;

            // Raycast hit interactable object
            if (hit_obj.CompareTag(interactTag))
            {
                if (hit_obj.GetComponent<IInteractable>() != null)
                {
                    Debug.DrawLine(rayOrigin.position, hit.point, Color.green, 2f);
                    return hit_obj.GetComponent<IInteractable>();
                }
            }

            // Raycast hit other object
            Debug.DrawLine(rayOrigin.position, hit.point, Color.red, 2f);
            return null;
        }

        private IInteractable GetRaycastObject()
        {
            Vector3 forward = rayOrigin.TransformDirection(Vector3.forward);
            Ray ray = new Ray(rayOrigin.position, forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength, ~ignoreLayer))
            {
                return CheckRayHit(hit);
            }
            return null;
        }

        #endregion
    }
}