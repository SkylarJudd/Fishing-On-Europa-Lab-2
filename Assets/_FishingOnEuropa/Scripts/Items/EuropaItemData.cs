using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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
        public FOEItem itemFOE;

        public ItemLocation itemLocation;

        private void Start()
        {
            InitaliseValues();
        }

        void InitaliseValues()
        {

            itemGO = gameObject;
            itemRB = gameObject.GetComponent<Rigidbody>();
            itemGrabbable = gameObject.GetComponent<Grabbable>();
            itemTransform = gameObject.transform;
        }
    }

    
}
