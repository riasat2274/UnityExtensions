using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor.Search;
using UnityEditor;
#endif
namespace RAK
{
    public static class MonoBehaviourExtender
    {
        public static void Add_DelayedAct(this MonoBehaviour mono, Action act, float delay,bool useRealTime = false)
        {
            Centralizer.Add_DelayedMonoAct(mono,act,delay,useRealTime);
        }

        public static List<T> GetAll<T>(this Transform root, List<T> tlist= null)
        {
            if (tlist == null) tlist = new List<T>();
            T t = root.GetComponent<T>();
            Component c =  t as Component;
            if (c) tlist.Add(t);

            foreach (Transform newRoot in root)
            {
                GetAll<T>(newRoot, tlist);
            }

            return tlist;
        }


        public static T GetHigherComponent<T>(this MonoBehaviour mono)
        {
            return GetHigherComponent<T>(mono.transform);
        }
        public static T GetHigherComponent<T>(this Transform transform)
        {

            T t = transform.GetComponent<T>();
            Component c = t as Component;
            if (c) return t;
            else
            {
                if (transform.parent != null)
                    return GetHigherComponent<T>(transform.parent);
                else
                    return default(T);
            }
        }

        public static void SearchShader(this MonoBehaviour mono, string shader = "Universal Render Pipeline/Lit")
        {
            Renderer[] rends = GameObject.FindObjectsOfType<Renderer>();
            Dictionary<string, List<Renderer>> allShaderstoRends = new();
            Dictionary<string, List<Material>> allShadersToMats = new();


            foreach (Renderer r in rends)
            {
                foreach (Material m in r.materials)
                {
                    if (r.enabled == false) continue;
                    if (!allShaderstoRends.ContainsKey(m.shader.name))
                    {
                        allShaderstoRends.Add(m.shader.name, new List<Renderer>());
                        allShadersToMats.Add(m.shader.name, new List<Material>());
                    }
                    allShaderstoRends[m.shader.name].Add(r);
                    allShadersToMats[m.shader.name].Add(m);
                }

            }
            foreach (KeyValuePair<string, List<Renderer>> kvp in allShaderstoRends)
            {
                Debug.Log(kvp.Key);
                if (kvp.Key == shader)
                {
#if UNITY_EDITOR
                    foreach (Renderer r in kvp.Value)
                    {
                        Debug.Log(SearchUtils.GetHierarchyPath(r.gameObject));
                        AssetDatabase.GUIDToAssetPath(AssetDatabase.GetAssetPath(r.gameObject));
                    }
#endif
                }
            }
            //foreach (KeyValuePair<string, List<Material>> kvp in allShadersToMats)
            //{
            //    Debug.Log(kvp.Key);
            //    if (kvp.Key == "Universal Render Pipeline/Lit")
            //    {
            //        foreach (Material m in kvp.Value)
            //        {
            //            Debug.Log( SearchUtils.GetHierarchyPath( m));
            //            $"{AssetDatabase.GUIDToAssetPath(AssetDatabase.GetAssetPath(m))}".Debug(Color.cyan);
            //        }
            //    }
            //}
        }
    }

}
