using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;

namespace Europa
{
    public class EconomyManager : Singleton<EconomyManager>
    {
        [Header("CharmList")]
        [SerializeField] private ScriptableListBase charms;
    }
}
