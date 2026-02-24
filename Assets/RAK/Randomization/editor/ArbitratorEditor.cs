using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{
#if UNITY_EDITOR
    public class ChanceKeeperCreator
    {
        [UnityEditor.MenuItem("Assets/Create/CreateChanceKeeper")]
        public static void Create()
        {
            ChanceKeeper so = ScriptableObject.CreateInstance<ChanceKeeper>();
            UnityEditor.AssetDatabase.CreateAsset(so, "Assets/ChanceKeeper.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = so;
        }
    }
#endif
}