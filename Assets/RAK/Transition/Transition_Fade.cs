using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RAK
{
    public class Transition_Fade : Transition
    {
        public Image img;

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
            Color c = img.color;
            c.a = a;
            img.color = c;
        }
    }
}