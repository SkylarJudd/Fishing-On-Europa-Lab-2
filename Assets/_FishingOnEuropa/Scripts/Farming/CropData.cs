using Europa;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public enum CropState
    {
        Seed, Sprout, Flowering, Fruited, Harvested
    }

    public class FOEItem_Crop : FOEItem
    {
        public GameObject go;
        public SeedsSO seedSO;
        public Transform itemTransform;
        public CropState cropState;
        public int farmIndex;

        public GrowthDataSO growthData;
    }
}
