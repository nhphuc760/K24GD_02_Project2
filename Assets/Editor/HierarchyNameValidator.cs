using UnityEditor;
using UnityEngine;

// Editor utility: when hierarchy changes, ensure no GameObject has an empty name.
// If an empty name is found, it's replaced with "GameObject" and a warning is logged.
[InitializeOnLoad]
public static class HierarchyNameValidator
{
    static HierarchyNameValidator()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private static void OnHierarchyChanged()
    {
        // Find all root objects in all open scenes
        var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        bool fixedAny = false;
        foreach (var go in allObjects)
        {
            if (go == null) continue;
            if (string.IsNullOrEmpty(go.name))
            {
                Undo.RecordObject(go, "Fix empty GameObject name");
                go.name = "GameObject";
                Debug.LogWarning($"HierarchyNameValidator: Found empty GameObject name and renamed to 'GameObject' (instance id {go.GetInstanceID()}).");
                fixedAny = true;
            }
        }

        if (fixedAny)
        {
            // Force the hierarchy to repaint so user sees changes
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
