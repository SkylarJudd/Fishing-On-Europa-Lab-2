using UnityEngine;
using Obvious.Soap;
using System.Security.Cryptography;
using Unity.VisualScripting;

namespace Europa
{
    [CreateAssetMenu(fileName ="scriptable_enum_FOECharm_Rarity", menuName = "Soap/ScriptableEnums/FOECharm_Rarity")]
    public class ScriptableEnumFOECharm_Rarity : ScriptableEnumBase
    {
        [SerializeField] private IntVariable basePriceVar;
        public int basePrice;
        [SerializeField] private IntVariable basePopulatonVar;
        public int basePopulation;

        public void Start()
        {
            basePrice = basePriceVar.Value;
            basePopulation = basePopulatonVar.Value;
        }
    }
}
