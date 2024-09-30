using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;
using equals;

namespace Europa
{
    public class EconomyManager : Singleton<EconomyManager>
    {
        [Header("CharmList")]
        [SerializeField] private ScriptableListFOEItem_Charm[] charm;
    }
}
