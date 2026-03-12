using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
namespace RAK
{
    public class Transition_Fly2D : Transition
    {
        public Vector2 offetIn;
        public Vector2 offetOut;

        bool initialized = false;
        public bool useAnchoredPos = true;
        public bool hasRecordedCorePos;
        public Vector2 corePos;
        float corez;

        RectTransform rectTR => transform as RectTransform;
        [ContextMenu("Record")]
        public void RecordCorePos()
        { 
            hasRecordedCorePos = true;
            corePos = useAnchoredPos? rectTR.anchoredPosition: new Vector2(transform.localPosition.x,transform.localPosition.y);
            corez = transform.localPosition.z;
            if (log) Debug.Log($" core pos : {corePos}, Screen: {Screen.height}, {Screen.width}".CMagenta());
        }
        [ContextMenu("Clear")]
        public void ClearRecord()
        {
            hasRecordedCorePos = false;
        }
        protected override void EnsureInit()
        {
            if (initialized) return;
            if (!hasRecordedCorePos)RecordCorePos();
            initialized = true;
        }
        Vector2 startPos => corePos + offetIn;
        Vector2 finalPos => corePos + offetOut;
        protected override void AppearStart()
        {
            gameObject.SetActive(true);
            if (useAnchoredPos)
            {
                rectTR.anchoredPosition = startPos;
            }
            else
            {
                transform.localPosition = startPos.SetZ(corez);
            }
        }
        protected override void AppearProgress(float p)
        {
            if (useAnchoredPos)
            {
                rectTR.anchoredPosition = Vector2.Lerp(startPos, corePos, p);
            }
            else
            {
                transform.localPosition = Vector2.Lerp(startPos, corePos, p).SetZ(corez);
            }
        }
        protected override void AppearEnd()
        {
            if (useAnchoredPos)
            {
                rectTR.anchoredPosition = corePos;
            }
            else
            {
                transform.localPosition = corePos.SetZ(corez);
            }
            gameObject.SetActive(true);
        }

        protected override void DissappearStart()
        {
            if (useAnchoredPos)
            {

                rectTR.anchoredPosition = corePos;
            }
            else
            {
                transform.localPosition = corePos.SetZ(corez);
            }
        }
        protected override void DissappearProgress(float p)
        {
            if (useAnchoredPos)
            {
                rectTR.anchoredPosition = Vector2.Lerp(corePos, finalPos, 1 - p);
            }
            else
            {
                transform.localPosition = Vector2.Lerp(corePos, finalPos, 1 - p).SetZ(corez); ;
            }
        }
        protected override void DissappearEnd()
        {
            if (useAnchoredPos)
            {
                rectTR.anchoredPosition = finalPos;
            }
            else
            {
                transform.localPosition = finalPos.SetZ(corez);

            }
            gameObject.SetActive(false);
        }
      
    }
}