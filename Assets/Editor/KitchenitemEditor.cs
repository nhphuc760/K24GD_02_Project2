using NUnit.Framework.Internal.Execution;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(KitchenItemDataSO))]
public class KitchenitemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Tìm field kế thừa
        var durability = serializedObject.FindProperty("_description");

        // Hiện nhưng khóa
        GUI.enabled = false;
        EditorGUILayout.PropertyField(durability); // VẪN HIỂN THỊ
        GUI.enabled = true;

        // Vẽ phần còn lại
        DrawPropertiesExcluding(serializedObject, "_description");

        serializedObject.ApplyModifiedProperties();
    }
}
