using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RAK;
//using PotatoSDK;
#if UNITY_EDITOR
using UnityEditor;

#endif
public class BuildSpecManager : MonoBehaviour
{
    public BuildType buildType;
    public string version;
    public int build_production;
    public int build_qa;
    public int build_marketing;
    public static bool isGameplay
    {
        get
        {
            if (!instance) return true;
            else
            {
                switch (instance.buildType)
                {
                    default:
                    case BuildType.PRODUCTION:
                    case BuildType.QA:
                        return true;
                    case BuildType.CREATIVE_0:
                    case BuildType.CREATIVE_1:
                        return false;
                }
            }
        }
    }
    public static bool isCreative
    {
        get
        {
            if (!instance) return false;
            else
            {
                switch (instance.buildType)
                {
                    default:
                    case BuildType.PRODUCTION:
                    case BuildType.QA:
                        return false;
                    case BuildType.CREATIVE_0:
                    case BuildType.CREATIVE_1:
                        return true;
                }
            }
        }
    }
    #region settings 

    //================================================================== version>0.6
    //public static bool disableBonusLevels => false; //play as normal level
    //public static bool enableHomeScene
    //{
    //    get
    //    {
    //        if (!instance) return true;
    //        else
    //        {
    //            switch (instance.buildType)
    //            {
    //                default:
    //                case BuildType.PRODUCTION:
    //                    return true;
    //                case BuildType.CREATIVE_0:
    //                case BuildType.QA:
    //                    return true;
    //            }
    //        }
    //    }
    //}
    //public static bool enableLegacyUI
    //{
    //    get
    //    {
    //        if (enableHomeScene) return false;
    //        return true;
    //    }
    //}
    public static bool enableFPSReporter
    {
        get
        {
            if (!instance) return true;
            else
            {
                switch (instance.buildType)
                {
                    default:
                    case BuildType.PRODUCTION:
                    case BuildType.CREATIVE_0:
                    case BuildType.CREATIVE_1:
                        return false;
                    case BuildType.QA:
                        return true;
                }
            }
        }
    }
    //================================================================== version>0.7
    public static bool enablePlayerMovementInertia => false;
    public static bool enableVariableProgress => false;

    //================================================================== from prev games
    public static bool enableUIToggle
    {
        get
        {
            if (!instance) return false;
            else
            {
                switch (instance.buildType)
                {
                    default:
                        return false;
                    case BuildType.CREATIVE_0:
                        return true;
                }
            }
        }
    }
    public static bool disableCoinReduction
    {
        get
        {
            if (!instance) return false;
            else
            {
                switch (instance.buildType)
                {
                    default:
                        return false;
                    case BuildType.CREATIVE_1:
                    case BuildType.CREATIVE_0:
                    case BuildType.QA:
                        return true;
                }
            }
        }
    }
    public static bool allLevelUnlocked
    {
        get
        {
            if (!instance) return false;
            else
            {
                switch (instance.buildType)
                {
                    default:
                        return false;
                    case BuildType.CREATIVE_1:
                    case BuildType.CREATIVE_0:
                    case BuildType.QA:
                        return true;
                }
            }
        }
    }
    public static bool allPurchaseUnlocked
    {
        get
        {
            if (!instance) return false;
            else
            {
                switch (instance.buildType)
                {
                    default:
                        return false;
                    case BuildType.CREATIVE_1:
                    case BuildType.CREATIVE_0:
                    //case BuildType.LION_QA:
                        return true;
                }
            }
        }
    }
    public static int initialCoinBonus
    {
        get
        {
            if (!instance) return 100;
            else
            {
                switch (instance.buildType)
                {
                    default:
                        return 0;
                    case BuildType.CREATIVE_1:
                    case BuildType.CREATIVE_0:
                    case BuildType.QA:
                        return 5000;
                }
            }
        }
    }
    public static bool enableABTestUI
    {
        get
        {
            if (!instance) return false;
            else
            {
                switch (instance.buildType)
                {
                    default:
                        return false;
                    case BuildType.QA:
                        return true;
                }
            }
        }
    }

    #endregion
    public static BuildSpecManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            if (buildType == BuildType.PRODUCTION && Screen.height == 1350 && Screen.width == 1080)
            {
                buildType = BuildType.CREATIVE_0;
                $"Build type switched to {buildType}, due to resolution".Debug("FFFF00");
            }

            if ( ((int)buildType>=20) && (Screen.height != 1350 || Screen.width != 1080))
            {
                buildType = BuildType.PRODUCTION;
                $"Build type switched to {buildType}, due to resolution".Debug("FFFF00");
            }
            //if (instance.txt1) instance.txt1.text = "";
            this.transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
            //Log2("initialized");
            
            //if (enableUIToggle)
            //{
            //    uiToggleButton.onClick.AddListener(OnUIToggleButton);
            //}
            //else
            //{
            //    uiToggleButton.gameObject.SetActive(false);
            //}
        }
    }
    //public Text txt1;
    //public Text txt2;






    //public static event Action onUIToggle;
    public static Dictionary<Camera, int> camdic = new Dictionary<Camera, int>();
    //public Button uiToggleButton;

    public static bool uiDisabled { get; private set; }
    public void OnUIToggleButton()
    {
        //onUIToggle?.Invoke();
        uiDisabled = !uiDisabled;
    }

    //public LayerMask uilayer;
    //private void Update()
    //{
    //    if (!enableUIToggle) return;
    //    Camera[] cams = FindObjectsOfType<Camera>();
    //    foreach (Camera cam in cams)
    //    {
    //        if (!camdic.ContainsKey(cam))
    //        {
    //            camdic.Add(cam, cam.cullingMask);
    //        }
    //    }

    //    foreach (var item in camdic)
    //    {
    //        if (item.Key == null) camdic.Remove(item.Key);
    //    }
    //    foreach (var item in camdic)
    //    {
    //        if (!uiDisabled)
    //        {
    //            item.Key.cullingMask = item.Value ;
    //        }
    //        else
    //        {
    //            item.Key.cullingMask = item.Value - (int)uilayer;
    //        }
    //    }
    //}
    //public static void Log1(string s)
    //{
    //    if (!instance) return;
    //    if (instance.txt1) instance.txt1.text += s;
    //}
    //public static void Log2(string s)
    //{
    //    if (!instance) return;
    //    if (instance.txt2) instance.txt2.text = s;
    //}
    public enum BuildType
    {
        PRODUCTION = 0,
        QA = 10,
        CREATIVE_0 = 20,
        CREATIVE_1 = 21,
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BuildSpecManager))]
public class BuildSpecManager_Editor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Set Up Build"))
        {
            BuildSpecManager bsm = target as BuildSpecManager;

            PlayerSettings.bundleVersion = bsm.version;
            switch (bsm.buildType)
            {
                case BuildSpecManager.BuildType.PRODUCTION:
                    PlayerSettings.Android.bundleVersionCode = bsm.build_production;
                    PlayerSettings.iOS.buildNumber = bsm.build_production.ToString();
                    break;
                case BuildSpecManager.BuildType.CREATIVE_1:
                case BuildSpecManager.BuildType.CREATIVE_0:
                    PlayerSettings.Android.bundleVersionCode = bsm.build_marketing;
                    PlayerSettings.iOS.buildNumber = bsm.build_marketing.ToString();
                    break;
                case BuildSpecManager.BuildType.QA:
                    PlayerSettings.Android.bundleVersionCode = bsm.build_qa;
                    PlayerSettings.iOS.buildNumber = bsm.build_qa.ToString();
                    break;
                default:
                    break;
            }
        }
        AssetDatabase.Refresh();
    }
}

#endif