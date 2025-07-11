using System.Collections.Generic;
using UnityEngine;
using GSS.Building;

public class BuildManager : MonoBehaviour
{
    [Header("Input Data")]
    [SerializeField] private InputData inputData;
    [SerializeField] private Vector3 padding;

    [Header("Raycast Data")]
    [SerializeField] private float rayLength = 2f;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private LayerMask ignoreLayer;
    [SerializeField] private float refreshRate = 0.1f;

    [Header("Ghost Object Data")]
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private Color validColour;
    [SerializeField] private Color invalidColour;

    [Header("Debug Settings")]
    [SerializeField] private bool showDebug = true;

    // Blocked tags for overlapping
    private readonly string[] blockedOverlapTags = new string[]
    {
        "Interact", "Buildable"
    };

    // Raycast Data
    private RaycastManager raycastManager = null;
    private Vector3 rayPosition;

    // Ghost Data
    private GhostManager ghostManager = new GhostManager();

    // Prefab Data
    private PlaceableObject currentObjectData;
    private PlacementRule currentBuildRule;

    // Snapping Data
    private int snappingMultiplier = 100;           // used for snapping to nearest position (10 = 0.1, 4 = 0.25 etc)
    private int rotationMultiplier = 30;            // rotates in 30 degree increments
    private Vector3 currentRotation;

    // Prefab Index Data
    private int objectIndex = 0;                    // current object index (value = object ID)
    private int currentIndex = 0;                   // current object index within a list of all object ID's

    // Building Data
    private PlacementController placementController;
    private bool isBuilding = false;
    private bool isValidHit = false;

    #region Enable Manager

    private void OnEnable()
    {
        Initialise();
    }

    private void Initialise()
    {
        if (inputData == null)
        {
            Debug.LogWarning("[BuildManager] No InputData found");
            this.enabled = false;
            return;
        }

        isBuilding = false;

        if (ghostManager == null)
        {
            ghostManager = new GhostManager();
        }

        if (raycastManager == null)
        {
            raycastManager = new RaycastManager(rayOrigin, rayLength, ignoreLayer, blockedOverlapTags, showDebug);
        }

        if (placementController == null)
        {
            placementController = new PlacementController(refreshRate, raycastManager, ghostManager);
        }

        RegisterInput();
    }

    private void RegisterInput()
    {
        inputData.OnBuildModeEvent += ToggleBuildMode;
    }

    #endregion

    #region Disable Manager

    private void OnDisable()
    {
        DisableBuildMode();         // clears ghost and input events
        UnregisterInput();          // clears toggling input
    }

    private void UnregisterInput()
    {
        inputData.OnBuildModeEvent -= ToggleBuildMode;
    }

    #endregion

    #region Toggle BuildMode

    private void ToggleBuildMode()
    {
        isBuilding = !isBuilding;
        
        if (isBuilding)
        {
            EnableBuildMode();
        }

        else
        {
            DisableBuildMode();
        }
    }

    private void EnableBuildMode()
    {
        // Get prefab data and reset index
        List<int> ids = BuildableAssetRegistry.GetAllIDs();
        if (ids != null && ids.Count > 0)
        {
            currentIndex = 0;
            objectIndex = ids[currentIndex];
        }
        SetPrefabObject();

        // Subscribe from Building Input
        inputData.OnPlaceEvent += PlaceObject;
        inputData.OnObjectChangeEvent += ChangeSelection;
        inputData.OnObjectRotateEvent += RotateObject;
        inputData.OnRemoveObjectEvent += RemoveObject;
    }

    private void DisableBuildMode()
    {
        // Clear current ghost object
        if (ghostManager.GhostObject != null)
        {
            ghostManager.DestroyGhost();
        }

        // Unsubscribe from Building Input
        inputData.OnPlaceEvent -= PlaceObject;
        inputData.OnObjectChangeEvent -= ChangeSelection;
        inputData.OnObjectRotateEvent -= RotateObject;
        inputData.OnRemoveObjectEvent -= RemoveObject;
    }

    #endregion

    #region Prefab Selection

    private async void SetPrefabObject()
    {
        // Check if object exists with current Object Index
        if (!BuildableAssetRegistry.IDExists(objectIndex))
        {
            if (showDebug) Debug.LogWarning($"[BuildManager] No Object for ID: {objectIndex}");
        }

        // Get prefab object from registry
        GameObject prefab = await BuildableAssetRegistry.GetPrefabAsync(objectIndex);
        if (prefab == null)
        {
            if (showDebug) Debug.LogWarning($"[BuildManager] No prefab for ID: {objectIndex}");
            return;
        }

        // Define prefab data
        currentObjectData = prefab.GetComponent<IProvidable<PlaceableObject>>().Get();
        currentBuildRule = currentObjectData.BuildRule;

        // Build ghost object
        ghostManager.CreateGhost(prefab, ghostMaterial);
        ghostManager.GhostObject.transform.Rotate(currentRotation, Space.World);
    }

    private void ChangeSelection(float input)
    {
        // Return 1 or -1 only
        int dir = (int)Mathf.Sign(input);

        // Get all object IDs
        List<int> valid_ids = BuildableAssetRegistry.GetAllIDs();
        if (valid_ids == null || valid_ids.Count == 0)
        {
            return;
        }

        currentIndex += dir;

        // Loop to start of list
        if (currentIndex >= valid_ids.Count)
        {
            currentIndex = 0;
        }

        // Loop to end of list
        if (currentIndex < 0)
        {
            currentIndex = valid_ids.Count - 1;
        }

        // Create new Prefab
        objectIndex = valid_ids[currentIndex];
        
        SetPrefabObject();
    }

    #endregion

    #region Runtime

    private void Update()
    {
        // Check building mode and object data
        if (!isBuilding || currentObjectData == null)
        {
            return;
        }

        if (!placementController.ShouldUpdate(Time.time))
        {
            return;
        }

        placementController.UpdatePlacement(currentObjectData, validColour, invalidColour);

        isValidHit = placementController.IsPlacementValid;
        rayPosition = placementController.LastRaycastPosition;
    }

    #endregion

    #region Rotation Logic

    private void RotateObject(float input)
    {
        int rotate_dir = rotationMultiplier * (int)Mathf.Sign(input);
        ghostManager.GhostObject.transform.Rotate(Vector3.up * rotate_dir, Space.World);
        currentRotation = ghostManager.GhostObject.transform.eulerAngles;
    }

    #endregion

    #region Placement Logic

    private void PlaceObject()
    {
        if (!isValidHit)
        {
            if (showDebug) Debug.LogWarning($"Cannot Place: Not valid placement location");
            return;
        }

        if (!currentObjectData.IsPlaceable())
        {
            if (showDebug) Debug.LogWarning($"Cannot Place: ObjectData requirements not met");
            return;
        }
        
        _ = PrefabController.Place(PooledType.Buildable, ghostManager.PrefabTransform, currentObjectData.ObjectID);
    }

    #endregion

    #region Removal Logic

    private readonly string BUILDABLE_TAG = "Buildable";
    private void RemoveObject()
    {
        // needs some work as some meshes don't remove if they have multiple colliders attached
        RaycastResult result = raycastManager.FindRaycast(BUILDABLE_TAG);
        PrefabController.Remove(PooledType.Buildable, result.HitTransform, result.ObjectID);
    }

    #endregion

    #region Debugging Tools
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showDebug || !isBuilding || ghostManager.GhostObject == null)
        {
            return;
        }

        // Display Size Validator
        if (ghostManager.GhostRenderer != null)
        {
            BoxOverlapCheck.DrawOverlapBounds(ghostManager.GhostRenderer.localBounds, ghostManager.GhostRenderer.transform, Color.cyan);
        }

        if (ghostManager.CustomVolume != null)
        {
            BoxOverlapCheck.DrawOverapCustomBounds(ghostManager.CustomVolume, Color.cyan);
        }

        // Draw hit position
        if (isValidHit)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(rayPosition, 0.1f);
        }
    }

#endif
#endregion
}