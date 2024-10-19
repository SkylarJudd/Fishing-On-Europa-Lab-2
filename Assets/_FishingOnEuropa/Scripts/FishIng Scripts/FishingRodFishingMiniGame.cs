using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;
using Autohand;

namespace Europa
{
    public class FishingRodFishingMiniGame : GameBehaviour
    {
        [SerializeField] FloatReference fishingRodHP; //how much hp the rod has 
        [SerializeField] FloatReference fishingEfficency; //the rate at which hybrid stamina is drained per second

        [SerializeField] private bool _heldInLeftHand;
        [SerializeField] private bool _heldInRightHand;

        /// <summary>
        /// This is a function that is called from the grabable Event that is found on the RodBase when the item is picked up
        /// </summary>
        /// <param name="_hand"></param>
        /// <param name="_grabbable"></param>
        public void OnPickup(Hand _hand, Grabbable _grabbable)
        {
            _FMGM._rod.rod_Held = true;

            if(_hand.left == true)
                _heldInLeftHand = true;
            else
                _heldInRightHand = true;

            _FMGM._rod.rod_Hand = _hand.left ? HandEnum.LeftHand : HandEnum.RightHand;
        }

        /// <summary>
        /// This is a function that is called from the grabable Event that is found on the RodBase when the Item is dropped
        /// </summary>
        /// <param name="_hand"></param>
        /// <param name="_grabbable"></param>
        public void OnDrop(Hand _hand, Grabbable _grabbable)
        {
            if (_hand.left == true)
            {
                _heldInLeftHand = false;
                _FMGM._rod.rod_Hand = HandEnum.RightHand;
            }
            else
            {
                _heldInRightHand = false;
                _FMGM._rod.rod_Hand = HandEnum.LeftHand;
            }
                

            if (_heldInRightHand == false && _heldInLeftHand == false)
                _FMGM._rod.rod_Held = false;

        }

    }
}
