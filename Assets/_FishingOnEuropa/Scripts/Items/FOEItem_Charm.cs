
using Obvious.Soap;
using UnityEngine;

namespace Europa
{
    [System.Serializable]

    public enum PopulaionZone { Green, Yellow, Red, Purple,Extinct }
    public class FOEItem_Charm : FOEItem
    {
        public ItemRarity rarity;
        public PopulaionZone zone;

        [Header("Population")]
        [SerializeField] private IntVariable basePopulation;
        [SerializeField] private IntVariable currentPopulation;

        [Header("Economy")]
        [SerializeField] private IntVariable basePrice;
        [SerializeField] private IntVariable currentPrice;
    }
}


