using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StockPrefabFactory : EditorWindow
{
    // Current stock item prefab
    private GameObject stockItemPrefab;

    // Current Shelf Prefab
    private GameObject shelfPrefab;
    private bool showShelf;

    // ShelfRow from Shelf Prefab
    private ShelfRow shelfRow;
    private Transform stockHoldPoint;
    
    // SizeProfile of Shelf Prefab
    private Bounds shelfBounds;
    private ShelfSizeProfile shelfProfile;

    // Preview new prefab
    private GameObject previewShelf;
    private GameObject previewRoot;

    // Prefab Spacing
    private float padding = 0.02f;
    private float fillAmount = 1f;

    // Live Preview
    private bool livePreview = true;
    private GameObject lastShelfPrefab;
    private GameObject lastStockPrefab;
    private float lastPadding;
    private float lastFillAmount;

    [MenuItem("Tools/Stock Prefab Factory")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<StockPrefabFactory>("Stock Prefab Factory");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Stock Prefab Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Shelf prefab field
        shelfPrefab = (GameObject)EditorGUILayout.ObjectField("Target Shelf Prefab", shelfPrefab, typeof(GameObject), false);
        showShelf = EditorGUILayout.Toggle("Preview Shelf Fit", showShelf);
        GUILayout.Space(10);

        // Stock item prefab field
        stockItemPrefab = (GameObject)EditorGUILayout.ObjectField("Stock Item Prefab", stockItemPrefab, typeof(GameObject), false);

        // Stock item prefab spacing (low = less padding, high = more padding)
        GUILayout.Space(10);
        fillAmount = EditorGUILayout.Slider("Fill Amount", fillAmount, 0.1f, 1f);
        livePreview = EditorGUILayout.Toggle("Live Preview", livePreview);

        // Analyse Shelf Section
        if (shelfPrefab != null && stockItemPrefab != null)
        {
            if (GUILayout.Button("Analyse Shelf"))
            {
                AnalyseShelf();
            }
        }

        // Preview Section
        if (shelfPrefab != null && stockItemPrefab != null)
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Preview Layout"))
            {
                PreviewLayout();
            }

            if (previewRoot != null && GUILayout.Button("Clear Preview"))
            {
                DestroyImmediate(previewRoot);
                if (previewShelf != null)
                {
                    DestroyImmediate(previewShelf);
                }
            }
        }

        if (livePreview && shelfPrefab != null && stockItemPrefab != null)
        {
            if (shelfPrefab != lastShelfPrefab ||
                stockItemPrefab != lastStockPrefab ||
                !Mathf.Approximately(padding, lastPadding) ||
                !Mathf.Approximately(fillAmount, lastFillAmount))
            {
                // Values have changed — re-preview
                PreviewLayout();

                // Store new values
                lastShelfPrefab = shelfPrefab;
                lastStockPrefab = stockItemPrefab;
                lastPadding = padding;
                lastFillAmount = fillAmount;
            }
        }

        if (previewRoot != null && GUILayout.Button("Save Layout Prefab"))
        {
            SaveLayoutPrefab();
        }
    }

    // Make to define bound area calculation
    private void AnalyseShelf()
    {
        // Instantiate temporarily to access components
        GameObject temp_shelf = (GameObject)PrefabUtility.InstantiatePrefab(shelfPrefab);
        shelfRow = temp_shelf.GetComponentInChildren<ShelfRow>();

        if (shelfRow == null)
        {
            Debug.LogWarning("ShelfRow or BoxCollider not found in shelf prefab.");
            DestroyImmediate(temp_shelf);
            return;
        }

        // STEP 1: Get collider bounds
        shelfProfile = shelfRow.ShelfProfile;
        Vector3 shelf_size = shelfProfile.BoundSize;

        // STEP 2: Get stock item size
        GameObject temp_item = Instantiate(stockItemPrefab);
        MeshRenderer item_renderer = temp_item.GetComponentInChildren<MeshRenderer>();

        if (item_renderer == null)
        {
            Debug.LogWarning("No MeshRenderer found in stock item prefab.");
            DestroyImmediate(temp_item);
            DestroyImmediate(temp_shelf);
            return;
        }

        Bounds itemBounds = item_renderer.bounds;
        Vector3 item_size = itemBounds.size;

        // Optional padding
        item_size += Vector3.one * padding;

        // STEP 3: Calculate fit
        int x_count = Mathf.FloorToInt(shelf_size.x / item_size.x);
        int y_count = Mathf.FloorToInt(shelf_size.y / item_size.y);
        int z_count = Mathf.FloorToInt(shelf_size.z / item_size.z);

        // Log Analytics
        Debug.Log($"Shelf ({shelfProfile.Size}) Bound Area: {shelf_size} | Prefab Item Mesh Area: {item_size}");
        Debug.Log($"Prefab Fit Count: X = {x_count}, Y = {y_count}, Z = {z_count}");

        // Clean up
        DestroyImmediate(temp_item);
        DestroyImmediate(temp_shelf);
    }

    // Make to preview layout of generated shelf item prefab
    private void PreviewLayout()
    {
        // Clear old previews
        if (previewRoot != null)
        {
            DestroyImmediate(previewRoot);
        }
        if (previewShelf != null)
        {
            DestroyImmediate(previewShelf);
        }

        // Get shelf variables
        GameObject temp_shelf = (GameObject)PrefabUtility.InstantiatePrefab(shelfPrefab);
        if (showShelf)
        {
            previewShelf = temp_shelf;
        }

        shelfRow = temp_shelf.GetComponentInChildren<ShelfRow>();
        if (shelfRow == null)
        {
            Debug.LogWarning("Shelf Row is null");
            return;
        }

        shelfProfile = shelfRow.ShelfProfile;
        stockHoldPoint = shelfRow.StockHoldPoint;

        // Prefab null check
        if (shelfBounds == null)
        {
            Debug.LogWarning("BoxCollider not found in shelf prefab.");
            DestroyImmediate(temp_shelf);
            return;
        }

        // Get row collider bounds
        Vector3 shelf_area = shelfProfile.BoundSize;

        // Create parent prefab
        GameObject temp_item = Instantiate(stockItemPrefab);
        MeshRenderer item_renderer = temp_item.GetComponentInChildren<MeshRenderer>();
        if (item_renderer == null)
        {
            Debug.LogWarning("No MeshRenderer found in stock item prefab.");
            DestroyImmediate(temp_item);
            DestroyImmediate(temp_shelf);
            return;
        }

        // Define item size within bounds
        Vector3 item_area = item_renderer.bounds.size + Vector3.one * padding;
        
        // Define Root Position and Rotation
        Vector3 origin = stockHoldPoint.position;
        Quaternion rotation = stockHoldPoint.rotation;

        // Get count of prefab we will create (bound size / item size)
        int max_x = Mathf.FloorToInt(shelf_area.x / item_area.x);
        int max_y = Mathf.FloorToInt(shelf_area.y / item_area.y);
        int max_z = Mathf.FloorToInt(shelf_area.z / item_area.z);

        int x_count = Mathf.Max(1, Mathf.FloorToInt(max_x * fillAmount));
        int y_count = Mathf.Max(1, Mathf.FloorToInt(max_y * fillAmount));
        int z_count = Mathf.Max(1, Mathf.FloorToInt(max_z * fillAmount));

        // Create item object origin
        previewRoot = new GameObject("StockPreviewLayout");
        previewRoot.transform.position = origin;
        previewRoot.hideFlags = HideFlags.DontSave;

        float cell_width = shelf_area.x / x_count;
        float cell_depth = shelf_area.z / z_count;

        for (int y = 0; y < y_count; y++)
        {
            for (int z = 0; z < z_count; z++)
            {
                for (int x = 0; x < x_count; x++)
                {
                    float posX = (x + 0.5f) * cell_width;
                    float posZ = (z + 0.5f) * cell_depth;

                    Vector3 offset =
                        stockHoldPoint.right * posX +
                        stockHoldPoint.up * (y * item_area.y) +
                        -stockHoldPoint.forward * posZ;

                    GameObject dummy = Instantiate(temp_item);
                    dummy.transform.position = origin + offset;
                    dummy.transform.rotation = rotation * Quaternion.Euler(stockItemPrefab.transform.localEulerAngles);
                    dummy.transform.SetParent(previewRoot.transform);
                    dummy.hideFlags = HideFlags.None;
                    dummy.name = stockItemPrefab.name;

                    foreach (var c in dummy.GetComponentsInChildren<Collider>())
                        DestroyImmediate(c);
                    foreach (var s in dummy.GetComponentsInChildren<MonoBehaviour>())
                        DestroyImmediate(s);
                }
            }
        }


        DestroyImmediate(temp_item);
        if (!showShelf)
            DestroyImmediate(temp_shelf);
    }

    private void SaveLayoutPrefab()
    {
        if (previewRoot == null)
        {
            Debug.LogWarning("No preview layout to save.");
            return;
        }

        // Clone the preview into the scene to make it savable
        GameObject clone = Instantiate(previewRoot);
        clone.name = previewRoot.name;
        clone.hideFlags = HideFlags.None;
        clone.transform.position = Vector3.zero;
        clone.transform.rotation = Quaternion.identity;

        // Remove editor-only components if needed
        foreach (var c in clone.GetComponentsInChildren<MonoBehaviour>())
        {
            if (c.GetType().Namespace == "UnityEditor" || c.GetType().Name.Contains("Editor"))
            {
                DestroyImmediate(c);
            }
        }

        // Save path and name
        string stockName = stockItemPrefab != null ? stockItemPrefab.name : "Unnamed";
        string shelfSize = shelfProfile != null ? shelfProfile.Size.ToString() : "Unknown";
        string fileName = $"{stockName}_Layout_{shelfSize}.prefab";

        string folderPath = $"Assets/StockLayouts";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "StockLayouts");
        }

        string fullPath = $"{folderPath}/{fileName}";

        // Save as prefab
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(clone, fullPath);
        if (savedPrefab != null)
        {
            Debug.Log($"Saved layout prefab: {fullPath}");
            EditorGUIUtility.PingObject(savedPrefab);
        }
        else
        {
            Debug.LogWarning("Failed to save layout prefab.");
        }

        // Cleanup
        DestroyImmediate(clone);
    }
}