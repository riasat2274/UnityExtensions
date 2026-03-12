using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
namespace RAK
{
    public class Transition_Fade_CanvasGroup : Transition
    {
        CanvasGroup target;
        protected override void EnsureInit()
        {
            if(target == null )target = GetComponent<CanvasGroup>();
        }
        protected override void AppearStart()
        {
            gameObject.SetActive(true);
        }
        protected override void AppearProgress(float p)
        {
            SetImageAlpha(p);
        }
        protected override void DissappearProgress(float p)
        {
            SetImageAlpha(p);
        }
        protected virtual void SetImageAlpha(float a)
        {
            target.alpha = a;
        }
    }
}