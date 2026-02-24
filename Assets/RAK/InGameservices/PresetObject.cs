using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RAK;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RAK
{
    [System.Serializable]
    public class PrefabT<T> 
    {
        public T Reference => prefab;
        public void SetReference(T reference) => prefab = reference;
        [SerializeField] T prefab;
        public GameObject sourceObject 
        {
            get
            {
                if (prefab == null) return null;
                if(typeof(T)== typeof(GameObject)) return prefab as GameObject;
                else return (prefab as Component).gameObject;
            }
        } 
        public T Instantiate_T(Transform parent)
        {
            return Instantiate(parent).GetComponent<T>();
        }
        public T Instantiate_T(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Instantiate(position, rotation, parent).GetComponent<T>();
        }
        public GameObject Instantiate( Transform parent)
        {
            GameObject go = null;
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                go = GameObject.Instantiate(sourceObject);
            }
            else
            {
                go = UnityEditor.PrefabUtility.InstantiatePrefab(sourceObject) as GameObject;
            }
#else
            go = GameObject.Instantiate(sourceObject);
#endif

            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            return go;
        }
        public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent= null)
        {   
        #if UNITY_EDITOR
            GameObject go=null;
            if(Application.isPlaying)
            {
                go = GameObject.Instantiate(sourceObject, position, rotation);
            }
            else
            {
                go = UnityEditor.PrefabUtility.InstantiatePrefab(sourceObject) as GameObject;
                go.transform.position = position;
                go.transform.rotation = rotation;
            }
#else
           GameObject go = GameObject.Instantiate(sourceObject, position, rotation);
#endif

            go.transform.SetParent(parent);
            return go;
        }
        public GameObject InstantiateWithLifetime(Vector3 position, Quaternion rotation,float lifeTime, System.Action onDestroy, Transform parent = null)
        {
            GameObject go = Instantiate(position, rotation, parent);
            Centralizer.Add_DelayedAct(() => {
                if(go!=null) GameObject.Destroy(go);
                onDestroy?.Invoke();
            }, lifeTime);
            return go;
        }

        public T Pool_Instantiate_T(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Pool_Instantiate(position, rotation, parent).GetComponent<T>();
        }
        public GameObject Pool_Instantiate(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject go = Pool.Instantiate(sourceObject, position, rotation);
            if(parent!= go.transform.parent)go.transform.SetParent(parent);
            return go;
        }
        public GameObject Pool_InstantiateWithLifetime(Vector3 position, Quaternion rotation, float lifeTime, System.Action onDestroy, Transform parent = null)
        {
            GameObject go = Pool_Instantiate(position, rotation, parent);
            Centralizer.Add_DelayedAct(() => {
                if (go != null) Pool.Destroy(go);
                onDestroy?.Invoke();
            }, lifeTime);
            return go;
        }
    }
    
    [System.Serializable]
    public class PrefabObject : PrefabT<GameObject>
    {
    }

}
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PrefabObject))]
    public class TroopSetupInfoEditor : FastPropertyDrawer
    {
        protected override void OnGUICore()
        {
            base.OnGUICore();
            AddLabel(this.displayName, .3f);
            AddToRow("prefab");
        }
    }
#endif