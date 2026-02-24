using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RAK;

#if UNITY_EDITOR
using UnityEditor;
public class FastPropertyDrawer : PropertyDrawer
{
    protected string displayName;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        this.position = position;
        this.property = property;
        elementHeight = position.height;
        remainingRowSpace = position.width;
        yOffset = 0;

        this.displayName = property.displayName;

        EditorGUI.BeginProperty(position, label, property);
        OnGUICore();
        EditorGUI.EndProperty();
    }
    protected virtual void OnGUICore()
    {
        
    }
    Rect position;
    SerializedProperty property;
    float spacingX = 5;
    float elementHeight;
    float remainingRowSpace;
    float yOffset;

    protected void AddToRow(string id, float widthBudget = 1)
    {
        if (widthBudget <= 1)
        {
            widthBudget = widthBudget * remainingRowSpace;
        }
        Rect rect = new Rect(position.x + position.width - remainingRowSpace, position.y + yOffset, widthBudget, elementHeight);
        remainingRowSpace -= widthBudget + spacingX;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative(id), GUIContent.none);


    }
    //private void DrawProperty<T>(Rect rect, SerializedProperty property, GUIContent label)
    //{

    //    EditorGUI.PropertyField(rect, property.FindPropertyRelative(id), GUIContent.none);
    //}
    //public void AddSizedList<T>(string id, int listSize)
    //{
    //    SerializedProperty p = property.FindPropertyRelative(id);
    //    List<T> vals = p.objectReferenceValue as List<T>;
    //    vals.Resize(listSize);

    //    EditorGUI.PropertyField(rect, property.FindPropertyRelative(id), GUIContent.none);
    //}

    protected void AddLabel(string id, float widthBudget = 1)
    {
        if (widthBudget <= 1)
        {
            widthBudget = widthBudget * remainingRowSpace;
        }
        Rect rect = new Rect(position.x + position.width - remainingRowSpace, position.y + yOffset, widthBudget, elementHeight);
        remainingRowSpace -= widthBudget + spacingX;
        EditorGUI.LabelField(rect, id);
       //EditorGUI.PropertyField(rect, property.FindPropertyRelative(id), GUIContent.none);


    }
}
#endif