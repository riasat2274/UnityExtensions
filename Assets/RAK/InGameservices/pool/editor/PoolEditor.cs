using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
namespace RAK
{

    public class PoolEditor
    {

#if UNITY_EDITOR
        [MenuItem("GameObject/CreatePoolForThisItem/1")]
        public static void CreatePool_1()
        {
            CreatePoolFromSelection(1);
        }

        [MenuItem("GameObject/CreatePoolForThisItem/10")]
        public static void CreatePool_10()
        {
            CreatePoolFromSelection(10);
        }

        [MenuItem("GameObject/CreatePoolForThisItem/25")]
        public static void CreatePool_25()
        {
            CreatePoolFromSelection(25);
        }
        [MenuItem("GameObject/CreatePoolForThisItem/50")]
        public static void CreatePool_50()
        {
            CreatePoolFromSelection(50);
        }
        public static void CreatePoolFromSelection(int count)
        {
            CreatePool(Selection.activeGameObject,count);
        }
        public static ( List<GameObject>, GameObject) CreatePool(GameObject sample,int count)
        {
            if (sample == null)
            {
                Debug.LogError("You must select an item to create a Pool!");
                return (null,null);
            }
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            GameObject pkeep = GameObject.Find("PoolKeeper");
            if (!pkeep)
            {
                pkeep = new GameObject("PoolKeeper");
            }


            GameObject currentPool = GameObject.Find(string.Format("Pool : {0}", sample.name));
            if (!currentPool)
            {
                currentPool = new GameObject(string.Format("Pool : {0}", sample.name));
                currentPool.transform.SetParent(pkeep.transform);
            }
            List<GameObject> gos = new();
            for (int i = 0; i < count; i++)
            {
                PooledItem pi = GameObject.Instantiate(sample, currentPool.transform).AddComponent<PooledItem>();
                pi.gameObject.name = string.Format("{0}-{1}", sample.name, currentPool.transform.childCount);
                pi.alive = false;
                pi.original = sample;
                gos.Add(pi.gameObject);
            }
            return (gos,currentPool);
        }
#endif
    }
}