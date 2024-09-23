using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    [Serializable]
    public class FOESaveItem
    {
        public int itemID;
        public FOEItem itemGO;
        public Transform itemPos;
        public ItemLocation itemLocation;
        public int itemInventorySlot;

    }
    [Serializable]
    public class FOESaveItem_Hybrid : FOESaveItem
    {
        public string hybridName;
        public bool hybridShiny;
        public int hybridTrust;

        //trust action tracking
        public bool hasBeenPatCurrentDay;
        public bool hasBeenFedCurrentDay;
        public bool hasHadPlushieCurrentDay;
        public bool charmCollectedCurrentDay;

        public float timeWhenCharmSpawned;
    }
    [Serializable]
    public class FOESaveItem_Crop : FOESaveItem
    {
        public CropState growthStage;

    }
}



