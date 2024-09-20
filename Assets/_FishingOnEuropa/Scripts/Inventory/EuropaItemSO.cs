using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

namespace Europa
{
    [CreateAssetMenu(fileName = "Inventory Item Scribtable Objects", menuName = "Europa/Items/Item", order = 1)]
    public class EuropaItemSO : ScriptableObject
    {

        [Header("SaveData")]
        public int itemID;

        [Header("Europa Item")]
        public FOEItem worldObject;
        public GameObject itemPrefab;
        public string itemName;
        public string itemDescription;

        [Header("Componets")]
        public GameObject itemGO;
        public Rigidbody itemRB;
        public Grabbable itemGrabbable;
        public Transform itemTransform;

        public PoolType poolType;
        public ItemLocation itemLocation;


    }
}

