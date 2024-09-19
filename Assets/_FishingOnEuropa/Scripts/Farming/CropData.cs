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

    public class CropData : MonoBehaviour
    {
        public GameObject go;
        public int itemID;
        public SeedsSO seedSO;
        public Transform itemTransform;
        public CropState cropState;
        public int farmIndex;
    }
}
