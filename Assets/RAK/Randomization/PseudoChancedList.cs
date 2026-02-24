using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace RAK
{
    [System.Serializable]
    public class PseudoChancedList<T>
    {
        int pseudoSize = 20;
        public List<int> pseudoList = new List<int>();
        float tightnessFactor = 1;
        float pseudoStrength
        {
            get
            {
                if (tightnessFactor < 0) return 0;
                return (1 - (pseudoSize - pseudoList.Count) / ((float)pseudoSize)) * tightnessFactor;

            }
        }
        float clusterStrength
        {
            get
            {
                if (tightnessFactor > 0) return 0;
                else return -tightnessFactor;
            }
        }

        public List<T> items = new List<T>();
        public List<float> chances = new List<float>();
        public List<float> chancesOriginal = new List<float>();
        int count;

        private T dummy = default(T);
        public void Refresh(int size, float strength)
        {
            count = 0;
            items.Clear();
            chances.Clear();
            chancesOriginal.Clear();
            pseudoList.Clear();

            pseudoSize = size;
            tightnessFactor = strength;
        }

        public void Add(T item, float chance)
        {
            count++;
            items.Add(item);
            chances.Add(chance);
            chancesOriginal.Add(chance);
        }
        public void Normalize()
        {
            float TotalChances = 0;
            for (int i = 0; i < chancesOriginal.Count; i++)
            {
                TotalChances += chancesOriginal[i];
            }
            for (int i = 0; i < chancesOriginal.Count; i++)
            {
                chances[i] = chances[i] / TotalChances;
                chancesOriginal[i] = chancesOriginal[i] / TotalChances;
                ////CM_Debchances[i]);
                ////CM_DebchancesOriginal[i]);
            }
        }

        int GetIndex(T item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(item, items[i]))
                    return i;
            }
            return -1;
        }

        //public void ResetChanceFor(T item, float chance)
        //{
        //    int index = GetIndex(item);
        //    if (index >= 0)
        //    {
        //        chances[index] = chance;
        //    }
        //    else
        //    {
        //        Debug.LogError("Item not found");
        //    }
        //}
        //public float GetChanceFor(T item)
        //{
        //    return chances[GetIndex(item)];// [item];
        //}

        public T Roll()
        {
            float totalWeight = 0;
            for (int i = 0; i < items.Count; i++)
            {
                totalWeight += chances[i];
            }
            float rollValue = UnityEngine.Random.Range(0.0f, totalWeight);
            float cumulativeWeight = 0;
            for (int i = 0; i < items.Count; i++)
            {
                cumulativeWeight += chances[i];
                //Debug.LogFormat ( "{0} <= {1} / {2}", rollValue, cumulativeWeight, totalWeight);
                if (rollValue <= cumulativeWeight)
                {
                    ModifyChancesOnRoll(i);
                    return items[i];
                }
            }
            Debug.LogError("Fallen Through!!");
            return dummy;
        }
        private void ModifyChancesOnRoll(int last_i)
        {
            if (pseudoList.Count >= pseudoSize)
            {
                pseudoList.RemoveAt(0);
            }
            pseudoList.Add(last_i);

            float[] pseudoRatio = new float[items.Count];
            for (int i = 0; i < pseudoList.Count; i++)
            {
                pseudoRatio[pseudoList[i]] += (1.0f / pseudoList.Count);
            }

            for (int i = 0; i < chances.Count; i++)
            {
                float v = chancesOriginal[i] + pseudoStrength*(chancesOriginal[i] - pseudoRatio[i]);
                chances[i] = Mathf.Clamp01(v);
                ////CM_Deb"Or: {0}, PR {1}, PS{2}",chances[i],pseudoRatio[i], pseudoStrength);
            }
            chances[last_i] += clusterStrength; 
        }
    }


    /*
     *public class Test : MonoBehaviour {
    PseudoChancedList<string> clist = new PseudoChancedList<string>();
	// Use this for initialization
	void Start () {
        clist.Refresh(10, 10);
        clist.Add("Alpha", 5);
        clist.Add("Beta", 3);
        clist.Add("Gama", 2);
        clist.Normalize();

        for (int i = 0; i < 10; i++)
        {
            clist.Roll();
        }

        for (int i = 0; i < 100; i++)
        {
            //CM_Debclist.Roll());
            int a=0;
            int b=0;
            int c=0;
            for (int j = 0; j < clist.pseudoList.Count; j++)
            {
                if (clist.pseudoList[j] == 0) a++;
                if (clist.pseudoList[j] == 1) b++;
                if (clist.pseudoList[j] == 2) c++;
            }
            //CM_Deb"{0}, {1}, {2}", a, b, c);
        }
        ////CM_Deb"Alpha {0}", clist.chances[0]);
        ////CM_Deb"Beta {0}", clist.chances[1]);
        ////CM_Deb"Gama {0}", clist.chances[2]);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
} 
     */
}