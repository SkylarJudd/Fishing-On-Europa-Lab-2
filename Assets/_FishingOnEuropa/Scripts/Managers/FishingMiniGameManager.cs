

// Ignore Spelling: bobber no i made it lure :)

using Obvious.Soap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI.BodyUI;
using static Europa.FishSpawnerGeyser;

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
        public FloatReference lure_HybridToLureResetTime; // The max amount of time before the line will reset

        [Header("Lure States")]
        public BoolReference lure_InWater;  // A bool that will be toggled when the lure is in the water, True for in water, and false for not in the water
        public FloatReference lure_InWaterTime; // A counter to track how long the lure has been in the water
        public FloatReference lure_InWaterTimeOut; // the max amount of time the lure can be out of the water after being in the water before it will reset itself
        public BoolReference lure_HybridAttached; //A bool that will be toggled when a hybrid is attached to the lure

        [Header("Mini Game")]
        public FloatReference lure_FishInLureRange;  // A sphere that keeps will find all the hybrids inside
        public FloatReference lure_StartingAngle;  // The starting angle of the fishing rod mini game
        public FloatReference lure_MaxAngle;  // The angle the lure can rotate
        public FloatReference lure_LureRotationSpeed;  // The rotation speed of the lure during the mini game
        public FloatReference lure_LureStartingDistance;  // The rotation speed of the lure during the mini game
        public List<float> lure_MiniGameDistancePoints;


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
        [SerializeField] private int MaxHybridsToFind = 5;
        [SerializeField] private int HybridCurrentFightAttempts = 0;
        [SerializeField] private int HybridMaxFightAttempts = 3;
        [SerializeField] private Vector3 lureRotationAxis;
        [SerializeField] public List<FOEItem_Hybrid> hybridsReachedLure;



        [Header("Coroutines")]
        private Coroutine lineCastCorutine;
        private Coroutine lureHitWaterCoroutine;
        private Coroutine startMiniGameCorutine;
        private Coroutine restingPeriodCoroutine;



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
            if(fishingMiniGameState == MiniGameState.LureWindrawn)
            {
                updateMiniGameState(MiniGameState.LureCast);
                lineCastCorutine = StartCoroutine(LineCastCoroutine());
                // Start a timer, if the lure dose not enter the water in a set amount of time, it will return. 
            }
        }
        public void OnLureHitWater()
        {
            if(fishingMiniGameState == MiniGameState.LureCast)
            {
                updateMiniGameState(MiniGameState.LureHitWater);
                lureHitWaterCoroutine = StartCoroutine(LureHitWaterCoroutine());
                // Will start a timer to make sure the lure stays in the water if it dose not it will restart the timer. 
            }
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
            startMiniGameCorutine = StartCoroutine(StartMiniGameCoroutine());
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
            OnHybridPulling();
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

        /// <summary>
        /// A function that updates the state of the mini game to the inputted state
        /// </summary>
        /// <param name="_State"></param>
        private void updateMiniGameState(MiniGameState _State)
        {
            fishingMiniGameState = _State;
        }

        /// <summary>
        /// a Function that is used to check how long the lure has been in its casting state, aka not hit the water, if the lure dose not come into contact with water it will return to the fishing rod
        /// </summary>
        /// <returns></returns>
        private IEnumerator LineCastCoroutine()
        {
            _lure.lure_CurrentCastResetTime.Value = 0;  //sets the value to 0 so this function is self resetting

            while (fishingMiniGameState == MiniGameState.LureCast)  // Keeps looping while the lure has been casted and has not hit the water
            {
                _lure.lure_CurrentCastResetTime.Value += Time.deltaTime;  // Adds to CurrentCastResetTime timer every frame

                if (_lure.lure_CurrentCastResetTime.Value >= _lure.lure_CastResetTime) // check to see if the CurrentCastResetTime has exceeded the CastResetTime and if so will return the lure to the rod
                {
                    OnLureReturnToRod();
                    StopCoroutine(lineCastCorutine);
                }


                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// This function is used to check of the lure has settled in the water or has bounced out, if the lure has bounced it it has _lure.lure_CastResetTime.Value amount of time to get back in the water
        /// </summary>
        /// <returns></returns>
        private IEnumerator LureHitWaterCoroutine()
        {
            _lure.lure_InWaterTime.Value = 0;  // sets the value back to 0 so this function is self resetting

            while (fishingMiniGameState == MiniGameState.LureHitWater)  // Keeps looping while the lure is in the state of hit water. 
            {
                if (_lure.lure_InWater == true)   // This will be set to true when the lure is in a water source
                {
                    _lure.lure_InWaterTime.Value += Time.deltaTime;
                    if (_lure.lure_InWaterTime.Value >= _lure.lure_CastResetTime.Value)
                    {
                        OnStartMiniGame();
                        StopCoroutine(lureHitWaterCoroutine);
                    }
                }
                else if (_lure.lure_InWater == false)  // This will be set to false when the lure is not in a water source
                {
                    _lure.lure_InWaterTime.Value -= Time.deltaTime;
                    if (_lure.lure_InWaterTime.Value <= -_lure.lure_CastResetTime.Value)
                    {
                        OnLureReturnToRod();
                        StopCoroutine(lureHitWaterCoroutine);
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// This Function Will start the mini game set all the values that are needed and then find the 5 closest hybrids to the lure then wait till they have all arrived at the lure
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartMiniGameCoroutine()
        {
            SetUpMiniGame();

            List<FOEItem_Hybrid> _convertedHybridList = ConvertToNormalList(_FNAVM._hybridsToNavList); // converts the hybrid nav list to a normal list so we can use sorting functions

            if (_convertedHybridList == null)
            {
                Debug.LogError($"No hybrids where found in the list, make sure hybrids have spawned before starting the mini game, if hybrids are spawned something has gone wrong");
                OnLureReturnToRod();
                StopCoroutine(startMiniGameCorutine);
            }
            else
            {
                

                List<FOEItem_Hybrid> _foundHybrids = GetClosestHybrids(_convertedHybridList); //gets the 5 closest hybrids in the list

                hybridsReachedLure.Clear();  // sets the hybrid reached nav points back to 0 so its self resetting, this value is iterated each time a hybrid reaches its destination. 
                float HybirdReachedNavPointTimeout = 0;

                foreach (FOEItem_Hybrid _hybrid in _foundHybrids)
                {
                    //remove Hybrid From any lists
                    _FNAVM.removeHybrid(_hybrid, false);

                    //Set Hybrid to nav point
                    _FNAVM.AssignHybridWithTargetAndState(_hybrid, _lure.lure_HybridAttachPoint.transform);

                }

                while (hybridsReachedLure.Count < _convertedHybridList.Count || HybirdReachedNavPointTimeout < _lure.lure_HybridToLureResetTime.Value)
                {

                    HybirdReachedNavPointTimeout += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                if (hybridsReachedLure.Count != 0)
                {
                    OnHybridsArrived();
                    StopCoroutine(startMiniGameCorutine);
                }
                else
                {
                    Debug.LogWarning($"was not able to find any hybrids, if there are hybrid spawned, then something is going very wrong");
                    OnLureReturnToRod();
                    StopCoroutine(startMiniGameCorutine);
                }
            }
        }

        /// <summary>
        /// Converts the ScriptableListFOEItem_Hybrid to a normal list so we can use sorting functions bc soap lists done have sorting for some reason reeeee
        /// </summary>
        /// <param name="_HybridList"></param>
        /// <returns></returns>
        private List<FOEItem_Hybrid> ConvertToNormalList(ScriptableListFOEItem_Hybrid _HybridList)
        {
            List<FOEItem_Hybrid> _hybridList = new List<FOEItem_Hybrid>();

            foreach (FOEItem_Hybrid _hybrid in _HybridList)
            {
                _hybridList.Add(_hybrid);
            }

            return _hybridList;
        }

        /// <summary>
        /// A function that is called at the start of the mini game for setting any values that need to be reset before the game starts
        /// </summary>
        private void SetUpMiniGame()
        {
            _lure.lure_MiniGameDistancePoints.Clear();
            HybridCurrentFightAttempts = 0;

            // Calculate divisions of the starting distance
            float division = _lure.lure_LureStartingDistance / HybridMaxFightAttempts;

            // Assign calculated points to the list dynamically based on HybridMaxFightAttempts
            for (int i = HybridMaxFightAttempts; i > 0; i--)
            {
                _lure.lure_MiniGameDistancePoints.Add(division * i);
            }
            _lure.lure_MiniGameDistancePoints.Add(0);
        }

        /// <summary>
        /// Finds the 5 closest FOEItem_Hybrids and returns them as a list.
        /// </summary>
        /// <param name="_hybrids">List of FOEItem_Hybrid objects to check distance from.</param>
        /// <returns>List of 5 closest FOEItem_Hybrid objects.</returns>
        private List<FOEItem_Hybrid> GetClosestHybrids(List<FOEItem_Hybrid> _Hybrid)
        {
            // Check if there are fewer than 5 hybrids to avoid unnecessary processing
            if (_Hybrid.Count <= MaxHybridsToFind)
                return new List<FOEItem_Hybrid>(_Hybrid);

            // Sort the hybrids based on distance to the lure's end point
            _Hybrid.Sort((a, b) =>
                Vector3.Distance(_lure.lure_EndPointTransform.Value, a.europaItemData.itemTransform.position)
                .CompareTo(Vector3.Distance(_lure.lure_EndPointTransform.Value, b.europaItemData.itemTransform.position))
            );

            // Return the top 5 closest hybrids
            return _Hybrid.GetRange(0, 5);
        }

        /// <summary>
        /// This function is used calculate the caught hybrid, by scaling all their catch chances between 1 - 100, it then orders them and begins counting up an int. Hybrids will return to swimming,
        /// their number is passed, but if it stops on their number then that will be the caught hybrid. 
        /// </summary>
        /// <returns></returns>
        private IEnumerator HyrbidCatchRoll()
        {
            // use the list of the hybrids attached to the lure to calculate the catch change of each of them. 
            CaculateCatchChanceList();
            // get a random number between 0 - 100
            int randomNumber = UnityEngine.Random.Range(1, 100);

            // work out what hybrid will be caught based off this number
            float total = 0;
            int hybridsToAnimate = 0;
            foreach (FOEItem_Hybrid _hybrid in hybridsReachedLure)
            {
                total += _hybrid.navigationData.scaledCatchChance;
                hybridsToAnimate++;

                if (total > randomNumber)
                {
                    _FNAVM.caughtHybrid = _hybrid;
                    break;
                }
            }
            total = hybridsReachedLure[0].navigationData.scaledCatchChance;
            int index = 0;

            for (int i = 1; i < randomNumber; i++)
            {
                if (i > total)
                {
                    //stop the animation the hybrid sussing out the lure for the current index
                    //return the hybrid to swimming and reset its values
                    ResetHybrid(hybridsReachedLure[index]);

                    index++;
                    total += hybridsReachedLure[index].navigationData.scaledCatchChance;
                }

                // animate the current index hybrid to look like its sussing out the  lure
                yield return new WaitForSeconds(0.1f);
            }

            for (int i = index +1; i < hybridsReachedLure.Count; i++ )
            {          
                    ResetHybrid(hybridsReachedLure[i]);
            }

            OnHybridAssignedToCatch();
        }

        /// <summary>
        /// a function that resets the hybrids before the return to swimming
        /// </summary>
        /// <param name="_Hybrid"></param>
        private void ResetHybrid(FOEItem_Hybrid _Hybrid)
        {
            _Hybrid.navigationData.scaledCatchChance = 0;
            _Hybrid.navigationData.arrivedAtLure = false;
            _Hybrid.navigationData.firstNav = true;
            _Hybrid.navigationData.itemTarget = null;

            _FNAVM.removeHybrid(_Hybrid, false);
            _FNAVM.AddHybridTolist(_Hybrid, HybridState.HybridFlocking);

        }

        /// <summary>
        /// This sets the catch chances for the hybrids in hybridsReachedLure to be between 0 - 100 and then orders them in a list
        /// </summary>
        private void CaculateCatchChanceList()
        {
            int total = 0;

            foreach (FOEItem_Hybrid _hybrid in hybridsReachedLure)
            {
                total += _hybrid.hybridSO.catchChance;
            }

            foreach (FOEItem_Hybrid _hybrid in hybridsReachedLure)
            {
                _hybrid.navigationData.scaledCatchChance = (_hybrid.hybridSO.catchChance / total) * 100;
            }

            hybridsReachedLure.Sort((x, y) => x.navigationData.scaledCatchChance.CompareTo(y.navigationData.scaledCatchChance));
        }



        /// <summary>
        /// Set rest period time and change fishEncounterState
        /// </summary>
        private IEnumerator RestingPeriodCoroutine()
        {
            Debug.Log("Starting Hybrid Resting");

            // Set the current Stamina to the hybrids max stamina;
            _FNAVM.caughtHybrid.navigationData.currentHybridStamina = _FNAVM.caughtHybrid.hybridSO.stamina;
            // Set the rest time to a value between the hybrids max rest time and their min rest time
            _FNAVM.caughtHybrid.navigationData.hybridRestTime = UnityEngine.Random.Range(_FNAVM.caughtHybrid.hybridSO.restMinTime, _FNAVM.caughtHybrid.hybridSO.restMaxTime);
            //remove from all lists and put in Mini Game List 
            _FNAVM.removeHybrid(_FNAVM.caughtHybrid, false);
            _FNAVM.AddHybridTolist(_FNAVM.caughtHybrid, HybridState.HybridMiniGame_Tired);
            //Sets the MiniGame Active Bool To False
            _rod.fightingMiniGameActive.Value = false;



            while (_FNAVM.caughtHybrid.navigationData.hybridRestTime >= 0 || _lure.Lure_CurrentDistance < _lure.lure_MiniGameDistancePoints[HybridCurrentFightAttempts])
            {
                _FNAVM.caughtHybrid.navigationData.hybridRestTime -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            OnHybridPulling();
        }

        private IEnumerator FightingPeriodCotoutine()
        {
            Debug.Log("Starting Hybrid Fighting");

            if (_lure.lure_MiniGameDistancePoints.Count < HybridMaxFightAttempts)
            {
                HybridCurrentFightAttempts++;
            }

            //remove from all lists and put in Mini Game List 
            _FNAVM.removeHybrid(_FNAVM.caughtHybrid, false);
            _FNAVM.AddHybridTolist(_FNAVM.caughtHybrid, HybridState.HybridMiniGame_Pulling);
            //Sets the MiniGame Active Bool To true
            _rod.fightingMiniGameActive.Value = true;

            var targetDir = _rod.rod_EndPointTransform - _lure.lure_HybridAttachPoint.transform.position;
            _lure.lure_StartingAngle.Value = Vector3.Angle(targetDir, _PLAYER.gameObject.transform.forward);

            StartCoroutine(SetHybridDirection());
            


            while (_FNAVM.caughtHybrid.navigationData.currentHybridStamina > 0)
            {
                RotateLure(lureRotationAxis);
                //TODO Compare Current Rotation direction to the players Rotation Direction, and subtract health from the fishing rod if the player is not pulling in the correct direction
                //TODO add cyody time to the pull direction so the player has a few seconds to fix their direction and incrase a value to be used by the sound 
                _FNAVM.caughtHybrid.navigationData.currentHybridStamina -= Time.deltaTime;
            }


            yield return new WaitForEndOfFrame();
        }

        private IEnumerator SetHybridDirection()
        {
            while (fishingMiniGameState == MiniGameState.HybridPulling)
            {
                int random = UnityEngine.Random.Range(1, 3);
                lureRotationAxis = random == 1 ? lureRotationAxis = Vector3.up : lureRotationAxis = Vector3.down;

                //TODO Set this up as a Enum so I can check agast it for the player pulling left for right

                yield return new WaitForSeconds(UnityEngine.Random.Range(5, 10));
            }
        }



        private void RotateLure(Vector3 _rotationAxix, float _SpeedMutiplayer = 1f)
        {
            //sets the delta angle from the lure rotation speed and the time
            float deltaAngle = (_lure.lure_LureRotationSpeed.Value * _SpeedMutiplayer) * Time.deltaTime;

            Vector3 targetDir = _rod.rod_EndPointTransform.Value - _lure.lure_HybridAttachPoint.transform.position;

            if (_lure.lure_StartingAngle.Value == 0)
            {
                _lure.lure_StartingAngle.Value = Vector3.Angle(targetDir, _PLAYER.gameObject.transform.forward);
            }

            float currentAngle = Vector3.Angle(targetDir, _PLAYER.playerHead.forward) - _lure.lure_StartingAngle.Value;

            //rotate bobber around player
            if (Mathf.Abs(currentAngle) < _lure.lure_MaxAngle && !Physics.CheckSphere(_lure.lure_Go.transform.position, 0.5f, terrainLayerMask))
            {
                _lure.lure_Go.transform.RotateAround(_rod.rod_EndPointTransform, _rotationAxix, deltaAngle);
            }
        }



        private void ResetLureRotation(Vector3 _rotationAxix, Vector3 _direction, float _SpeedMutiplayer = 0.5f)
        {

            Vector3 rotationAxis = new();
            Vector3 direction = new();



            if (_rod.rod_RodPullDirection.Value)
            {
                // Set bobber rotation for the "true" case
                rotationAxis = Vector3.up;
                direction = Vector3.left;
            }
            else
            {
                // Set bobber rotation for the "false" case
                rotationAxis = Vector3.down;
                direction = Vector3.right;
            }



            //sets the delta angle from the lure rotation speed and the time
            float deltaAngle = (_lure.lure_LureRotationSpeed.Value * _SpeedMutiplayer) * Time.deltaTime;

            Vector3 targetDir = _rod.rod_EndPointTransform.Value - _lure.lure_HybridAttachPoint.transform.position;

            if (_lure.lure_StartingAngle.Value == 0)
            {
                _lure.lure_StartingAngle.Value = Vector3.Angle(targetDir, _PLAYER.gameObject.transform.forward);
            }

            float currentAngle = Vector3.Angle(targetDir, _PLAYER.playerHead.forward) - _lure.lure_StartingAngle.Value;

            //rotate bobber around player
            if (Mathf.Abs(currentAngle) > 0.5f && !Physics.CheckSphere(_lure.lure_Go.transform.position, 0.5f, terrainLayerMask))
            {
                _lure.lure_Go.transform.RotateAround(_rod.rod_EndPointTransform, _rotationAxix, deltaAngle);
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
        /// <param name="_fullHybridList">Initial hybrids collider list</param>
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


