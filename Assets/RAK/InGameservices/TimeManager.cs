using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RAK;

namespace RAK
{
    [DefaultExecutionOrder(-10000)]
    public class TimeManager : MonoBehaviour
    {

        public static TimeManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
            Init();
        }
        private void OnDestroy()
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = .02f;
        }
        static TimeFreezer Freezer { get; set; }
        public static bool IsTimeScaled => Freezer.IsTimeScaled;
        void Init()
        {
            Freezer = new TimeFreezer(this);
        }

        public static bool Freeze(float timeSpan, Action onComplete)
        {
            if (IsTimeScaled) return false;
            Freezer.GradualFreeze(timeSpan, onComplete);
            return true;
        }
        public static void Defrost(float timeSpan, Action onComplete)
        {
            if (!IsTimeScaled) onComplete?.Invoke();
            else Freezer.Defrost(timeSpan, onComplete);
        }

        public static void ForceDeFrost()
        {

            Time.timeScale = 1;
            Time.fixedDeltaTime = .02f;
            Instance.Init();
        }


        public const long TicksPerSecond = 10000000;
        public static long TicksNow
        {
            get
            {
                return DateTime.Now.Ticks;
            }
        }

        public static bool IsItTimeYet(long lastClaimed, long period)
        {
            //$"time yet {ticks} >= {lastClaimed} + {period}".Debug("FF0000");
            return (TicksNow >= lastClaimed + period);
        }

        public static float ProgressMade(long lastClaimed, long period)
        {
            //$"time yet {ticks} - {lastClaimed} / {period} = {((ticks - lastClaimed) / (float)period)}".Debug("FF0000");

            return ((TicksNow - lastClaimed) / (float)period);
        }

        public static float SecondsToNext(long lastClaimed, long period)
        {
            return (period - (float)(TicksNow - lastClaimed)) / TicksPerSecond;
        }
        public static float ElapsedTime(long start)
        {
            return ((float)(TicksNow - start)) / TicksPerSecond;
        }
        public static long GetTickTimeByOffsetSecondsFromNow(float offsetTime)
        {
            return GetTickTimeByOffsetSeconds(TicksNow,offsetTime);
        }
        public static long GetTickTimeByOffsetSeconds(long baseValue, float offsetTime)
        {
            return baseValue +  (long)(offsetTime * TicksPerSecond);
        }
        public static IEnumerator SlowMo(Vector3 durations, float slowScale, Action<float> onScaleRatioChange, Action onComplete)
        {
            float startScale = Time.timeScale;
            float fixedDelta = Time.fixedDeltaTime;
            QRealTimer timer = new QRealTimer(durations.x, Time.realtimeSinceStartup);
            while (!timer.IsTimeOut)
            {
                Time.timeScale = Mathf.Lerp(startScale, slowScale, timer.Progress);
                Time.fixedDeltaTime = fixedDelta * Time.timeScale;
                onScaleRatioChange?.Invoke(timer.Progress);
                yield return null;
            }
            onScaleRatioChange?.Invoke(1);
            Time.timeScale = slowScale;
            Time.fixedDeltaTime = fixedDelta * Time.timeScale;
            yield return new WaitForSecondsRealtime(durations.y);
            timer = new QRealTimer(durations.z, Time.realtimeSinceStartup);
            while (!timer.IsTimeOut)
            {
                Time.timeScale = Mathf.Lerp(slowScale, startScale, timer.Progress);
                onScaleRatioChange?.Invoke(1 - timer.Progress);
                yield return null;
            }

            onScaleRatioChange?.Invoke(0);
            Time.timeScale = startScale;
            Time.fixedDeltaTime = fixedDelta * Time.timeScale;
            onComplete?.Invoke();
        }
        /// <summary>
        /// positive time to min:sec
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Time_MinSec(float t)
        {
            float time = Mathf.Clamp(t, 0, int.MaxValue);
            int minutes = Mathf.FloorToInt(time / 60F); int seconds = Mathf.FloorToInt(time % 60F); 
            return string.Format("{0:0}:{1:00}", minutes, seconds);
        }

        public class TimeFreezer
        {
            MonoBehaviour mono;
            float startScale;
            float fixedDelta;
            Coroutine freezingRoutine;
            bool fullResetTimeScale = true;


            public bool IsTimeScaled => Time.timeScale != 1;
            public TimeFreezer(MonoBehaviour mono, bool fullResetTimeScale = true)
            {
                this.mono = mono;
                startScale = Time.timeScale;
                fixedDelta = Time.fixedDeltaTime;
                this.fullResetTimeScale = fullResetTimeScale;
            }
            public void GradualFreeze(float timeSpan, Action onComplete)
            {
                freezingRoutine = mono.StartCoroutine(GradualFreezeRoutine(timeSpan, onComplete));
            }
            IEnumerator GradualFreezeRoutine(float timeSpan, Action onComplete)
            {
                QRealTimer timer = new QRealTimer(timeSpan, Time.realtimeSinceStartup);
                while (!timer.IsTimeOut)
                {
                    Time.timeScale = Mathf.Lerp(startScale, 0, timer.Progress);
                    Time.fixedDeltaTime = fixedDelta * Time.timeScale;
                    yield return null;
                }
                Time.timeScale = 0;
                Time.fixedDeltaTime = fixedDelta * Time.timeScale;

                onComplete?.Invoke();
            }
            public void Defrost(float timeSpan, Action onComplete)
            {
                if (freezingRoutine != null) mono.StopCoroutine(freezingRoutine);
                mono.StartCoroutine(DefrostRoutine(timeSpan, onComplete));
            }
            IEnumerator DefrostRoutine(float timeSpan, Action onComplete)
            {
                QRealTimer timer = new QRealTimer(timeSpan, Time.realtimeSinceStartup);
                while (!timer.IsTimeOut)
                {
                    Time.timeScale = Mathf.Lerp(0, startScale, timer.Progress);
                    Time.fixedDeltaTime = fixedDelta * Time.timeScale;
                    yield return null;
                }
                if (fullResetTimeScale)
                {
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = .02f;
                }
                else
                {
                    Time.timeScale = startScale;
                    Time.fixedDeltaTime = fixedDelta;
                }
                onComplete?.Invoke();
            }
        }

    }

    [Serializable]
    public class TimedItem
    {
        public string timer_key;
        public float period_seconds;
        public TimedItem(){}

        public TimedItem(string key, float period_seconds, bool startReady){
            this.timer_key = key;
            this.period_seconds = period_seconds;
            Init(startReady);
        }
        public long periodTicks => (long)(period_seconds * TimeManager.TicksPerSecond);
        public bool IsPeriodComplete => TimeManager.IsItTimeYet(lastClaimed.value, periodTicks);
        public float TimeRemaining_Seconds => TimeManager.SecondsToNext(lastClaimed.value, periodTicks);
        public string TimeRemaining_String => $"{Mathf.Clamp(Mathf.CeilToInt(TimeRemaining_Seconds), 0, int.MaxValue)}";
        public string TimeRemaining_MinSecString => TimeManager.Time_MinSec(TimeRemaining_Seconds);
        public float ProgressMade => TimeManager.ProgressMade(lastClaimed.value, periodTicks);

        [NonSerialized] HardData<long> lastClaimed;
        public void Init(bool startReady)
        {
            long offset=  TimeManager.GetTickTimeByOffsetSecondsFromNow(startReady?-period_seconds:0);
            lastClaimed = new HardData<long>(timer_key, offset);
        }
        public float ClaimAndReset()
        {
            float f = ProgressMade;
            lastClaimed.value = TimeManager.TicksNow;
            return Mathf.Clamp01(f);
        }
        public void InstaFill()
        {
            lastClaimed.value = TimeManager.GetTickTimeByOffsetSecondsFromNow( -period_seconds);
        }

        public void SendBackStartTimeBy(float seconds)
        {
            long ticks = (long)(seconds * TimeManager.TicksPerSecond);
            lastClaimed.value = lastClaimed.value - ticks;
        }
    }

    [Serializable]
    public class TimedAutoFillResource
    {

        public string timer_key;
        public float unit_collection_time;
        public int max_inventory;


        public bool IsMaxedOut => CollectedCount >= max_inventory;
        public int Count => CollectedCount + bonusCount.value;
        public int EmptyCount => max_inventory - Count;
        public int CollectedCount =>  Mathf.FloorToInt(CollectedValue);
        float CollectedValue => Mathf.Clamp(ElapsedCollectionTime / unit_collection_time, 0, max_inventory);
        float ElapsedCollectionTime => TimeManager.ElapsedTime(collectionStartTime.value);
        [NonSerialized] HardData<long> collectionStartTime;
        [NonSerialized] HardData<int> bonusCount;
        public string RefillInfoText { 
            get 
            {
                if (CollectedValue == max_inventory) return $"Capacity Full";
                else
                {
                    float timeTillNext = (1 - (CollectedValue - CollectedCount)) * unit_collection_time;
                    return $"{TimeManager.Time_MinSec(timeTillNext)}";
                }
            }
        }
        public void Init( int initialBonusCount)
        {
            Init(max_inventory, initialBonusCount);
        }
        public void Init(int startValue, int initialBonusCount)
        {


            bonusCount = new HardData<int>($"{timer_key}_BONUS_COUNT", initialBonusCount);
            collectionStartTime = new HardData<long>($"{timer_key}_CSTIME", StartTimeByCurrentValue(startValue));
        }

        public void Recharge(int count)
        {
            int space = max_inventory - CollectedCount;
            if (space >= count)
            {
                collectionStartTime.value = StartTimeByCurrentValue(CollectedValue + count);
            }
            else
            {
                if (space > 0)
                {
                    collectionStartTime.value= StartTimeByCurrentValue(CollectedValue + space);
                    count -= space;
                }
                bonusCount.value += count;
            }
        }
        public void Consume(int count)
        {
            //Debug.Log(bonusCount.value);
            if (bonusCount.value > 0)
            {
                if (bonusCount.value >= count)
                {
                    bonusCount.value -= count;
                    count = 0;
                }
                else
                {
                    count -= bonusCount.value;
                    bonusCount.value = 0;
                }
            }
            if(count>0) collectionStartTime.value = StartTimeByCurrentValue(CollectedValue - count);
            //Debug.Log(bonusCount.value);
            //Debug.Log(CollectedCount);
        }

        long StartTimeByCurrentValue(float value)
        {
            float timeWorth = value * unit_collection_time;
            return TimeManager.GetTickTimeByOffsetSecondsFromNow(-timeWorth);
        }
    }

    [Serializable]
    public class TimeStamp
    {
        public string timer_key;
        [NonSerialized] HardData<long> _startTimeHD;
        public long startTime { get; private set; }
        public void Init(float offset)
        {
            long lv = TimeManager.GetTickTimeByOffsetSecondsFromNow(offset);
            _startTimeHD = new HardData<long>(timer_key, lv);
        }

        long PeriodTicks(float period_s)
        {
            return (long)(period_s * TimeManager.TicksPerSecond); 
        }
        public float ProgressTo(float period_s)
        {
            return TimeManager.ProgressMade(_startTimeHD.value, PeriodTicks(period_s));
        }
        public bool IsPeriodComplete(float period_s)
        {
            return TimeManager.IsItTimeYet(_startTimeHD.value, PeriodTicks(period_s));
        }
    }
    [Serializable]
    public class TimeRecharge
    {
        public string timer_key;
        [NonSerialized] HardData<long> _chargeEndValidityHD;
        public void Init(float offset)
        {
            long lv = TimeManager.GetTickTimeByOffsetSecondsFromNow(offset);
            _chargeEndValidityHD = new HardData<long>(timer_key, lv);
        }
        public bool IsCharged => !OutOfTime;
        public bool OutOfTime => TimeManager.TicksNow > _chargeEndValidityHD.value;
        public long TicksLeft => _chargeEndValidityHD.value - TimeManager.TicksNow;
        public float TimeRemaining_Seconds => TicksLeft /(float) TimeManager.TicksPerSecond;
        public string TimeRemaining_MinSecString => TimeManager.Time_MinSec(TimeRemaining_Seconds);

        public void Recharge(float time)
        {
            if (OutOfTime) _chargeEndValidityHD.value = TimeManager.GetTickTimeByOffsetSecondsFromNow(time);
            else _chargeEndValidityHD.value = TimeManager.GetTickTimeByOffsetSeconds(_chargeEndValidityHD.value, time);
        }

    }
    public class QTimer
    {
        public float Span { get; private set; }
        float stime;
        Centralizer.DelayedAct onTimeOutAct;
        public QTimer(float span, Action onTimeOut = null)
        {
            this.Span = span;
            this.stime = Time.time;
            if (onTimeOut != null)
            {
                onTimeOutAct = Centralizer.Add_DelayedAct(onTimeOut, span);
            }
        }
        public QTimer(float span, float stime, Action onTimeOut = null)
        {
            this.Span = span;
            this.stime = stime;
            if (onTimeOut != null)
            {
                float stDiff = Time.time - stime;
                onTimeOutAct = Centralizer.Add_DelayedAct(onTimeOut, span - stDiff);
            }
        }
        public void CleanUp()
        {
            if (onTimeOutAct !=null)
            {
                Centralizer.DeQueue_DelayedAct(onTimeOutAct);
            }
        }
        public void Reset()
        {
            stime = Time.time;
        }
        public float Progress { get { return (Time.time - stime) / Span; } }
        public bool IsTimeOut { get { return Progress >= 1; } }
        public float TimeLeft { get { return Span - (Time.time - stime); } }
        public int CountDownTime
        {

            get
            {
                int t = Mathf.RoundToInt(TimeLeft);
                if (t < 0) t = 0;
                return t;
            }
        }
    }
    public class QRealTimer
    {
        float span;
        float stime;
        public QRealTimer(float span, float stime)
        {
            this.span = span;
            this.stime = stime;
        }
        public void Reset()
        {
            stime = Time.realtimeSinceStartup;
        }
        public float Progress { get { return (Time.realtimeSinceStartup - stime) / span; } }
        public bool IsTimeOut { get { return Progress >= 1; } }
        public float TimeLeft { get { return span - (Time.realtimeSinceStartup - stime); } }
        public int CountDownTime
        {

            get
            {
                int t = Mathf.RoundToInt(TimeLeft);
                if (t < 0) t = 0;
                return t;
            }
        }
    }
    public class QRealTimeStamp
    {
        public float stime { get; private set; }
        public QRealTimeStamp(float offset = 0)
        {
            this.stime = Time.realtimeSinceStartup + offset;
        }
        public void Reset()
        {
            stime = Time.realtimeSinceStartup;
        }
        public float ProgressTo(float period_s)
        { 
            return (Time.realtimeSinceStartup - stime) / period_s; 
        }
        public bool IsTimeOut(float period_s) { return ProgressTo(period_s) >= 1;  }
        public float TimeLeftTill(float period_s) { return period_s - (Time.realtimeSinceStartup - stime); } 
        public int CountDownTime(float period_s)
        {
                int t = Mathf.RoundToInt(TimeLeftTill(period_s));
                if (t < 0) t = 0;
                return t;
        }
    }
}