using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{
    public class Centralizer : MonoBehaviour
    {
        #region singleton management
        public static Centralizer Instance
        {
            get
            {
                if (instance == null) Init();
                return instance;
            }
        }
        static Centralizer instance;
        public static Centralizer Init()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Centralization Manager");
                instance = go.AddComponent<Centralizer>();
            }
            return instance;
        }
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        #region updatable action
        private class UpdateableAction
        {
            private GameObject _go;

            public GameObject go
            {
                get
                {
                    if (_go == null) _go = mono.gameObject;
                    return _go;
                }
            }
            public MonoBehaviour mono;
            public Action act;
            public float timeGap;
            public float lastTime;

            public UpdateableAction(MonoBehaviour m, Action a, float tg, float ig)
            {
                mono = m;
                act = a;
                timeGap = tg;
                if (ig <= 0)
                {
                    lastTime = Time.time;
                    a();
                }
                else
                {
                    lastTime = Time.time - tg + ig;
                }

            }
        }


        public int actCount = 0;
        private List<long> updateableKey = new List<long>();
        private List<UpdateableAction> updateableActions = new List<UpdateableAction>();
        //private Dictionary<int, UpdateableAction> updateableActionDictionary = new Dictionary<int, UpdateableAction>();
        public static int Add_Update(MonoBehaviour mono, System.Action act, float timeGap = 0, float initialGap =0)
        {
            if (mono == null)
            {
                Debug.LogError("Tried to add null mono behaviour on unified update manager!");
                return -1;
            }
            if (act == null)
            {
                Debug.LogError("Tried to add null action on unified update manager!");
                return -1;
            }

            Init();
            instance.updateableActions.Add(new UpdateableAction(mono, act, timeGap, initialGap));
            instance.updateableKey.Add(instance.actCount);
            //instance.updateableActionDictionary.Add(instance.actCount, new UpdateableAction(mono, act, timeGap, initialGap));
            instance.actCount++;
            return instance.actCount - 1;

        }
        public static bool Remove_Update(long key)
        {
            if (instance.updateableKey.Contains(key))
            {
                int indexOf = instance.updateableKey.IndexOf(key);
                instance.updateableKey.RemoveAt(indexOf);
                instance.updateableActions.RemoveAt(indexOf);
                return true;
            }
            //if (instance.updateableActionDictionary.ContainsKey(key))
            //{
            //    instance.updateableActionDictionary.Remove(key);
            //    return true;
            //}
            return false;
        }


        List<long> trashList = new List<long>();
        void Update_UpdatableActions()
        {
            ////CM_DebupdateableActionDictionary.Count);
            if (trashList.Count > 0) trashList.Clear();
            for (int i = updateableActions.Count - 1; i >=0 ; i--)
            {
                UpdateableAction ua = updateableActions[i];
                if (ua.mono != null)
                {
                    if (ua.mono.enabled && ua.go.activeInHierarchy && ua.act != null)
                    {
                        if (Time.time >= (ua.lastTime + ua.timeGap))
                        {
                            ua.lastTime = Time.time;
                            try
                            {
                                ua.act();
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError(e.Message);
                                trashList.Add(updateableKey[i]);
                            }
                        }
                    }
                }
                else
                {
                    trashList.Add(updateableKey[i]);
                }
            }

            foreach (var key in trashList)
            {
                Remove_Update(key);
            }

        }
        #endregion

        #region delayed action
        [SerializeField] long delayedActIDCounter;
        [SerializeField] long queuedDelayedActs;
        public class DelayedAct
        {
            public long actID;
            public Action act;
            public float time;
            public bool useRealTime;
            public bool needsMono;
            public MonoBehaviour monoRef;
            public DelayedAct(long actID, Action act, float delay, bool useRealTime, bool needsMono, MonoBehaviour monoRef)
            {
                this.actID = actID;
                this.monoRef = monoRef;
                this.needsMono = needsMono;
                this.useRealTime = useRealTime;
                this.act = act;
                if (this.useRealTime)
                {
                    this.time = Time.realtimeSinceStartup + delay;
                }
                else
                {
                    this.time = Time.time + delay;
                }
                if (act == null)
                {
                    Debug.LogError("act is null!");
                }
            }
        }
        private List<DelayedAct> delayedActions = new List<DelayedAct>(10000);
        //private List<long> ids= new List<long>(10000);

        public static DelayedAct Add_DelayedMonoAct(MonoBehaviour mono, System.Action act, float delay, bool useRealTime = false)
        {
            if (act == null)
            {
                Debug.LogError("Tried to add null action on delayed act manager!");
                return null;
            }

            Init();
            long actID = instance.delayedActIDCounter++;
            DelayedAct da = new DelayedAct(actID, act, delay, useRealTime, true, mono);
            instance.delayedActions.Add(da);
            //instance.ids.Add(actID);
            return da;
        }

        public static DelayedAct Add_DelayedAct(System.Action act, float delay, bool useRealTime= false)
        {
            if (act == null)
            {
                Debug.LogError("Tried to add null action on delayed act manager!");
                return null;
            }

            Init();

            long actID = instance.delayedActIDCounter++;
            DelayedAct da = new DelayedAct(actID, act, delay, useRealTime, false, null);
            instance.delayedActions.Add(da);
            //instance.ids.Add(actID);
            return da;
        }
        public static void DeQueue_DelayedAct(DelayedAct act, bool triggerActionOnDeQueue=false)
        {
            for (int i = 0; i < instance.delayedActions.Count; i++)
            {
                if (instance.delayedActions[i] == act)
                {
                    DelayedAct da = instance.delayedActions[i];
                    instance.delayedActions.RemoveAt(i);
                    //instance.ids.RemoveAt(i);
                    if (triggerActionOnDeQueue)
                    {
#if UNITY_EDITOR
                        da.act?.Invoke();
#else
                        try
                        {
                            da.act?.Invoke();
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"Delayed Error From: {da.monoRef}-{da.monoRef.gameObject}");
                            Debug.LogError(e.Message);
                            Debug.LogError(e.StackTrace);
                            if(e.InnerException!=null)Debug.LogError(e.InnerException.Message);
                            //Debug.Log(da.monoRef.gameObject);
                        }
#endif
                    }
                    break;
                }
            }
        }
        void Update_DelayedActions()
        {
            int N = delayedActions.Count;
            for (int loopIndex = 0, removedCount = 0; loopIndex < N; loopIndex++)
            {
                int correctedIndex = loopIndex - removedCount;
                DelayedAct da = delayedActions[correctedIndex];
                bool timePassed = (da.useRealTime ? (Time.realtimeSinceStartup >= da.time):(Time.time >= da.time));

                if (da.needsMono && !da.monoRef)
                {
                    delayedActions.RemoveAt(correctedIndex);
                    //ids.RemoveAt(correctedIndex);
                    removedCount++;
                }
                else if (timePassed)
                {
                    if (da.act != null)
                    {
                        Action a = da.act; 
                        delayedActions.RemoveAt(correctedIndex);
                        //ids.RemoveAt(correctedIndex);
                        removedCount++;


#if UNITY_EDITOR
                        a?.Invoke();
#else
                        try
                        {
                            a?.Invoke();
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"Delayed Error From: {da.monoRef}-{da.monoRef.gameObject}");
                            Debug.LogError(e.Message);
                            Debug.LogError(e.StackTrace);
                            if (e.InnerException!=null)Debug.LogError(e.InnerException.Message);
                            //Debug.Log(da.monoRef.gameObject);
                        }
#endif


                    }
                }
            }
            queuedDelayedActs = delayedActions.Count;
        }
#endregion

        private void Update()
        {
            Update_UpdatableActions();
            Update_DelayedActions();
        }


        public static Coroutine DoProgressive(float tspan, Action<float> onProgress, Action onComplete, bool useRealTime = false)
        {
            return Instance.StartCoroutine(DoProgressiveRoutine(tspan, onProgress, onComplete,useRealTime));
        }
        public static void DoProgressiveOnMono(MonoBehaviour mono, float tspan, Action<float> onProgress, Action onComplete, bool useRealTime = false)
        {
            mono.StartCoroutine(DoProgressiveRoutine(tspan, onProgress, onComplete,useRealTime));
        }

        public static IEnumerator DoProgressiveRoutine(float tspan, Action<float> onProgress, Action onComplete = null, bool useRealTime = false)
        {
            if (useRealTime)
            {
                QRealTimer timer = new QRealTimer(tspan, Time.realtimeSinceStartup);
                while (!timer.IsTimeOut)
                {
                    onProgress?.Invoke(timer.Progress);
                    yield return null;
                }
            }
            else
            {
                QTimer timer = new QTimer(tspan, Time.time);
                while (!timer.IsTimeOut)
                {
                    onProgress?.Invoke(timer.Progress);
                    yield return null;
                }
            }

            onProgress?.Invoke(1);
            onComplete?.Invoke();
        }

        public static void WaitForTap(Action onTapDown, Action onTapUp=null)
        {
            int id = 0;
            id = Add_Update(Centralizer.instance, () =>
            {
                if (onTapDown !=null && Input.GetMouseButtonUp(0))
                {
                    onTapDown?.Invoke();
                    onTapDown = null;
                }
                if (onTapUp != null && Input.GetMouseButtonDown(0))
                {
                    onTapUp?.Invoke();
                    onTapUp = null;
                }
                if(onTapDown==null && onTapUp==null) Remove_Update(id);
            }, 0);
        }
    }
}