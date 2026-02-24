using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif



#if UNITY_EDITOR
[CustomEditor(typeof(CSVDownloadBox))]
public class CSVDownloadBoxEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        EditorGUI.EndDisabledGroup();
        CSVDownloadBox box = (CSVDownloadBox)Selection.activeObject;
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("baseDirectory"));
        Show(serializedObject.FindProperty("dataList"),box);
        serializedObject.ApplyModifiedProperties();

    }


    public static void Show(SerializedProperty list, CSVDownloadBox contextBox)
    {
        EditorGUILayout.Space();
        EditorGUI.indentLevel += 1;
        for (int i = 0; i < list.arraySize; i++)
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Download"))
            {
                contextBox.Download(i,(TextAsset ta)=> { Debug.Log(ta.text); });
            }
            if (GUILayout.Button("(-)Remove"))
            {
                contextBox.dataList.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

        }
        EditorGUI.indentLevel -= 1;
        EditorGUILayout.Space();
        if (GUILayout.Button("(+)Add"))
        {
            contextBox.dataList.Add(new CSVDownloadData());
        }
    }
}
#endif