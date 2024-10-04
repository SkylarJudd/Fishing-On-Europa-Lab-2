

// Ignore Spelling: bobber no i made it lure :)

using Obvious.Soap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UnityEditor.XR.Management;
using UnityEngine;

namespace Europa
{

    public enum MiniGameState { LureWindrawn, LureCast, LureHitWater, LureReturn, StartMiniGame, RollForHybridCatch,NonCaughtsReturnToSwim, HybridPulling, HybridTied, HybridCaught }
    public enum FishEncounterState { None, Resting, Fighting, Caught }
    public enum PullDirections { NotSet, Left, Right, Middle }

    [Serializable]
    public class FOELure
    {
        [Header("GameObjects")]
        public GameObject lure_Go;
        public GameObject lure_HybridAttachPoint;

        [Header("Position")]
        public Vector3Reference lure_EndPointTransform; // The current Position of the LureEndPoint

        [Header("Distance")]
        public FloatReference Lure_CurrentDistance; // This is the current distance the lure is from the fishing rod end point
        public FloatReference Lure_CurrentMaxDistance; // This is the current Max distance the lure is aloud to be from the rods end point, if the line exceeds this point it will add a force to the lure
        public FloatReference Lure_MaxDistanceFromRod; // This is the Max distance the lure will ever be, if the lure exceeds this value it will add a force in the direction of the fishing rod

        [Header("Casting Forces")]
        public FloatReference Lure_CastingForceMultiplier; // The force multilayer that will be applied to the lure when the lure is casted
        public FloatReference Lure_MaxCastingForce; // The Max amount of force that can be applied to the lure when casting

        [Header("Reeling Forces")]
        public FloatReference Lure_ReelingForceMultiplier; // The force multilayer that will be applied to the lure when the lure is reeled
        public FloatReference Lure_MaxReelingForce; // The Max amount of force that can be applied to the lure when reeling

        [Header("Reset")]
        public FloatReference lure_ResetDistance;  // The distance the lure needs to be from the end of the rod to reset
        public FloatReference lure_CurrentCastResetTime; // The current time elapsed that the reset time has been running for
        public FloatReference lure_CastResetTime; // The max amount of time before the line will reset

        [Header("Lure States")]
        public BoolReference lure_InWater;  // A bool that will be toggled when the lure is in the water, True for in water, and false for not in the water
        public FloatReference lure_InWaterTime; // A counter to track how long the lure has been in the water
        public FloatReference lure_InWaterTimeOut; // the max amount of time the lure can be out of the water after being in the water before it will reset itself
        public BoolReference lure_HybridAttached; //A bool that will be toggled when a hybrid is attached to the lure

        [Header("Mini Game")]
        public FloatReference lure_FishInLureRange;
        public FloatReference lure_StartingAngle;


    }

    [Serializable]
    public class FOERod
    {
        [Header("Position")]
        public Vector3Reference rod_EndPointTransform; // The current Position of the LureEndPoint

        [Header("Rod End Data")]
        public Vector3Reference rod_EndForceDirection; // this keeps track of the direction the rod is traveling in to be used in adding the direction to the lure on cast
        public FloatReference rod_EndCurrentSpeed; // this keeps track of the current speed of the end point of the rod, to be added to the lure on cast. 

        [Header("Player Input")]
        public BoolReference rod_PlayerCastInput; // this keeps track of if the player is holding down the trigger to cast the fishing rod. 

        [Header("MiniGame")]
        public BoolReference rod_RodPullDirection; // The direction the fishing rod is being pulled  true = right , false = left
        public FloatReference rod_fishingRodHP; //how much hp the rod has 
        public FloatReference rod_fishingEfficiency; //the rate at which hybrid stamina is drained per second

        public BoolReference fightingMiniGameActive; //this Bool will be true when the mini game is active
    }

    [Serializable]
    public class FOEReel
    {
        [Header("Rotation")]
        public FloatReference reel_CurrentReelRotation; // the current rotation of the reel in world space
        public FloatReference reel_SmoothedReelRotation; // the current smoothed rotation of the reel in world space
        public FloatReference reel_LastReelRotation; // the last frames rotation of the reel in world space

        [Header("Smoothing")]
        public IntReference smoothingWindow; // how many frames to smooth the rotation over, used to iron out spikes and maintain an average consistent movement 

        [Header("Player Input")]
        public BoolReference reel_PlayersHandOnReel; // keeps track of when the player has grabbed the reel on the fishing rod, true if grabbed false if not. 

    }

    public class FishingMiniGameManager : Singleton<FishingMiniGameManager>
    {
        public MiniGameState fishingMiniGameState;

        [Header("Lure")]
        public FOELure _lure;

        [Header("Rod")]
        public FOERod _rod;

        [Header("Reel")]
        public FOEReel _reel;

        [Header("Hybrids")]
        public LayerMask fishMask;
        [SerializeField] LayerMask terrainLayerMask;
        public Collider[] nearbyHybrids;

        [Header("MiniGame")]


        private Coroutine lineCastCorutine;



        private float time = 0.0f;
        float interpolationPeriod = 1f;

        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    BeginRestingPeriod();
            //}
            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    BeginFightingPeriod();
            //}

            //bobberGameObject.transform.RotateAround(_PLAYER.transform.position, Vector3.up, bobberRotationSpeed * Time.deltaTime);

            //switch (fishingMiniGameState)
            //{
            //    case Europa.MiniGameState.LureWindrawn:

            //    case Europa.MiniGameState.LureCast:




            //    case Europa.MiniGameState.LureHitWater:

            //        if (_FNAVM.caughtHybrid == null)
            //        {
            //            //Get nearby hybrids
            //            nearbyHybrids = Physics.OverlapSphere(_lure.lure_Go.transform.position, _lure.lure_FishInLureRange.Value, fishMask);

            //            if (nearbyHybrids.Length >= 5)
            //            {
            //                //Determine 5 closest hybrids
            //                nearbyHybrids = ReturnClosestHybrids(_lure.lure_Go, nearbyHybrids, 5);

            //            }


            //        }


            //        break;
            //    case Europa.MiniGameState.HybridPulling:

            //        switch (fishEncounterState)
            //        {
            //            case FishEncounterState.Resting:

            //                //add to return to middle

            //                _FNAVM.caughtHybrid.navigationData.hybridRestTime -= Time.deltaTime;

            //                if (_FNAVM.caughtHybrid.navigationData.hybridRestTime <= 0.0f)
            //                {
            //                    //rest time is over
            //                    print("well rested");
            //                    fishEncounterState = FishEncounterState.Fighting;
            //                }
            //                else
            //                {
            //                    ////if handle is moving.Handle should be clamped to only move in circular motion
            //                    //if (rb_reelHandle.velocity.magnitude > 0)
            //                    //{



            //                    //}
            //                    //else
            //                    //{
            //                    //    //if not moving, make idle
            //                    //    if (!_FNAVM.CheckHybridState(targetHybrid, HybridState.HybridIdle))
            //                    //    {
            //                    //        //remove from lists
            //                    //        //_FNAVM.RemoveHybrid(targetHybrid, false);

            //                    //        _FNAVM.UpdateHybridState(targetHybrid, HybridState.HybridIdle);

            //                    //        //remove add to new list
            //                    //        _FNAVM.AddHybridTolist(targetHybrid);
            //                    //    }

            //                    //}

            //                    //check if hybrid is close enough to end
            //                    if (_lure.Lure_CurrentDistance <= _lure.lure_ResetDistance)
            //                    {
            //                        //CAUGHT
            //                        fishEncounterState = FishEncounterState.Caught;


            //                    }

            //                }

            //                break;
            //            case FishEncounterState.Fighting:

            //                if (!_FNAVM.CheckHybridState(_FNAVM.caughtHybrid, HybridState.HybridMiniGame_Pulling))
            //                    _FNAVM.UpdateHybridState(_FNAVM.caughtHybrid, HybridState.HybridMiniGame_Pulling);

            //                //determine pull direction
            //                if (currentPullDirection == PullDirections.NotSet) currentPullDirection = GetDirection();

            //                time += Time.deltaTime;

            //                if (time >= interpolationPeriod)
            //                {
            //                    time = time - interpolationPeriod;
            //                    print("UpdateHybridAndRodDuringFighting");
            //                    //if rod is pulling in other direction
            //                    if (CheckIfPullingInCorrectDirction())
            //                    {
            //                        //deplete stamina each second
            //                        _FNAVM.caughtHybrid.navigationData.currentHybridStamina -= _rod.rod_fishingEfficiency.Value;
            //                        print("Current Hybrid Stamina " + _FNAVM.caughtHybrid.navigationData.currentHybridStamina);
            //                        if (_FNAVM.caughtHybrid.navigationData.currentHybridStamina <= 0.0f)
            //                        {
            //                            //if stamina = 0 switch to resting
            //                            BeginRestingPeriod();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        //else damage rod
            //                        _rod.rod_fishingRodHP.Value = _rod.rod_fishingRodHP.Value - _FNAVM.caughtHybrid.hybridSO.damageToRod;
            //                        print("Current Rod HP " + _rod.rod_fishingRodHP.Value);

            //                        //if rod hp = 0

            //                        if (_rod.rod_fishingRodHP.Value <= 0.0f)
            //                        {
            //                            //escaped
            //                            _FNAVM.UpdateHybridState(_FNAVM.caughtHybrid, HybridState.HybridMiniGame_Escaped);


            //                            fishEncounterState = FishEncounterState.None;

            //                        }
            //                    }

            //                }


            //                break;
            //        }


            //        break;

            //        /*  1. make sure updteMiniGame is called in start
            //         *  2. set _FNAVM.caughtHyrbid = targetHybridNavData
            //         *  3. to change fish state _FNVAM.caughtHybrid.hybrid state > update that state
            //         *  4. caught fish movement for fish encounter is in _FNAVM.UpdateMiniGame
            //         *  
            //         *  END MINIGAME IENUMRATOR WHEN FISHING IS DONE
            //         */

            //}


        }

        //public enum MiniGameState { LureWindrawn, LureCast, LureHitWater, LureReturn, StartMiniGame, HybridPulling, HybridTied, HybridCaught }
        public void OnLineCast()
        {
            updateMiniGameState(MiniGameState.LureCast);
            lineCastCorutine = StartCoroutine(LineCastCoroutine());
            // Start a timer, if the lure dose not enter the water in a set amount of time, it will return. 
        }
        public void OnLureHitWater()
        {
            updateMiniGameState(MiniGameState.LureHitWater);
            // Will start a timer to make sure the lure stays in the water if it dose not it will restart the timer. 
        }
        public void OnLureReturnToRod()
        {
            updateMiniGameState(MiniGameState.LureReturn);
            // Will start a coroutine reducing the current max distance until the lure arrives back to the rod. 
        }
        public void OnLureArriveAtRod()
        {
            updateMiniGameState(MiniGameState.LureWindrawn);
            // Will Reset the mini Game and make sure everything is set back to its starting values. 
        }
        public void OnStartMiniGame()
        {
            updateMiniGameState(MiniGameState.StartMiniGame);
            // Will cause the 5 closest hybrids to swim towards the lure and wait for them to send a message saying they have arrived
        }
        public void OnHybridsArrived()
        {
            updateMiniGameState(MiniGameState.RollForHybridCatch);
            // Once all the hybrids have arrived it Will start a Coroutine that will visually show each of the hybrids taking a bite of the bait in ascending order based on rarity. 
        }
        public void OnHybridAssignedToCatch()
        {

            updateMiniGameState(MiniGameState.NonCaughtsReturnToSwim);
            // Once a hybrid has been assigned, It will loop through the rest of the hybrids and return them to swimming, and set the caught hybrid to pulling
        }
        public void OnHybridTried()
        {
            updateMiniGameState(MiniGameState.HybridTied);
            // will make the caught hybrid follow the lure transform
            // and start a timer to check when the hybrid is no longer tired. 
            // hybrid will stop being tired when the timer runs out or when the current distance = the starting distance / 3 then * 2 and 1 so the player has to fight the fish 3 times before catching
            // no matter the distance. 
        }
        public void OnHybridPulling()
        {
            updateMiniGameState(MiniGameState.HybridPulling);
            // will make the fish pull left and right and compare the players movements left and right, if the player is pulling in the incorrect direction then it will add
            // damage to the fishing rod. 
        }
        public void OnHybridCaught()
        {
            updateMiniGameState(MiniGameState.LureWindrawn);
            // Will need to set the hybrid state to be an item and remove it from the navigation list. 
        }


        private void updateMiniGameState(MiniGameState _State)
        {
            fishingMiniGameState = _State;
        }


        private IEnumerator LineCastCoroutine()
        {
            _lure.lure_CurrentCastResetTime.Value = 0;

            while (fishingMiniGameState == MiniGameState.LureCast)
            {
                _lure.lure_CurrentCastResetTime.Value += Time.deltaTime;

                if (_lure.lure_CurrentCastResetTime.Value >= _lure.lure_CastResetTime)
                {
                    OnLureReturnToRod();
                    StopCoroutine(lineCastCorutine);
                }


                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LureHitWaterCoroutine()
        {
            _lure.lure_InWaterTime.Value  = 0;

            while ( fishingMiniGameState == MiniGameState.LureHitWater)
            {
                if (_lure.lure_InWater == true)
                {
                    _lure.lure_InWaterTime.Value += Time.deltaTime;
                }

                yield return new WaitForEndOfFrame();
            }
        }





        /// <summary>
        /// Subscribe to OnLureHitWater event on Lure
        /// </summary>
        public void LureHitWater()
        {
            fishingMiniGameState = Europa.MiniGameState.LureHitWater;
        }

        /// <summary>
        /// Set rest period time and change fishEncounterState
        /// </summary>
        public void BeginRestingPeriod()
        {

            _FNAVM.caughtHybrid.navigationData.currentHybridStamina = _FNAVM.caughtHybrid.hybridSO.stamina;


            _FNAVM.caughtHybrid.navigationData.hybridRestTime = UnityEngine.Random.Range(_FNAVM.caughtHybrid.hybridSO.restMinTime, _FNAVM.caughtHybrid.hybridSO.restMaxTime);

            //while reel is moving, change target fish to move to lure
            _FNAVM.removeHybrid(_FNAVM.caughtHybrid, false);

            _FNAVM.UpdateHybridState(_FNAVM.caughtHybrid, HybridState.HybridMiniGame_Tired);

            //remove from swim list and put in swimToPoint list
            _FNAVM.AddHybridTolist(_FNAVM.caughtHybrid);

            print(_FNAVM.caughtHybrid.navigationData.hybridRestTime);
            _rod.fightingMiniGameActive.Value = false;

        }

        public void BeginFightingPeriod()
        {



            print("BeginFightingPeriod");

            _rod.fightingMiniGameActive.Value = true;

            var targetDir = _rod.rod_EndPointTransform - _lure.lure_HybridAttachPoint.transform.position;
            _lure.lure_StartingAngle.Value = Vector3.Angle(targetDir, _PLAYER.gameObject.transform.forward);


        }

        public void ConfirmMiniGameStart()
        {
            //CHANGE INPUT HERE - commit to catching fish (or pull rod)
            if (nearbyHybrids.Length > 0 && fishingMiniGameState == Europa.MiniGameState.LureHitWater)
            {
                _FNAVM.caughtHybrid = CalculateHybridWithHighestCatchChance(nearbyHybrids);

                _FNAVM.caughtHybrid.hybridSO = _FNAVM.caughtHybrid.GetComponent<HybridInfo>().hybridInfo;

                _FNAVM.removeHybrid(_FNAVM.caughtHybrid, false);

                _FNAVM.UpdateHybridState(_FNAVM.caughtHybrid, HybridState.HybridMiniGame_SwimToLure);

                //remove from swim list and put in swimToPoint list
                _FNAVM.AddHybridTolist(_FNAVM.caughtHybrid);

            }
        }

        /// <summary>
        /// Generate pull direction which does not collide with terrain
        /// </summary>
        /// <returns></returns>
        public PullDirections GetDirection()
        {
            List<PullDirections> viableDirection = new();

            viableDirection.Add(PullDirections.Right);
            //viableDirection.Add(PullDirections.Middle);
            viableDirection.Add(PullDirections.Left);


            ////check middle for collision with raycast
            //if (!Physics.Raycast(bobberTipGO.transform.position, Vector3.back, 3f, terrainLayerMask))
            //    viableDirection.Add(PullDirections.Middle);

            ////check left for collision with raycast
            //if (!Physics.Raycast(bobberTipGO.transform.position, Vector3.right, 3f, terrainLayerMask))
            //    viableDirection.Add(PullDirections.Left);

            ////check left for collision with raycast
            //if (!Physics.Raycast(bobberTipGO.transform.position, Vector3.left, 3f, terrainLayerMask))
            //    viableDirection.Add(PullDirections.Right);

            return viableDirection[UnityEngine.Random.Range(0, viableDirection.Count)];

        }

        public bool CheckForHybridCollision(Vector3 direction)
        {
            if (!Physics.Raycast(_lure.lure_HybridAttachPoint.transform.position, direction, 3f, terrainLayerMask))
            {
                return true; //no collision
            }
            else return false; //there is a collision
        }

        /// <summary>
        /// Reset variables to null or default to prepare for next catch
        /// </summary>
        void ResetVariables()
        {
            _FNAVM.caughtHybrid.navigationData.hybridRestTime = 0;
            _rod.fightingMiniGameActive.Value = false;
            _lure.lure_StartingAngle.Value = 0;

        }

        /// <summary>
        /// Return a given amount of hybrids which are closest to the center
        /// </summary>
        /// <param name="_fullHybridList">Inital hybrids collider list</param>
        /// <param name="_returnListLength"> How many hybrids will be returned</param>
        /// <returns></returns>
        Collider[] ReturnClosestHybrids(GameObject center, Collider[] _fullHybridList, int _returnListLength)
        {
            List<Collider> newHybridsList = new List<Collider>();


            //add first hybrid for comparisons
            newHybridsList.Add(_fullHybridList[0]);
            float maxHybridDistance = Vector3.Distance(center.transform.position, _fullHybridList[0].transform.position);
            int maxHybridIndex = 0;

            //only add up to returnListLength
            for (int i = 1; i < _fullHybridList.Length; i++)
            {
                //add hybrids no matter distance
                if (newHybridsList.Count < _returnListLength)
                {
                    newHybridsList.Add(_fullHybridList[i]);

                }
                //hybrids distance is less than current max, so replace current max
                else if (Vector3.Distance(center.transform.position, _fullHybridList[i].transform.position) < maxHybridDistance)
                {
                    //remove old max
                    newHybridsList.RemoveAt(maxHybridIndex);

                    //add new hybrids
                    newHybridsList.Add(_fullHybridList[i]);

                    //create temp max distance to compare too
                    var tempMax = Vector3.Distance(center.transform.position, newHybridsList[0].transform.position);

                    //find new max
                    foreach (var hybrid in newHybridsList)
                    {
                        if (Vector3.Distance(center.transform.position, hybrid.transform.position) > tempMax)
                        {
                            tempMax = Vector3.Distance(center.transform.position, hybrid.transform.position);
                            maxHybridIndex = newHybridsList.IndexOf(hybrid);
                            maxHybridDistance = tempMax;
                        }
                    }


                }

            }

            return newHybridsList.ToArray();


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_hybridColliderArray"></param>
        /// <returns></returns>
        FOEItem_Hybrid CalculateHybridWithHighestCatchChance(Collider[] _hybridColliderArray)
        {

            //get all hybrid catch chances
            List<float> hybridCatchChances = new();

            //percentage to spawn in this group
            List<float> hybridCatchChancePercentage = new();

            //number generation list
            List<int> generateNumber100List = new();

            //total of all catch chances
            int catchChanceTotal = 0;


            foreach (var hybrid in _hybridColliderArray)
            {
                hybridCatchChances.Add(hybrid.GetComponent<HybridInfo>().hybridInfo.catchChance);
                //print("hybrid catch chance is " + hybrid.GetComponent<HybridInfo>().hybridInfo.catchChance);
                catchChanceTotal += hybrid.GetComponent<HybridInfo>().hybridInfo.catchChance;
            }

            //calculate percentage
            foreach (var catchChance in hybridCatchChances)
            {
                float result = catchChance / catchChanceTotal;
                print(result);
                result = result * 100;
                hybridCatchChancePercentage.Add(Mathf.RoundToInt(result));

            }

            for (int i = 0; i < hybridCatchChances.Count; i++)
            {
                //add hybridCatchChance number to generateNumber100List hybridCatchChancePercentage amount of times
                for (int j = 0; j < hybridCatchChancePercentage[i]; j++)
                {
                    generateNumber100List.Add((int)hybridCatchChances[i]);
                }
            }

            //generate number
            int hybridCatchChanceChosen = generateNumber100List[UnityEngine.Random.Range(0, generateNumber100List.Count)];

            GameObject chosenHybrid = _hybridColliderArray[hybridCatchChances.IndexOf(hybridCatchChanceChosen)].gameObject;

            return _FNAVM.GetHybridFromGO(chosenHybrid);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_lure.lure_Go.transform.position, _lure.lure_FishInLureRange.Value);


        }

       
    }
}


