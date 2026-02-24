using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{
    public class DepletionList<T>
    {
        private List<T> list;
        private List<T> depletingList;
        public int RemainingCount => depletingList.Count;
        public DepletionList(List<T> list)
        {
            this.list = list;
            this.depletingList = new(list);
        }
        public T Deplete()
        {
            if (depletingList.Count == 0)
            {
                return default(T);
            }
            int index = Random.Range(0, depletingList.Count);
            T item = depletingList[index];
            depletingList.RemoveAt(index);
            return item;
        }
        public List<T> Keep(int N)
        {

            List<T> items = new();
            while(depletingList.Count>N)
            {
                Deplete();
            }
            items.AddRange(depletingList);
            return items;
        }
        public List<T> Remaining()
        {
            return new List<T>(depletingList);
        }
    }
}
