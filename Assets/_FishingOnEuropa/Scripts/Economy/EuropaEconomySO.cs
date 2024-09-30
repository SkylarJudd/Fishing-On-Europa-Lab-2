using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;

namespace Europa
{
    [CreateAssetMenu(fileName = "Economy Scriptable Objects", menuName = "Europa/Economy")]
    public class EuropaEconomySO : ScriptableObject
    {
        [Header("Population")]
        public IntVariable basePopulation;
        public IntVariable currentPopulation;

        [Header("Economy")]
        public IntVariable basePrice;
        public IntVariable currentPrice;
    }
}
