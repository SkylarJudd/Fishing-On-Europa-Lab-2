using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public enum HybridState
    {
        HybridIdle,
        HybridFlying,
        HybridHitWater,
        HybridFlocking,
        HybridAvoidingWall,

        HybridMiniGame_SwimToLure,
        HybridWaitForMiniGame,
        HybridMiniGame_Pulling,
        HybridMiniGame_Tired,
        HybridMiniGame_Caught,
        HybridMiniGame_Escaped,

        HybridOnLand_Sitting,
        HybridOnLand_Walking,
        HybridSwimToItem,
        HybridLookAtHand,
        HybridWatchPlayer,

    }

   

    [Serializable]
    public class HybridNavigationData : MonoBehaviour
    {
        [Header("Hybrid Nav")]

        public HybridState hybridState;
        public ItemLocation hybridLocation;
        public Vector3 velocity;

        public float minSpeed;
        public float maxSpeed;
        public float waterHeight;

        public bool isTurning;
        public bool aboutToHitWall = false;
        public bool firstNav;

        [Header("Hybrid Player Interact")]
        public float distanceToPlayer;
        public float distanceToLeftHand;
        public float distanceToRightHand;

        [Header("Hybrid MiniGame")]
        public bool arrivedAtLure = false;
        public float hybridRestTime;
        public float currentHybridStamina;
        public float scaledCatchChance;



        public Transform itemTarget; // this is used for the item target you can set the lure, 

        public bool shiny;


    }
}


