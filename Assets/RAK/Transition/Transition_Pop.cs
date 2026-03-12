using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace RAK
{
    public class Transition_Pop : Transition
    {
        protected override void AppearStart()
        {
            gameObject.SetActive(true);
        }
        protected override void AppearProgress(float p)
        {
            transform.localScale = Vector3.one * p;
        }
        protected override void DissappearProgress(float p)
        {
            transform.localScale = Vector3.one * p;
        }
    }
}