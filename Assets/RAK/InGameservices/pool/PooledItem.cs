using UnityEngine;
using System.Collections.Generic;
namespace RAK
{
    /* See Pool.cs for Guide*/
    /// <summary>
    /// This class is automatically attached to all the gameobjects instantitated by Pool.cs
    /// This class is also needed to be manually attached to duplicate items if pre-populated pooling is necessary.
    /// in which case 
    /// 1. alive should set to false
    /// 2. original should point to main prefab reference
    /// </summary>
    public class PooledItem : MonoBehaviour
    {
        public bool alive  = true;
        public int useCount = 0;
        public GameObject original;

        void OnDestroy() { if (alive) RAK.Pool.Destroy(gameObject); }

        void Awake()
        {
            if (!alive && original != null)
            {
                Pool.RegisterToPool(this);
            }
        }
    }

}
