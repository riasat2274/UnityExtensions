using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{
    public class Transition : MonoBehaviour
    {
        public AnimationCurve transitionCurve;
        protected Coroutine transitionBaseRoutine;
        public bool useRealTime;
        public bool log;
        public enum State
        {
            Inactive = 0,
            Transitioning = 1,
            Active =2,
        }
        public State state = 0;

        public float inTime = .25f;
        public float outTime = .15f;
        protected virtual void EnsureInit()
        { 
        }
        protected virtual void AppearStart()
        {

        }
        protected virtual void AppearProgress(float p)
        {

        }
        protected virtual void AppearEnd()
        {
            gameObject.SetActive(true);
        }
        protected virtual void DissappearStart()
        {

        }
        protected virtual void DissappearProgress(float p)
        {

        }
        protected virtual void DissappearEnd()
        {
            gameObject.SetActive(false);
        }
        public void Appear(Action onComplete=null)
        {
            EnsureInit();
            state = State.Transitioning;
            AppearStart();

            if (log) "AppearStart".Debug();
            transitionBaseRoutine = Centralizer.DoProgressive(inTime, (float p) => {
                AppearProgress(transitionCurve.Evaluate(p));
            }, () => {
                AppearProgress(transitionCurve.Evaluate(1));
                AppearEnd();
                state = State.Active;
                onComplete?.Invoke();
                if (log) "AppearEnd".Debug();
            },useRealTime);
        }

        public void Dissapear(Action onComplete = null)
        {
            EnsureInit();
            state = State.Transitioning;
            DissappearStart();
            if (log) "DisAppearStart".Debug();
            transitionBaseRoutine = Centralizer.DoProgressive(inTime, (float p) => {
                DissappearProgress(transitionCurve.Evaluate(1 - p));
            }, () => {
                DissappearProgress(0);
                DissappearEnd();
                state = State.Inactive;
                onComplete?.Invoke();
                if (log) "DisAppearStart".Debug();
            }, useRealTime);
        }


        public void ForceDissapear()
        {
            EnsureInit();
            state = State.Inactive; 
            DissappearProgress(1);
            DissappearEnd();
            if (transitionBaseRoutine != null) { 
                StopCoroutine(transitionBaseRoutine); 
                transitionBaseRoutine = null;
            }

        }
        public void ForceAppear()
        {
            EnsureInit();
            state = State.Inactive;
            AppearProgress(1);
            AppearEnd();
            if (transitionBaseRoutine != null)
            {
                StopCoroutine(transitionBaseRoutine);
                transitionBaseRoutine = null;
            }

        }

        public void TransitionActive(bool enable, Action onComplete=null)
        {
            if (enable) Appear(onComplete);
            else Dissapear(onComplete);
        }
        public void ForceActive(bool enable)
        {
            if (enable) ForceAppear();
            else ForceDissapear();
        }
    }
}