using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RAK
{
    public static class CSharpExtender
    {
        /// <summary>
        /// you can iterate over any function with int parameter, like this:
        ///Action<int> f = FuncWithIntParam;
        ///f.Iter(0, 10);
        ///or,
        ///CSharpExtender.Iter(f,0, 10);
        ///or,
        ///((Action<int>) FuncWithIntParam).Iter(0, 10);
        /// </summary>
        /// <param name="f"></param>
        /// <param name="start_i"></param>
        /// <param name="end_i"></param>
        public static void Iter(this Action<int> f, int start_i, int end_i)
        {
            int inc = (start_i <= end_i) ? +1 : -1;
            Func<int, bool> cond = (int i) =>
            {
                if (inc > 0) return i <= end_i;
                else return i >= end_i;
            };
            for (int i = start_i; cond(i); i += inc)
            {
                f?.Invoke(i);
            }
        }
        /// <summary>
        /// you can call this version of Iter() like this:
        /// Func<int, IEnumerator> f = FuncWithIntParam;
        /// yield return f.Iter(0, 10);
        /// ExampleFunction:
        /// public IEnumerator FuncWithIntParam(int i)
        /// {
        ///     Debug.Log(i + "_" + Time.deltaTime); 
        ///     yield return new WaitForSeconds(1); //iterated once per second
        /// }
        /// </summary>
        /// <param name="f"></param>
        /// <param name="start_i"></param>
        /// <param name="end_i"></param>
        /// <returns></returns>
        public static IEnumerator Iter(this Func<int, IEnumerator> f, int start_i, int end_i)
        {
            int inc = (start_i <= end_i) ? +1 : -1;
            Func<int, bool> cond = (int i) =>
            {
                if (inc > 0) return i <= end_i;
                else return i >= end_i;
            };
            for (int i = start_i; cond(i); i += inc)
            {
                yield return f?.Invoke(i);
            }
        }
        public static T GetFirstImplementationOfType<T>()
        {
            T tinstance = default(T);
            MonoBehaviour[] monos = GameObject.FindObjectsOfType<MonoBehaviour>();
            for (int i = 0; i < monos.Length; i++)
            {
                tinstance = monos[i].GetComponent<T>();
                if (tinstance != null) break;
            }
            return tinstance;
        }
        public static void DoAfter(MonoBehaviour mono, System.Action act, float delay, System.Func<bool> ShouldWait)
        {
            if (mono == null)
            {
                //CM_Deb"mono is null");
                return;
            }

            if (mono.gameObject.activeSelf == false)
            {
                //CM_Deb"mono go is not active");
                return;
            }

            mono.StartCoroutine(Act(act, delay, ShouldWait));
        }
        private static IEnumerator Act(System.Action act, float delay, System.Func<bool> ShouldWait)
        {
            if (ShouldWait != null)
            {
                while (ShouldWait())
                {
                    yield return null;
                }
            }
            yield return new WaitForSeconds(delay);
            if (act != null) act();
        }

        static List<Component> trlist = new List<Component>();
        public static List<Component> GetAllOfType(Transform root, System.Type type, bool getRoot = false)
        {
            if (trlist == null)
                trlist = new List<Component>();
            else
                trlist.Clear();


            if (getRoot)
            {
                Component c = root.GetComponent(type);
                if (c != null) trlist.Add(c);
            }
            loadtransformList(root, type);

            return trlist;
        }
        private static void loadtransformList(Transform root, System.Type type)
        {
            foreach (Transform tr in root)
            {
                Component c = tr.GetComponent(type);
                if (c != null) trlist.Add(c);
                loadtransformList(tr, type);
            }
        }

        public static bool IsBetween(this int i, int start, int end, bool inclusive = true)
        {
            return inclusive ? (i>=start&&i<=end) : (i>start&&i<end);
        }

        public static double GetMedian(this List<int> numbers)
        {
            // Sort the list 
            numbers.Sort();
            // Get the count of numbers 
            int count = numbers.Count;
            // If count is odd, return the middle element 
            if (count % 2 == 1)
            {
                return numbers[count / 2];
            }
            else // If count is even, return the average of the two middle elements 
            {
                int middle1 = numbers[(count / 2) - 1];
                int middle2 = numbers[count / 2];
                return (middle1 + middle2) / 2.0;
            }

        }
    }
    public class BoolValent
    {   
        public static implicit operator bool(BoolValent obj)
        {
            return obj != null;
        }
    }

    public class BlockedBool 
    {
        public List<Func<bool>> blockers = new();

        public bool value 
        {
            get
            {
                foreach (Func<bool> b in blockers)
                {
                    if (b()) return false;
                }
                return true;
            }
        }

        public bool ValueWithoutSpecificBlocker(Func<bool> blocker)
        {
            foreach (Func<bool> b in blockers)
            {
                if (b == blocker) continue;
                if (b()) return false;
            }
            return true;
        }
    }
}