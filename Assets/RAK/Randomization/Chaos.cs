using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{
    public static class Chaos
    {
        public static T DepleteRandomly<T>(this List<T> tlist)
        {
            if (tlist.Count == 0) return default(T);
            int index = UnityEngine.Random.Range(0, tlist.Count);
            T item = tlist[index];
            tlist.RemoveAt(index);
            return item;
        }
        public static float Deviate(this float baseValue, float deviationFraction)
        {
            return baseValue * (1 + Random.Range(-deviationFraction, deviationFraction));
        }
        public static bool Roll(this float target, float max = 1)
        {
            return Random.Range(0, max) < target;
        }
        public static T RandomItem<T>(this List<T> list)
        {
            if(list.Count==0) return default(T);
            return list[Random.Range(0, list.Count)];
        }
        public static List<T> TruncateRandomlyTo<T>(this List<T> tlist, int count)
        {
            if (tlist.Count <= count) return tlist;

            while (tlist.Count > count)
            {
                tlist.RemoveAt(Random.Range(0, tlist.Count));
            }

            return tlist;
        }

        public static float GaussianRandom(float stdDev)
        {
            float u1 = Random.value;
            float u2 = Random.value;
            float randomStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1, 10)) * Mathf.Sin(2 * Mathf.PI * u2);
            return randomStdNormal * stdDev;
        }

        public static float GaussianRandomInRange(float stdDev, float range)
        {
            if (range < stdDev)
            {
                Debug.LogError("Range should be bigger than the standard deviation!");
            }

            float f;
            do
            {
                f = GaussianRandom(stdDev);
            } while (Mathf.Abs(f) > range);

            return f;
        }
    }
}
