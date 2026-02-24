using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RAK
{
    [System.Serializable]
    public class ChancedList<T>
    {
        public List<T> items = new List<T>();
        public List<float> chances = new List<float>();
        public int Count { get { return items.Count; } }
        public bool IsEmpty { get { return items.Count == 0; } }

        private T dummy = default(T);
        public void Clear ()
        {
            items.Clear();
            chances.Clear();
        }

        public void Add(T item, float chance)
        {
            items.Add(item);
            chances.Add(chance);
        }
        int GetIndex(T item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(item,items[i]))
                    return i;
            }
            return -1;
        }

        public void ResetChanceFor(T item,float chance)
        {
            int index = GetIndex(item);
            if (index>=0) {
                 chances[index] = chance;
            } else {
                Debug.LogError ("Item not found");
            }
        }
        public float GetChanceFor(T item)
        {
            return chances[GetIndex(item)];// [item];
        }

        private T RollInternal(bool oneShot , float chanceReformMultiplier)
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
                    T retVal = items[i];
                    if (oneShot)
                    {
                        items.RemoveAt(i);
                        chances.RemoveAt(i);
                    }
                    else if (chanceReformMultiplier >= 0)
                    {
                        chances[i] *= chanceReformMultiplier;
                    }
                    return retVal;
                }
            }
            Debug.LogError("Fallen Through!!");

            return dummy;
        }

        /// <summary>
        /// Get a random item of type T
        /// </summary>
        /// <param name="chanceReformMultiplier">reduce or increase chance for the rolled item</param>
        /// <returns></returns>
        public T Roll(float chanceReformMultiplier = -1)
        {
            return RollInternal(oneShot: false, chanceReformMultiplier: chanceReformMultiplier);
        }
        /// <summary>
        /// Get a random item of type T . and removes the item from list
        /// </summary>
        /// <returns></returns>
        public T RollOneShot()
        {
            return RollInternal(oneShot: true, chanceReformMultiplier: -1);
        }
    }

    [System.Serializable]
    public class SpawnPriority
    {
        public GameObject item;
        public float probability;
        public int minPresence;
        public int maxPrseence;
        public int maxSpawn;
        internal int currentPresence;
        internal int totalSpawn;
        
        public bool IsPresenceBelowNeededStockLevel { get { return  currentPresence < minPresence; } }
        public bool IsOutOfStock { get { return maxSpawn > 0 && totalSpawn >= maxSpawn; } }
        public bool IsTooManyAlready { get { return maxPrseence>0 && currentPresence >= maxPrseence; } }



    }
    public static class Arbitrator
    {
        static ChancedList<SpawnPriority> spawnList = new ChancedList<SpawnPriority>();
        public static SpawnPriority GetSpawnSelection(this List<SpawnPriority> priorities)
        {
            SpawnPriority selection = null;
            spawnList.Clear();

            foreach (var spawnP in priorities)
            {
                if (spawnP.IsOutOfStock || spawnP.IsTooManyAlready) continue;

                if (spawnP.IsPresenceBelowNeededStockLevel)
                {
                    selection = spawnP;
                    break;
                }
                spawnList.Add(spawnP, spawnP.probability);

            }


            if (selection == null) selection = spawnList.Roll();

            selection.currentPresence++;
            selection.totalSpawn++;

            return selection;

        }
    }




    /*
     *     public class ChancedList<T>
    {
        Dictionary<T,float> inventory = new Dictionary<T, float>();
        private List<T> items = new List<T>();
        private List<float> chances = new List<float>();

        public T dummy;
        public void Clear ()
        {
            inventory.Clear ();
            items.Clear();
            chances.Clear();
        }

        public void Add(T item, float chance)
        {
            items.Add(item);
            chances.Add(chance);
            inventory.Add (item,chance);
        }
        bool GetIndex(T item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(item,items[i]))
                    return i;
            }
            return -1;
        }

        public void ResetChanceFor(T item,float chance)
        {
            if (inventory.ContainsKey (item)) {
                inventory [item] = chance;
            } else {
                Debug.LogError ("Item not found");
            }
        }
        public float GetChanceFor(T item)
        {
            return inventory [item];
        }

        public T Roll()
        {
            float totalWeight = 0;
            foreach (KeyValuePair<T,float> item in inventory) {
                totalWeight += item.Value;
            }
            float rollValue = UnityEngine.Random.Range (0.0f, totalWeight);
            float cumulativeWeight = 0;
            foreach (KeyValuePair<T,float> item in inventory) {
                cumulativeWeight += item.Value;
                //Debug.LogFormat ( "{0} <= {1} / {2}", rollValue, cumulativeWeight, totalWeight);
                if (rollValue <= cumulativeWeight) {
                    return item.Key;
                }
            }
            Debug.LogError ("Fallen Through!!");
            return dummy;
        }
    }
     */
    [System.Serializable]
    public class PseudoRandomArbitrator {
        public List<ProbabilityCase> data;
        public float totalOccurances;
        public float pseudoM;

        public PseudoRandomArbitrator(Dictionary<char,float> baseListing, float pseudoMultiplier)
        {
            totalOccurances = 0;
            pseudoM = pseudoMultiplier;
            data = new List<ProbabilityCase>();
            foreach (KeyValuePair<char,float> item in baseListing) {
                ProbabilityCase prC = new ProbabilityCase ();
                prC.id = item.Key;
                prC.baseProbability = item.Value;
                prC.occurances = 0;
                data.Add (prC);
            }
        }

        public char Arbitrate()
        {
            SetCurrentProbability ();
            float rollValue = UnityEngine.Random.Range (0.0f, 1f);
            float cumulativeWeight = 0;
            foreach (ProbabilityCase item in data) {
                cumulativeWeight += item.currentProbability;
                if (rollValue <= cumulativeWeight) {
                    item.occurances += 1;
                    totalOccurances += 1;
                    return item.id;
                }
            }
            Debug.LogError ("We are in trouble here!!");
            return '?';
        }


        void SetCurrentProbability()
        {   
            if (totalOccurances != 0) {
                float addedCurrentProbability = 0;
                for (int i = 0; i < data.Count; i++) {
                    float pDiff = (data [i].baseProbability - (data [i].occurances / totalOccurances)) * pseudoM;
                    data [i].currentProbability = Mathf.Clamp (data [i].baseProbability + pDiff, 0, 1);     
                    addedCurrentProbability += data [i].currentProbability;
                    //Debug.Log (data [i].currentProbability);
                }
                for (int i = 0; i < data.Count; i++) {
                    data [i].currentProbability /= addedCurrentProbability;
                    //Debug.Log (data [i].currentProbability);
                }
            } else {
                for (int i = 0; i < data.Count; i++) {
                    data [i].currentProbability = data[i].baseProbability;
                }
            }

        }

        public class ProbabilityCase
        {
            public char id;
            public float baseProbability;
            public float currentProbability;
            public float occurances;
        }
    }

}
