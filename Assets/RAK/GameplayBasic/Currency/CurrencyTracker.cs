using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;




#if UNITY_EDITOR
using UnityEditor;
#endif  
namespace RAK
{
    public class CurrencyTracker : MonoBehaviour
    {
        public TextMeshProUGUI coinDisplayText;
        public CurrencyType currencyType;
        public Image icon;
        public float timeToAnimate = 1;
        public float stepTime = 1 / 20.0f;

        double savedValue;
        double displayValue;
        public bool pauseExternal;

        public int idleFormatThreshold = 99999;


        public Transform scaleTarget;
        public AnimationCurve popAnimationCurve;

        public Button requestMoreButton;
        public void DisplayString()
        {
            if (displayValue>idleFormatThreshold)
            {
                coinDisplayText.text = displayValue.IdleFormatter();
            }
            else
            {
                coinDisplayText.text = displayValue.ToString("0");
            }
        }
        private void Awake()
        {
            if (requestMoreButton)
            {
                requestMoreButton.onClick.RemoveAllListeners();
                requestMoreButton.onClick.AddListener(() => Currency.RequestMore(currencyType));
            }
        }
        void OnEnable()
        {
            scaleTarget.localScale = Vector3.one;
            savedValue = Currency.Balance(currencyType);
            displayValue = savedValue;
            DisplayString();
            if (runningTextRoutine != null)
            {
                StopCoroutine(runningTextRoutine);
                runningTextRoutine = null;
            }
            Currency.coinMan.AddListner_BalanceChanged(currencyType, OnCoinChange);
        }
        void OnCoinChange(double change)
        {
            savedValue = Currency.Balance(currencyType);
            if (runningTextRoutine != null) StopCoroutine(runningTextRoutine);
            runningTextRoutine = StartCoroutine(TextRoutine());
        }

        private void OnDisable()
        {
            Currency.coinMan.RemoveListner_BalanceChanged(currencyType, OnCoinChange);
        }

        Coroutine runningTextRoutine;
        IEnumerator TextRoutine()
        {
            float startTime = Time.realtimeSinceStartup;
            int stepCount = Mathf.RoundToInt(timeToAnimate / this.stepTime);

            float stepTime = timeToAnimate / stepCount;
            double diff = savedValue - displayValue;
            double initialValue = displayValue;
            if(scaleTarget) Centralizer.DoProgressive(timeToAnimate, (float p) => 
            {
                scaleTarget.localScale = Vector3.one * popAnimationCurve.Evaluate(p);
            }, () => 
            {
                scaleTarget.localScale = Vector3.one;
            });
            for (int i = 0; i < stepCount; i++)
            {
                while (pauseExternal)
                {
                    yield return null;
                    startTime += Time.deltaTime;
                }

                yield return new WaitForSecondsRealtime(stepTime);

                float p = Mathf.Clamp01((Time.realtimeSinceStartup - startTime) / timeToAnimate);
                displayValue = initialValue + diff * p;
                DisplayString();
            }
            displayValue = savedValue;
            DisplayString();
        }


        public static Dictionary<CurrencyType, GameObject> flyFabs = new();
        void EnsureFlyFab()
        {
            if (flyFabs.ContainsKey(currencyType))
            {
                if(flyFabs[currencyType] != null) return;
                else flyFabs.Remove(currencyType);
            }
            GameObject fab = Instantiate(icon.gameObject, null);
            fab.transform.position = Vector3.zero;
            fab.transform.rotation = Quaternion.identity;
            fab.transform.localScale = Vector3.one;
            fab.gameObject.SetActive(false);
            flyFabs.Add(currencyType, fab);
        }
        public void RequestCurrencyFlyIn(RectTransform sourceTransform, AnimationCurve curve, float tspan, Action onComplete)
        {
            EnsureFlyFab();
            GameObject fab = flyFabs[currencyType];
            GameObject flier = Pool.Instantiate(fab, sourceTransform.parent);
            flier.gameObject.SetActive(true);
            RectTransform flRect = flier.GetComponent<RectTransform>();
            flRect.position = sourceTransform.position;
            flRect.rotation = sourceTransform.rotation;
            flRect.localScale = sourceTransform.localScale;
            flRect.sizeDelta = sourceTransform.sizeDelta;

            RectTransform targetRect = icon.GetComponent<RectTransform>();
            flRect.SetParent(targetRect);
            Vector3 startPos = flRect.localPosition;
            Quaternion startRot = flRect.localRotation;
            Vector3 startScale = flRect.localScale;
            Vector2 startSize = flRect.sizeDelta;
            Centralizer.DoProgressive(tspan, (float p) =>
            {
                float cp = curve.Evaluate(p);
                flRect.localPosition = Vector3.Lerp(startPos, Vector3.zero, cp);
                flRect.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, cp);
                flRect.localScale = Vector3.Lerp(startScale, Vector3.one, cp);
                flRect.sizeDelta = Vector2.Lerp(startSize, flRect.sizeDelta, cp);

            }, () =>
            {
                flRect.localPosition = Vector3.zero;
                flRect.localRotation = Quaternion.identity;
                flRect.localScale = Vector3.one;
                flRect.sizeDelta = targetRect.sizeDelta;
                Pool.Destroy(flier);
                onComplete?.Invoke();
            });

        }


    }


#if UNITY_EDITOR
    [CustomEditor(typeof(CurrencyTracker))]
    public class CurrencyTrackerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CurrencyTracker pct = (CurrencyTracker)target;
            bool isSmallCurrency = pct.currencyType == CurrencyType.STAR;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+2000"))
            {
                Currency.Transaction(pct.currencyType, isSmallCurrency?4 :2000 );
            }
            if (GUILayout.Button("-1000"))
            {
                Currency.Transaction(pct.currencyType, -(isSmallCurrency ? 2 : 1000));
            }
            if (GUILayout.Button("x2"))
            {
                Currency.Transaction(pct.currencyType, Currency.Balance(pct.currencyType));
            }
            if (GUILayout.Button("/2"))
            {
                Currency.Transaction(pct.currencyType, -Currency.Balance(pct.currencyType) / 2);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}