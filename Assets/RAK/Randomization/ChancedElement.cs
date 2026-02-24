using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{

    [System.Serializable]
    public class ChancedElement<T>
    {
        public float chance;
        public T element;
    }

    public static class ChanceListExtender
    {
        public static ChancedList<T> GetList<T>(this List<ChancedElement<T>> list)
        {
            var clist = new ChancedList<T>();
            foreach (var item in list)
            {
                clist.Add(item.element, item.chance);
            }
            return clist;
        }
    }
}
