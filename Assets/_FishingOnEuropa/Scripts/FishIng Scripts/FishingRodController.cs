using Obvious.Soap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
        [SerializeField] private Vector3Reference fishingRodEndPointTransform;
        [SerializeField] private Vector3Reference lureEndPointTransform;
        [SerializeField] private FloatReference lureCurrentDistance;
        [SerializeField] private FloatReference lureCurrentMaxDistance;
        [SerializeField] private FloatReference lureMaxDistanceFromRod;

        [SerializeField] private FloatReference rodEndSpeed;
        [SerializeField] private FloatReference castTriggerSpeed;

        [SerializeField] private BoolReference playerCastInput;
        [SerializeField] private BoolReference fishingLineCasted;

        [SerializeField] float lureReturnSpeed = 10f;

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
        [Tooltip("The Rigid body of the fishing Rod")]
        private Rigidbody fishingRodRB;
        [SerializeField]
        [Tooltip("The Minimum amount the transform needs to move by for the direction to be updated")]
        private float minMoveAmount;

        [SerializeField]
        [Tooltip("The Minimum amount the transform needs to move by for the direction to be updated")]
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
            lureCurrentDistance.Value = Vector3.Distance(fishingRodEndPointTransform.Value, lureEndPointTransform.Value);
        }
        [ContextMenu("Casting for pc nerds")]
        public void AutoCast()
        {
            fishingLineCasted.Value = true;
            UpdateLineLength(lureMaxDistanceFromRod.Value);
        }

        private void Cast()
        {
            if (rodEndSpeed > castTriggerSpeed)
            {
                fishingLineCasted.Value = true;
                UpdateLineLength(lureMaxDistanceFromRod.Value);
            }
        }

        private void UpdateLineLength(float _length)
        {
            lureCurrentMaxDistance.Value = _length;
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

        public void OnTriggerPressed(InputAction.CallbackContext _context)
        {
            print("Trigger Pressed");
            float input = _context.ReadValue<float>();

            if (input == 0 && playerCastInput.Value == true)
            {
                Cast();
                playerCastInput.Value = false;
            }
            else if (input > 0 && playerCastInput.Value == false)
            {
                playerCastInput.Value = true;
            }
            else if (input > 0 && fishingLineCasted.Value == true)
            {
                StartCoroutine(LerpLureToRod());
            }
            else if (input == 0)
            {
                playerCastInput.Value = false;
            }

        }

        private IEnumerator LerpLureToRod()
        {
            while (fishingLineCasted.Value == true)
            {
                lureCurrentMaxDistance.Value = Mathf.Lerp(lureCurrentMaxDistance.Value, 0, lureReturnSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

        }
    }
}

