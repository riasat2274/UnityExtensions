using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif


[System.Serializable]
public class CSVDownloadData
{
    public string title;
    public string googleID;
    public string sheetID="0";
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CSVDownloadData))]
public class CSVDownloadDataEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUIUtility.labelWidth = 120;
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float elementHeight = (position.height - 20) / 3;
        // Calculate rects

        var titleLabelRect = new Rect(position.x, position.y, 150, elementHeight);
        var titleRect = new Rect(position.x+155, position.y, position.width-155, elementHeight);
        var googleIDLabelRect = new Rect(position.x, position.y + elementHeight + 5, position.width, elementHeight);
        var googleIDRect = new Rect(position.x + 155, position.y + elementHeight + 5, position.width-155, elementHeight);
        var sheetIDLabelRect = new Rect(position.x, position.y + elementHeight*2 + 10, position.width, elementHeight);
        var sheetIDRect = new Rect(position.x+155, position.y + elementHeight*2 + 10, position.width-155, elementHeight);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(titleRect, property.FindPropertyRelative("title"), GUIContent.none);
        EditorGUI.PropertyField(googleIDRect, property.FindPropertyRelative("googleID"), GUIContent.none);
        EditorGUI.PropertyField(sheetIDRect, property.FindPropertyRelative("sheetID"), GUIContent.none);
        EditorGUI.LabelField(titleLabelRect, "CSV Name");
        EditorGUI.LabelField(googleIDLabelRect,"Google ID");
        EditorGUI.LabelField(sheetIDLabelRect,"Sheet ID");


        // Set indent back to what it was
        EditorGUI.indentLevel = indent;


        EditorGUI.EndProperty();

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label)*3 + 20;
    }

}
#endif