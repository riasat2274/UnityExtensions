using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RAK;

#if UNITY_EDITOR
using UnityEditor;
public class LinePropertyDrawer : PropertyDrawer
{
    protected string displayName;
    public int ListIndex
    {
        get
        {
            bool isListElement = Property.propertyPath.Contains("Array.data[");

            if (isListElement)
            {
                // Extract the list index
                string propertyPath = Property.propertyPath;
                int startIndex = propertyPath.IndexOf("Array.data[") + 11;
                int endIndex = propertyPath.IndexOf("]", startIndex);
                int listIndex = int.Parse(propertyPath.Substring(startIndex, endIndex - startIndex));

                return listIndex;
            }
            else
            {
                return -1;
            }
        }
    }

    public Rect availablePosition { get; private set; }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        this.position = position;
        this.Property = property;
        elementHeight = (position.height/RowCount)-spacingY;
        consumedRowSpace = new float[AdditionalRowCount + 1];
        yOffset = 0;

        this.displayName = property.displayName;
        propertyPosDatas.Clear();
        rowIndex = 0;


        if (IsExpandible)
        {
            Rect expRect = new Rect(position.x - 15, position.y, position.width, elementHeight);
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(expRect, property.isExpanded, label);

            if (property.isExpanded)
            {
                availablePosition = new Rect(position.x, position.y + elementHeight, position.width, position.height - elementHeight);
                EditorGUI.BeginProperty(availablePosition, label, property);
                EditorGUI.indentLevel++;
                OnGUICore();
                DrawByPosData();
                EditorGUI.indentLevel--;
                EditorGUI.EndProperty();
            }
            EditorGUI.EndFoldoutHeaderGroup();
        }
        else 
        {
            property.isExpanded = true;
            availablePosition = position;
            EditorGUI.BeginProperty(position, label, property);
            OnGUICore();
            DrawByPosData();
            EditorGUI.EndProperty();
        }

    }
    protected virtual void OnGUICore()
    {
        
    }
    Rect position;
    protected SerializedProperty Property { get; private set; }
    protected int rowIndex;
    protected virtual float spacingX => 5;
    protected virtual float spacingY => 5;
    float elementHeight;
    float[] consumedRowSpace;
    float yOffset;
    int RowCount => (IsExpandible ? 1:0) + (Property.isExpanded?AdditionalRowCount:0);
    public virtual int AdditionalRowCount => 1;
    public virtual bool IsExpandible => AdditionalRowCount > 1;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Property = property;
        float baseHeight = base.GetPropertyHeight(property, label);

        if (IsExpandible)
        {
            return (baseHeight + spacingY) * (1 + (property.isExpanded ? AdditionalRowCount : 0));
        }
        else
        {
            return (baseHeight + spacingY) * AdditionalRowCount;
        }
    }
    class PropertyPosData
    {
        public string id_label;
        public int rowIndex;
        public int rowCount;
        public float posXPix;
        public float widthPix;
        public PropertyType type;
        public Vector2 minMax;
    }
    
    public enum PropertyType 
    {
        Field,
        Label,
        Space,
        Texture,
        Slider,
    }
    void DrawByPosData()
    {
        int indexOffset = IsExpandible ? 1 : 0;  
        foreach (var item in propertyPosDatas)
        {
          
            Rect rect = new Rect(position.x + item.posXPix, position.y + (item.rowIndex+ indexOffset) * (elementHeight+spacingY)*item.rowCount, item.widthPix, elementHeight);
            //EditorGUI.PropertyField(rect, property.FindPropertyRelative(item.id_label), GUIContent.none);
            switch (item.type)
            {
                case PropertyType.Label:
                    EditorGUI.LabelField(rect, item.id_label);
                    break;
                case PropertyType.Space:
                    break;
                case PropertyType.Field:

                    EditorGUI.BeginProperty(rect,GUIContent.none, Property.FindPropertyRelative(item.id_label));
                    EditorGUI.PropertyField(rect, Property.FindPropertyRelative(item.id_label), GUIContent.none);

                    EditorGUI.EndProperty();
                    break;
                case PropertyType.Slider:
                    EditorGUI.BeginProperty(rect, GUIContent.none, Property.FindPropertyRelative(item.id_label));
                    EditorGUI.Slider(rect, Property.FindPropertyRelative(item.id_label), item.minMax.x, item.minMax.y, GUIContent.none);
                    EditorGUI.EndProperty();
                    break;
                case PropertyType.Texture:

                    EditorGUI.BeginProperty(rect, GUIContent.none, Property.FindPropertyRelative(item.id_label));
                    //EditorGUI.PropertyField(rect, Property.FindPropertyRelative(item.id_label), GUIContent.none);
                    //GUIContent gc = new GUIContent(item.id_label);
                    //rect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), gc);

                    // Draw texture field
                    SerializedProperty sp = Property.FindPropertyRelative(item.id_label);
                    sp.objectReferenceValue = EditorGUI.ObjectField(rect, sp.objectReferenceValue, typeof(Texture2D),false);
                    EditorGUI.EndProperty();
                    break;
                default:
                    break;
            }
        }
    }


    [System.NonSerialized] List<PropertyPosData> propertyPosDatas=new();
    public void AddVerticalField(string id, float widthBudget, int startRow=-1, string modifiedTitle = null, float spaceWidth=0)
    {
        if (startRow == -1)
        {
            startRow = rowIndex;
        }
        if (modifiedTitle == null)
        {
            modifiedTitle = id;
        }
        rowIndex = startRow;
        Add(modifiedTitle, PropertyType.Label, widthBudget,startRow);
        Add(id, PropertyType.Field, widthBudget, startRow + 1);
        if (spaceWidth > 0) 
        {
            Add("", PropertyType.Space, spaceWidth, startRow);
            Add("", PropertyType.Space, spaceWidth, startRow + 1);

        }
    }
    public void AddHorizontalField(string id, float widthBudget, int startRow = -1, string modifiedTitle = null, float labelSizeRatio = .5f)
    {
        if (startRow == -1)
        {
            startRow = rowIndex;
        }
        if (modifiedTitle == null)
        {
            modifiedTitle = id.ConvertCamelCase();
        }
        rowIndex = startRow;
        Add(modifiedTitle, PropertyType.Label, widthBudget* labelSizeRatio, startRow);
        Add(id, PropertyType.Field, widthBudget*(1- labelSizeRatio), startRow);
    }

    public void AddTexture(string id, float widthBudget, int rowbudget, int startRow = -1)
    { 
        if (startRow == -1)
        {
            startRow = rowIndex;
        }


        rowIndex = startRow;

        if (widthBudget <= 1)
        {
            widthBudget = widthBudget * position.width;
        }
        PropertyPosData ppd = new PropertyPosData()
        {
            id_label = id,
            rowIndex = rowIndex,
            rowCount = rowbudget,
            posXPix = consumedRowSpace[rowIndex],
            widthPix = widthBudget - spacingX,
            type = PropertyType.Texture,
        };
        propertyPosDatas.Add(ppd);
        for (int i = 0; i < rowbudget; i++)
        {
            consumedRowSpace[rowIndex + i] += widthBudget;
        }
    }

    private PropertyPosData Add(string id, PropertyType type, float widthBudget , int rowIndex=-1)
    {
        if (rowIndex == -1)
        {
            rowIndex = this.rowIndex;
        }

        if (widthBudget <= 1)
        {
            widthBudget = widthBudget * position.width;
        }
        PropertyPosData ppd = new PropertyPosData()
        {
            id_label = id,
            rowIndex = rowIndex,
            rowCount = 1,
            posXPix = consumedRowSpace[rowIndex],
            widthPix = widthBudget - spacingX,
            type = type
        };
        propertyPosDatas.Add(ppd);

        consumedRowSpace[rowIndex] += widthBudget;
        return ppd;
    }

    protected void AddSlider(string id,float min, float max, float widthBudget = 1)
    {
        Add(id, PropertyType.Slider, widthBudget).minMax = new Vector2(min,max);
    }
    protected void AddField(string id, float widthBudget = 1)
    {
        Add(id, PropertyType.Field, widthBudget);
    }
    protected void AddLabel(string id,  float widthBudget = 1)
    {
        Add(id, PropertyType.Label, widthBudget);
    }
    protected void AddSpace(float widthBudget)
    {
        Add("", PropertyType.Space, widthBudget);
    }
}
#endif