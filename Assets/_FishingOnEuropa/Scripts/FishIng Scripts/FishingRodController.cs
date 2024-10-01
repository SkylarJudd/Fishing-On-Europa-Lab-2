using Obvious.Soap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public enum RodDirectionLR
    {
        RodNutral, RodLeft, RodRight
    }
    public enum RodDirectionFB
    {
        RodNutral, RodBack, RodForward
    }

    public class FishingRodController : Singleton<FishingRodController>
    {
        [Header("Line Properties")]
        [SerializeField] Vector3Reference fishingRodEndPointTransform;
        [SerializeField] Vector3Reference LureEndPointTransform;
        [SerializeField] FloatReference LureCurrentDistance;
        [SerializeField] FloatReference LureCurrentMaxDistance;
        [SerializeField] FloatReference LureMaxDistanceFromRod;
        [SerializeField] BoolReference Casted;

        

        [Header("FishingRod")]
        [SerializeField]
        [Tooltip("The GameObject Of the Fishing Rod")]
        private GameObject fishingRod;
        [SerializeField]
        [Tooltip("The Current direction of the fishing rod moving")]
        private RodDirectionLR rodDirectionLeftRight = RodDirectionLR.RodNutral;
        [SerializeField]
        [Tooltip("The Current direction of the fishing rod moving")]
        private RodDirectionFB rodDirectionForwardBack = RodDirectionFB.RodNutral;

        [SerializeField]
        [Tooltip("The Current direction of the fishing rod moving")]
        private Vector2 rodDirectionVector;

        [SerializeField]
        [Tooltip("The Rigidbody of the fishing Rod")]
        private Rigidbody fishingRodRB;
        [SerializeField]
        [Tooltip("The end transform of the fishing Rod")]
        private Vector3Reference endPointTransform;
        [SerializeField]
        [Tooltip("The Minium amout the transform needs to move by for the direction to be updated")]
        private float minMoveAmount;

        [SerializeField]
        [Tooltip("The Minium amout the transform needs to move by for the direction to be updated")]
        private FishingMiniGameManager _FMGM;

        float CurrentReelRotation = 0;
        private Vector3 lastRodTransform;

        private void Start()
        {
            lastRodTransform = fishingRod.transform.localPosition;
        }
        private void Update()
        {
            CaculateDistance();
            //UpDateRodDiretion();
        }

        private void CaculateDistance()
        {
            LureCurrentDistance.Value = Vector3.Distance(fishingRodEndPointTransform.Value, LureEndPointTransform.Value);
        }

        private void Cast()
        {

        }

        private void UpdateLineLength(float _length)
        {

        }

        private void UpDateRodDiretion()
        {
            if (_FMGM.bobberState == FishingMiniGameManager.BobberState.AttachedFish)
            {
                if (Vector3.Distance(lastRodTransform, fishingRod.transform.localPosition) > minMoveAmount)
                {
                    (rodDirectionLeftRight, rodDirectionForwardBack) = CaculateDirection(fishingRod.transform.position, lastRodTransform);
                    lastRodTransform = fishingRod.transform.position;
                }
            }
        }

        private (RodDirectionLR, RodDirectionFB) CaculateDirection(Vector3 p1, Vector3 p2)
        {
            float leftRightVector = p1.x > p2.x ? 1 : 0;
            RodDirectionLR returnLR = leftRightVector == 1 ? RodDirectionLR.RodLeft : RodDirectionLR.RodRight;
            float forwardBackVector = p1.z > p2.z ? 1 : 0;
            RodDirectionFB returnFW = forwardBackVector == 1 ? RodDirectionFB.RodForward : RodDirectionFB.RodBack;

            rodDirectionVector = new Vector2(leftRightVector, forwardBackVector);
            return (returnLR, returnFW);
        }
    }
}

