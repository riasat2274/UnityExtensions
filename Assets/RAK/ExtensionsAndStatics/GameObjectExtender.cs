using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RAK
{
    public static class GameObjectExtender
    {
        public static void ActivateAll(this List<GameObject> gameObjects,bool enable = true)
        {
            foreach (var item in gameObjects)
            {
                item.SetActive(enable);
            }
        }
        public static void DeactivateAll(this List<GameObject> gameObjects)
        {
            foreach (var item in gameObjects)
            {
                item.SetActive(false);
            }

        }
        public static void ActiveByCount(this List<GameObject> gameObjects, int value)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].SetActive(i<value);
            }
        }
        public static void ActiveByCount(this List<Image> images, int value, Sprite active, Sprite inactive)
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i].sprite = i < value ? active : inactive;
            }
        }

        public static GameObject InstantiateAt(this GameObject prefab, Transform root, bool reset = false)
        {
            GameObject go = GameObject.Instantiate(prefab);
            go.transform.SetParent(root);
            if (reset)
            {
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }
            return go;
        }
        public static T InstantiateAt<T>(this GameObject prefab, Transform root, bool reset = false)
        {
            GameObject go = prefab.InstantiateAt(root,reset);
            return go.GetComponent<T>();
        }
        public static void DestroyAttachedGameObjects<T>(this List<T> components)
        {
            for (int i = components.Count-1; i>=0; i--)
            {
                Component C = components[i] as Component;
                if (C != null)
                {
                    GameObject.Destroy(C.gameObject);
                }
                else
                {
                    GameObject.Destroy(components[i] as Object);
                }
            }
            components.Clear();

        }
        public static void SetLayer(this GameObject parent, string layerName, bool includeChildren = true)
        {
            int layer = LayerMask.NameToLayer(layerName);
            //Debug.Log(layer);
            parent.layer = layer;
            if (includeChildren)
            {
                foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
                {
                    trans.gameObject.layer = layer;
                }
            }
        }
        public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
        {
            parent.layer = layer;
            if (includeChildren)
            {
                foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
                {
                    trans.gameObject.layer = layer;
                }
            }
        }
    }
}