using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{
    [System.Serializable]
    public class ChancedFloat : ChancedElement<float>
    {
    }

    [System.Serializable]
    public class ChancedInt : ChancedElement<int>
    {
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ChancedFloat))]
    public class ChancedFloatDrawer : FastPropertyDrawer
    {

        protected override void OnGUICore()
        {
            base.OnGUICore();
            //AddLabel(this.displayName, .3f);
            AddToRow("element", 120);
            AddLabel("Chance:", 50);
            AddToRow("chance");

        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(ChancedInt))]
    public class ChancedIntDrawer : FastPropertyDrawer
    {
        protected override void OnGUICore()
        {
            base.OnGUICore();
            //AddLabel(this.displayName, .3f);
            AddToRow("element", 120);
            AddLabel("Chance:", 50);
            AddToRow("chance");
        }
    }
#endif

}
