using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;

namespace Europa
{
    public class FishingRodFishingMiniGame : MonoBehaviour
    {
        [SerializeField] FloatReference fishingRodHP; //how much hp the rod has 
        [SerializeField] FloatReference fishingEfficency; //the rate at which hybrid stamina is drained per second

    }
}
