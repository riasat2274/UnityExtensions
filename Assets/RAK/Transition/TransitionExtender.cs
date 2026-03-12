using RAK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class TransitionExtender 
{
    //public List<Transition> transitions;
    //public override void Appear(Action onComplete = null)
    //{
    //    foreach (var t in transitions) { t.Appear(); }
    //}

    public static void SequentialTransitionActive(this List<Transition> trlist, bool enable, int startIndex=0, Action onComplete = null, float gap = 0)
    {
        if (startIndex>=trlist.Count) onComplete?.Invoke();
        else
        {
            trlist[startIndex].TransitionActive(enable, () => {
                Centralizer.Add_DelayedAct(() => {
                    SequentialTransitionActive(trlist, enable, startIndex + 1, onComplete,gap);
                }, gap, trlist[startIndex].useRealTime);
            });
        }
    }
    
    
    public static void TransitionActive(this List<Transition> trlist, bool enable, Action onComplete = null)
    {
        float longestDuration = 0f;
        int longestIndex = -1;
        for (int i = 0; i < trlist.Count; i++)
        {
            float duration = enable? trlist[i].inTime : trlist[i].outTime;
            if (duration > longestDuration)
            {
                longestDuration = duration;
                longestIndex = i;
            }
        }
        for (int i = 0; i < trlist.Count; i++)
        {
            trlist[i].TransitionActive(enable, i== longestIndex? onComplete : null);
        }
    }
    public static void ForceActive(this List<Transition> trlist, bool enable)
    {
        foreach (var tr in trlist)
        {
            tr.ForceActive(enable);
        }
    }
    public static float MaxTimeIn(this List<Transition> trlist)
    {
        float mTime = 0f;
        foreach (var tr in trlist)
        {
            if(tr.inTime> mTime) mTime = tr.inTime;
        }
        return mTime;
    }
    public static float MaxTimeOut(this List<Transition> trlist)
    {
        float mTime = 0f;
        foreach (var tr in trlist)
        {
            if (tr.outTime > mTime) mTime = tr.outTime;
        }
        return mTime;
    }
}
