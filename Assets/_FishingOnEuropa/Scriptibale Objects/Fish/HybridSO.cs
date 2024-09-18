using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public enum SpawnLocation { Zone1, Zone2, Zone3 };

    [CreateAssetMenu(fileName = "HybridScriptableObjects", menuName = "Europa/Items/Hybrids", order = 1)]

    public class HybridSO : ScriptableObject
    {

        [Header("Spawning")]
        public PoolType hybridPoolType;
        public ItemRarity rarity;
        public SpawnLocation spawnLocation;
        public int spawnChance;
        public GameObject inWorldVisuals;

        [Header("Fishing MiniGame")]
        public float damageToRod;
        public int catchChance;
        public int restMinTime, restMaxTime;

        [Header("Hybrid AI")]
        public float fishSpeed;
        public float rotationSpeed;
        public FoodType[] foodEaten;
        public FoodList favFood;
        public ToyList favToy;
        public HybridHeadPatTrigger patTrigger;

    }
}

    

