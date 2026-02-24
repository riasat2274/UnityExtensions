using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{
    public static class ListExtender 
    {
        public static void Resize<T>(this List<T> list, int newSize)
        {
            if (list == null) list = new List<T>();
            int dif = newSize - list.Count;

            for (int i = 0; i < Mathf.Abs(dif); i++)
            {
                if (dif > 0)
                {
                    list.Add(default(T));
                }
                else
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
        }
        public static void Shuffle<T>(this List<T> list)
        {
            List<int> ilist = new List<int>();
            List<T> newList = new List<T>();

            for (int i = 0; i < list.Count; i++)
            {
                ilist.Add(i);
            }
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = ilist[Random.Range(0, ilist.Count)];
                ilist.Remove(randomIndex);
                newList.Add(list[randomIndex]);
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = newList[i];
            }
        }
        public static T SafeIndex<T>(this List<T> list, int i)
        {
            if (i >= 0 && i < list.Count)
            {
                return list[i];
            }
            else
            {
                Debug.LogError("Index out of bounds! Default is returned");
                return default(T);
            }
        }
        public static T ClampedIndex<T>(this List<T> list, int i)
        {
            if (list.Count == 0)
            {
                Debug.LogError("List is empty! Default is returned");
                return default(T);
            }

            if (i >= 0 && i < list.Count)
            {
                return list[i];
            }
            else if (i < 0)
            {
                return list[0];
            }
            else
            {
                return list[list.Count - 1];
            }
        }
        /// <summary>
        /// Execute specific action on every item in list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tlist"></param>
        /// <param name="action"></param>
        public static void ExecutePerElement<T>(this List<T> tlist, System.Action<T> action)
        {
            foreach (var item in tlist)
            {
                action?.Invoke(item);
            }
        }

        public static T GetMajority<T>(this List<T> tlist)
        {
            Dictionary<T, int> dict = new Dictionary<T, int>();
            foreach (var item in tlist)
            {
                if (dict.ContainsKey(item))
                {
                    dict[item]++;
                }
                else
                {
                    dict.Add(item, 1);
                }
            }
            T majority = default(T);
            int max = 0;
            foreach (var item in dict)
            {
                if (item.Value > max)
                {
                    majority = item.Key;
                    max = item.Value;
                }
            }
            return majority;
        }
    }
}
