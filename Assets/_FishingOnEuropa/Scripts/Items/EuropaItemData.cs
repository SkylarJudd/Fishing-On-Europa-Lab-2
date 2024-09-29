using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    [Serializable]
    public class EuropaItemData : MonoBehaviour
    {
        public string itemName;

        [Header("Componets")]
        public GameObject itemGO;
        public Rigidbody itemRB;
        public Grabbable itemGrabbable;
        public Transform itemTransform;

        public ItemLocation itemLocation;
    }
}
