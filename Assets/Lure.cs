using Obvious.Soap;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Europa
{
    public class Lure : MonoBehaviour
    {
        [SerializeField] Vector3Reference fishingRodEndPointTransform;
        [SerializeField] FloatReference LureCurrentDistance;
        [SerializeField] FloatReference LureCurrentMaxDistance;
        [SerializeField] FloatReference LureMaxDistanceFromRod;
        [SerializeField] BoolReference Casted;

        private void Start()
        {

        }

        private void Update()
        {
            
        }
    }
}
