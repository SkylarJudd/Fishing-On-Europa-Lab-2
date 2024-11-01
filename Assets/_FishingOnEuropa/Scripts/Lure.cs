using Obvious.Soap;
using System;
using System.Collections;
using UnityEngine;

namespace Europa
{
    public class Lure : MonoBehaviour
    {

        [SerializeField] Vector3Reference LureEndPointTransform;
     
        private void Update()
        {
            LureEndPointTransform.Value = transform.position;
        }


    }
}
