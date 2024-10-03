
using Obvious.Soap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class FishingMiniGameManager : Singleton<FishingMiniGameManager>
    {
        [Header("Bobber")]
        [SerializeField] float bobberRange = 3; //detection range of nearby hybrids when cast
        [SerializeField] float minBobberReelDistance; //how close the bobber needs to be before reeling is complete
        public GameObject bobberGameObject, bobberTipGO; //fish spot is where fish will attach to

        public enum BobberState { Withdrawn, Cast, HitWater, AttachedFish }

        public BobberState bobberState;

        [Header("Hybrids")]
        public GameObject targetHybrid; //hybrid used for fishing encounter
        HybridSO targetHybridSO;
        FOEItem_Hybrid targetHybridNavData;
        public LayerMask fishMask, fishCollisionLayerMask;
        public Collider[] nearbyHybrids;
        public float trackingLureInFightSpeed;

        [Header("Handle")]
        [SerializeField] float handleVelocity; //speed fishing rod handle is moving
        [SerializeField] float minPullAngle; //min rod angle to count as pulling in corrrect direction
        [SerializeField] Rigidbody rb_reelHandle;

        [Header("Fishing Rod")]
        [SerializeField] FloatReference fishingRodHP; //how much hp the rod has 
        [SerializeField] FloatReference fishingEfficency; //the rate at which hybrid stamina is drained per second
        public GameObject fishingRod;

        [Header("Lure")]
        [SerializeField] FloatReference LureCurrentDistance;
        [SerializeField] FloatReference LureCurrentMaxDistance;
        [SerializeField] FloatReference LureMaxDistanceFromRod;
        [SerializeField] BoolReference Casted;
        [SerializeField] BoolReference fightingMiniGameActive;
        [SerializeField] BoolReference rodPullDirection;


        [Header("Fish Encounter")]
        [SerializeField] LayerMask terrainLayerMask;

        [SerializeField] float hybridRestTime, currentHybridStamina;
        public float bobberRotationSpeedFighting, maxAngle;
        public enum FishEncounterState { None, Resting, Fighting, Caught }
        public FishEncounterState fishEncounterState;
        public enum PullDirections { NotSet, Left, Right, Middle }
        public PullDirections currentPullDirection;

        void Update()
        {
            //bobberGameObject.transform.RotateAround(_PLAYER.transform.position, Vector3.up, bobberRotationSpeed * Time.deltaTime);

            switch (bobberState)
            {
                case BobberState.Withdrawn:

                case BobberState.Cast:

                    


                case BobberState.HitWater:

                    if (targetHybrid == null)
                    {
                        //Get nearby hybrids
                        nearbyHybrids = Physics.OverlapSphere(bobberGameObject.transform.position, bobberRange, fishMask);

                        if (nearbyHybrids.Length >= 5)
                        {
                            //Determine 5 closest hybrids
                            nearbyHybrids = ReturnClosestHybrids(bobberGameObject, nearbyHybrids, 5);

                        }

                        //CHANGE INPUT HERE - commit to catching fish (or pull rod)
                        if (nearbyHybrids.Length > 0 && Input.GetKey(KeyCode.Space))
                        {
                            targetHybrid = CalculateHybridWithHighestCatchChance(nearbyHybrids);

                            targetHybridSO = targetHybrid.GetComponent<HybridInfo>().hybridInfo;
                            //change fish state here

                            //need to get parent / FOE Item
                            targetHybrid = targetHybrid.GetComponentInParent<FOEItem_Hybrid>().europaItemData.itemGO;

                            _FNAVM.removeHybrid(targetHybrid, false);

                            _FNAVM.UpdateHybridState(targetHybrid, HybridState.HybridMiniGame_SwimToLure);

                            //remove from swim list and put in swimToPoint list
                            _FNAVM.AddHybridTolist(targetHybrid);

                        }
                    }


                    break;
                case BobberState.AttachedFish:

                    switch (fishEncounterState)
                    {
                        case FishEncounterState.Resting:

                            //add to return to middle

                            hybridRestTime -= Time.deltaTime;

                            if (hybridRestTime <= 0.0f)
                            {
                                //rest time is over
                                print("well rested");
                                fishEncounterState = FishEncounterState.Fighting;
                            }
                            else
                            {
                                ////if handle is moving.Handle should be clamped to only move in circular motion
                                //if (rb_reelHandle.velocity.magnitude > 0)
                                //{

                                    

                                //}
                                //else
                                //{
                                //    //if not moving, make idle
                                //    if (!_FNAVM.CheckHybridState(targetHybrid, HybridState.HybridIdle))
                                //    {
                                //        //remove from lists
                                //        //_FNAVM.RemoveHybrid(targetHybrid, false);

                                //        _FNAVM.UpdateHybridState(targetHybrid, HybridState.HybridIdle);

                                //        //remove add to new list
                                //        _FNAVM.AddHybridTolist(targetHybrid);
                                //    }

                                //}

                                //check if hybrid is close enough to end
                                if (LureCurrentDistance <= minBobberReelDistance)
                                {
                                    //CAUGHT
                                    fishEncounterState = FishEncounterState.Caught;


                                }

                            }

                            break;
                        case FishEncounterState.Fighting:

                            if (!_FNAVM.CheckHybridState(targetHybrid, HybridState.HybridMiniGame_Pulling))
                                _FNAVM.UpdateHybridState(targetHybrid, HybridState.HybridMiniGame_Pulling);

                            //determine pull direction
                            if (currentPullDirection == PullDirections.NotSet) currentPullDirection = GetDirection();

                            
                            break;
                    }


                    break;

                    /*  1. make sure updteMiniGame is called in start
                     *  2. set _FNAVM.caughtHyrbid = targetHybridNavData
                     *  3. to change fish state _FNVAM.caughtHybrid.hybrid state > update that state
                     *  4. caught fish movement for fish encounter is in _FNAVM.UpdateMiniGame
                     *  
                     *  END MINIGAME IENUMRATOR WHEN FISHING IS DONE
                     */

            }


        }


        IEnumerator UpdateHybridAndRodDuringFighting()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                //if rod is pulling in other direction
                if (CheckIfPullingInCorrectDirction())
                {
                    //deplete stamina each second
                    currentHybridStamina -= fishingEfficency;
                    if (currentHybridStamina <= 0.0f)
                    {
                        //if stamina = 0 switch to resting
                        BeginRestingPeriod();
                    }
                }
                else
                {
                    //else damage rod
                    fishingRodHP.Value -= targetHybridSO.damageToRod;

                    //if rod hp = 0

                    if (fishingRodHP <= 0.0f)
                    {
                        //escaped
                        _FNAVM.UpdateHybridState(targetHybrid, HybridState.HybridMiniGame_Escaped);


                        fishEncounterState = FishEncounterState.None;

                    }
                }
            }
        }


        bool CheckIfPullingInCorrectDirction()
        {
            bool directionCorrect = false;

            if(currentPullDirection == PullDirections.Right && rodPullDirection) //true is right, false is left
            {
                directionCorrect = true;
            }
            else if(currentPullDirection == PullDirections.Left && !rodPullDirection)
                directionCorrect = true;

            return directionCorrect;
        }

        /// <summary>
        /// Subscribe to OnLureHitWater event on Lure
        /// </summary>
        public void LureHitWater()
        {
            bobberState = BobberState.HitWater;
        }

        /// <summary>
        /// Set rest period time and change fishEncounterState
        /// </summary>
        public void BeginRestingPeriod()
        {
            //while reel is moving, change target fish to move to lure
            _FNAVM.removeHybrid(targetHybrid, false);

            _FNAVM.UpdateHybridState(targetHybrid, HybridState.HybridMiniGame_Tired);

            //remove from swim list and put in swimToPoint list
            _FNAVM.AddHybridTolist(targetHybrid);

            hybridRestTime = Random.Range(targetHybridSO.restMinTime, targetHybridSO.restMaxTime);
            print(hybridRestTime);
            fishEncounterState = FishEncounterState.Resting;
            fightingMiniGameActive.Value = true;

        }

        public void BeginFightingPeriod()
        {
            fishEncounterState = FishEncounterState.Fighting;

            currentHybridStamina = targetHybridSO.stamina;

            fightingMiniGameActive.Value = true;

            StartCoroutine(UpdateHybridAndRodDuringFighting());




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

            return viableDirection[Random.Range(0, viableDirection.Count)];

        }

        public bool CheckForHybridCollision(Vector3 direction)
        {
            if (!Physics.Raycast(bobberTipGO.transform.position, direction, 3f, terrainLayerMask))
            {
                return true; //no collsion
            }
            else return false; //there is a collision
        }

        /// <summary>
        /// Reset variables to null or default to prepare for next catch
        /// </summary>
        void ResetVariables()
        {
            hybridRestTime = 0;
            fightingMiniGameActive.Value = false;
            currentPullDirection = PullDirections.NotSet;
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


            //add first hybrid for comparisions
            newHybridsList.Add(_fullHybridList[0]);
            float maxHybridDistance = Vector3.Distance(center.transform.position, _fullHybridList[0].transform.position);
            int maxHybridIndex = 0;

            //only add up to returnListLength
            for (int i = 1; i < _fullHybridList.Length; i++)
            {
                //add hyrbids no matter distance
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
        GameObject CalculateHybridWithHighestCatchChance(Collider[] _hybridColliderArray)
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
            int hybridCatchChanceChosen = generateNumber100List[Random.Range(0, generateNumber100List.Count)];

            GameObject chosenHybrid = _hybridColliderArray[hybridCatchChances.IndexOf(hybridCatchChanceChosen)].gameObject;

            return chosenHybrid;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(bobberGameObject.transform.position, bobberRange);


        }
    }
}


