using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    [CreateAssetMenu(fileName = "Seeds", menuName = "Europa/Farming/Seeds", order = 1)]

    public class SeedsSO : ScriptableObject
    {
        public int ItemID;
        public float growthTime;

        public FOEItem foodProduced;

        [Header("Plant Stages")]
        public GameObject[] growthStages;
        public FoodList foodItem;


    }
}

