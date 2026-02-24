using UnityEngine;
using System.Collections.Generic;
namespace RAK
{
    public static class Pool
    {
        /* AUTHOR: ARIFUR RAHMAN, LEAD GAME DEVELOPER (MINDFISHER GAMES)
        * DATE CREATED: 10 Nov, 2015
        * LAST UPDATED: 16 Jan, 2019
        */
        /// <QuickGuide>
        /// This class provide means to conventional pooling with convenience of pooling object on the fly
        /// Simplest way to use this class:
        ///
        /// Replacing:  "Instantiate()" with "Pool.Instantiate()" 
        /// Replacing: "Destroy()" with "Pool.Destroy()"
        /// 
        /// Warning: Even though the Pool.cs takes care of the pooling management, it does not return the object to a clean state as created for the first time
        /// Changing the pooled items in any way (except: position, rotation and parent) would persist while the object is reused automatically
        /// </QuickGuide>


        /// <Pre-PopulatingGuide>
        /// If pre-populating is necessary follow these steps
        /// 1. Create a copy of the object on scene
        /// 2. add PooledItem.cs to it
        /// 3. ensure the "alive" field is set to "false"
        /// 4. set "original" field with the reference of the prefab that it is a duplicate of 
        /// 5. duplicate this version as many times as you need and live in the scene somewhere
        /// 6. use instantiation and destroy call similar to using pooling on the fly.
        /// </Pre-PopulatingGuide>
        private static Dictionary<GameObject, List<GameObject>> poolDictionary = new Dictionary<GameObject, List<GameObject>>();
        private static Dictionary<GameObject, GameObject> prefabMap = new Dictionary<GameObject, GameObject>();
        
        public static int poolCount { get; private set; }

        public static Transform UnmanagedPooledObjectParent;


        /// <summary>
        /// Maintain A root transform for all unmanaged pooled objects
        /// </summary>
        private static void EnsurePoolMainHierarchyExists()
        {
            if (UnmanagedPooledObjectParent) return;
            GameObject poolkeeper_go = GameObject.Find("PoolKeeper");

            if (!poolkeeper_go)
            {
                UnmanagedPooledObjectParent = new GameObject("PoolKeeper").transform;
            }
            else
            {
                UnmanagedPooledObjectParent = poolkeeper_go.transform;
            }
        }

        /// <summary>
        /// Replaces: MonoBehaviour.Instantiate()
        /// </summary>
        /// <param name="original">original version to copy from</param>
        /// <param name="parent">parent object </param>
        /// <returns></returns>
        public static GameObject Instantiate(GameObject original, Transform parent = null)
        {
            return Instantiate(original, Vector3.zero, Quaternion.identity, parent);
        }
        public static GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (!poolDictionary.ContainsKey(original))
            {

                poolDictionary.Add(original, new List<GameObject>());
                poolCount++;
            }
            EnsurePoolMainHierarchyExists();
            List<GameObject> currentPool = poolDictionary[original];
            CleanPool(currentPool);
            if (parent == null)
            {
                parent = UnmanagedPooledObjectParent;
            }

            GameObject go;
            if (currentPool.Count == 0)
            {
                go = MonoBehaviour.Instantiate(original, position, rotation) as GameObject;


                PooledItem pitem = go.AddComponent<PooledItem>();
                pitem.original = original;
                pitem.alive = true;
                pitem.useCount++;
                prefabMap.Add(go, original);
            }
            else
            {
                go = currentPool[currentPool.Count - 1];
                currentPool.RemoveAt(currentPool.Count - 1);
                if(go ==null)Debug.LogError("isnull");
                PooledItem pitem = go.GetComponent<PooledItem>();
                if (pitem.alive) Debug.LogErrorFormat("Pool Error On - {0}", pitem.name);
                pitem.useCount++;
                pitem.alive = true;

                go.SetActive(true);
            }
            Transform goTrans = go.transform;
            if (goTrans.parent != parent)
            {
                goTrans.SetParent(parent);
            }
            goTrans.position = position;
            goTrans.rotation = rotation;

            return go;

        }

        //Checks if an object is being managed through Pool.cs
        public static bool IsPooled(GameObject gameObject)
        {
            return prefabMap.ContainsKey(gameObject);
        }


        /// <summary>
        /// Replaces: MonoBehaviour.Destroy()
        /// </summary>
        /// <param name="gameObject">object to destroy</param>
        /// <param name="shouldTrash">whether the script should remove destroyed object from current hierarchy (helpful if childcount is important)</param>
        public static void Destroy(GameObject gameObject, bool shouldTrash = false)
        {
            if (gameObject == null)
            {
                return;
            }
            else if (!prefabMap.ContainsKey(gameObject))
            {
                //Debug.Log("pre destroy".CCyan());
                MonoBehaviour.Destroy(gameObject);
                return;
            }


            GameObject original = prefabMap[gameObject];
            List<GameObject> currentPool = poolDictionary[original];
            PooledItem pitem = gameObject.GetComponent<PooledItem>();
            if (!pitem.alive) {
                Debug.LogWarningFormat("Tried to destroy an object twice - {0}", pitem.name);
                return;
            }
            //else { //CM_Deb"{0} pool item destroyed", pitem.name); }
            pitem.alive = false;
            gameObject.SetActive(false);
            currentPool.Add(gameObject);

            if (shouldTrash)
            {
                if (UnmanagedPooledObjectParent == null)
                    UnmanagedPooledObjectParent = (new GameObject("PoolManager")).transform;

                gameObject.transform.SetParent(UnmanagedPooledObjectParent);
            }
        }




        //clears unnecessary pools
        private static void CleanPool(List<GameObject> currentPool)
        {
            for (int i = currentPool.Count - 1; i >=0; i--)
            {
                if (currentPool[i] == null) currentPool.RemoveAt(i);
            }
        }

        #region Pre-Populating (Not mandatory)
        public static void RegisterToPool(PooledItem ppitem)
        {
            GameObject original = ppitem.original;

            if (!poolDictionary.ContainsKey(original))
            {
                poolDictionary.Add(original, new List<GameObject>());
                poolCount++;
            }

            List<GameObject> currentPool = poolDictionary[original];

            GameObject go;

            if (ppitem == null)
            {
                Debug.LogError("Pre pooled item is null");
                return;
            }
            go = ppitem.gameObject;
            //ppitem.prepopulationParent = ppitem.transform.parent;
            prefabMap.Add(go, original);
            currentPool.Add(go);
            go.SetActive(false);

        }
        #endregion
    }

}
