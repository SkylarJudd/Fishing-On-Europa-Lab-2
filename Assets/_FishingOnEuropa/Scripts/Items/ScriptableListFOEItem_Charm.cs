using UnityEngine;
using Obvious.Soap;
using Europa;


namespace equals
{
    [CreateAssetMenu(fileName = "scriptable_list_" + nameof(FOEItem_Charm), menuName = "Soap/ScriptableLists/" + nameof(FOEItem_Charm))]
    public class ScriptableListFOEItem_Charm : ScriptableList<FOEItem_Charm>
    {
        [Header("Enums")]
        [SerializeField] private ScriptableEnumFOECharm_Rarity rarityType = null;
        public ScriptableEnumFOECharm_Rarity RarityType => rarityType;
        public PopulaionZone populaionZone;

        [Header("Population")]
        public IntVariable currentPopulation;

        [Header("Economy")]
        public IntVariable currentPrice;
        public float priceAdjustmentVariable;

        public void CalculatePopulationZone()
        {
            var populationZoneValue = currentPopulation/rarityType.basePopulation;

            if(populationZoneValue > 1.2f)
            {
                populaionZone = PopulaionZone.Purple;
            }

            if(populationZoneValue < .6)
            {
                if(populationZoneValue < .4)
                {
                    populaionZone = PopulaionZone.Red;
                }                
                populaionZone = PopulaionZone.Yellow;
            }
        }
    }
}

