using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace RAK
{
    [System.Serializable]
    public class ChanceKeeper : ScriptableObject
    {
        public int pseudoSize = 10;
        public float pseudoStrength = 10;
        public List<GameobjectChance> options = new List<GameobjectChance>();

        [System.NonSerialized] public PseudoChancedList<GameObject> clist;
        public void Init()
        {
            clist = new PseudoChancedList<GameObject>();
            clist.Refresh(pseudoSize, pseudoStrength);
            foreach (var item in options)
            {
                clist.Add(item.gameobject, item.chance);
            }
            clist.Normalize();
        }

        public GameObject Roll()
        {
            if (clist == null)
            {
                Debug.LogError("Chance Manager not initialized...");
                return null;
            }
            return clist.Roll();
        }

    }
    [System.Serializable]
    public class GameobjectChance
    {
        public GameObject gameobject;
        public float chance = 1;
        //public int spawnParam = 0;
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(GameobjectChance))]
        public class ElementChanceEditor : FastPropertyDrawer
        {
            protected override void OnGUICore()
            {
                base.OnGUICore();
                AddToRow("chance", 30);
                AddToRow("gameobject");
                //AddToRow("spawnParam");
            }
        }
#endif
    }

}