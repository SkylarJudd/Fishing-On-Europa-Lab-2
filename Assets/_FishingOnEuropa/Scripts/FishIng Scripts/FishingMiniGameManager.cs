using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishNavigationManager;

public class FishingMiniGameManager : Singleton<FishingMiniGameManager>
{
    [Header("Bobber")]
    [SerializeField] float bobberRange = 3; //detection range of nearby hybrids when cast
    [SerializeField] float minBobberReelDistance; //how close the bobber needs to be before reeling is complete
    public GameObject bobberGameObject;

    public enum BobberState { Withdrawn, Cast, AttachedFish }
    public BobberState bobberState;

    [Header("Hybrids")]
    [SerializeField]
    GameObject targetHybrid; //hybrid used for fishing encounter
    HybridSO targetHybridSO;
    hybridNavData targetHybridNavData;
    [SerializeField] LayerMask fishMask;
    public Collider[] nearbyHybrids;

    [Header("Handle")]
    [SerializeField] float handleVelocity; //speed fishing rod handle is moving
    [SerializeField] float minPullAngle; //min rod angle to count as pulling in corrrect direction
    Rigidbody rb_reelHandle;


    //[Header("Fish Encounter")]
    public enum FishEncounterState { None, Resting, Fighting, Caught}
    public FishEncounterState fishEncounterState;
    float hybridRestTime;

    // Update is called once per frame
    void Update()
    {
        
        switch(bobberState)
        {
            case BobberState.Cast:

                if(targetHybrid == null)
                {
                    //Get nearby hybrids
                    nearbyHybrids = Physics.OverlapSphere(bobberGameObject.transform.position, bobberRange, fishMask);

                    if (nearbyHybrids.Length >= 5)
                    {
                        //Determine 5 closest hybrids
                        nearbyHybrids = ReturnClosestHybrids(nearbyHybrids, 5);

                    }

                    //CHANGE INPUT HERE - commit to catching fish (or pull rod)
                    if(Input.GetKey(KeyCode.Space))
                    {
                        targetHybrid = CalculateHybridWithHighestCatchChance(nearbyHybrids);

                        targetHybridSO = targetHybrid.GetComponent<HybridInfo>().hybridInfo;
                        //change fish state here

                        //need to get parent / FOE Item
                        targetHybrid = targetHybrid.GetComponentInParent<FOEItem_Hybrid>().gameObject;

                        _FNAVM.UpdateHybridState(targetHybrid, HybridState.HybridMiniGame_SwimToLure);

                        //remove from swim list and put in swimToPoint list
                        _FNAVM.removeHybrid(targetHybrid, false);
                        _FNAVM.AddHybridTolist(targetHybrid);

                        print(targetHybrid.name);
                    }
                }
                

                break;
            case BobberState.AttachedFish:

                switch(fishEncounterState)
                {
                    case FishEncounterState.Resting:

                        hybridRestTime -= Time.deltaTime;

                        if(hybridRestTime <= 0.0f)
                        {
                            //rest time is over

                        }
                        else
                        {
                            //if handle is moving. Handle should be clamped to only move in circular motion
                            if(rb_reelHandle.velocity.magnitude > 0)
                            {
                                //dont know how Skylar will do reeling so cand do this 
                            }
                        }

                        break;
                }


                break;

                /*  1. make sure updteMiniGame is called in start
                 *  2. set _FNAVM.caughtHyrbid = targetHybridNavData
                 *  3. to change fish state _FNVAM.caughtHybrid.hybrid state > update that state
                 *  4. caught fish movement for fish encounter is in _FNAVM.UpdateMiniGame
                 */

        }


    }


    /// <summary>
    /// Set rest period time and change fishEncounterState
    /// </summary>
    void BeginRestingPeriod()
    {
        hybridRestTime = Random.Range(targetHybridSO.restMinTime, targetHybridSO.restMaxTime);
        fishEncounterState = FishEncounterState.Resting;

    }

    /// <summary>
    /// Reset variables to null or default to prepare for next catch
    /// </summary>
    void ResetVariables()
    {
        hybridRestTime = 0;
    }

    /// <summary>
    /// Return a given amount of hybrids which are closest to the bobber
    /// </summary>
    /// <param name="_fullHybridList">Inital hybrids collider list</param>
    /// <param name="_returnListLength"> How many hybrids will be returned</param>
    /// <returns></returns>
    Collider[] ReturnClosestHybrids(Collider[] _fullHybridList, int _returnListLength)
    {
        List<Collider> newHybridsList = new List<Collider>();
        

        //add first hybrid for comparisions
        newHybridsList.Add(_fullHybridList[0]);
        float maxHybridDistance  = Vector3.Distance(bobberGameObject.transform.position, _fullHybridList[0].transform.position);
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
            else if (Vector3.Distance(bobberGameObject.transform.position, _fullHybridList[i].transform.position) < maxHybridDistance)
            {
                //remove old max
                newHybridsList.RemoveAt(maxHybridIndex);

                //add new hybrids
                newHybridsList.Add(_fullHybridList[i]);

                //create temp max distance to compare too
                var tempMax = Vector3.Distance(bobberGameObject.transform.position, newHybridsList[0].transform.position);

                //find new max
                foreach (var hybrid in newHybridsList)
                {
                    if(Vector3.Distance(bobberGameObject.transform.position, hybrid.transform.position) > tempMax)
                    {
                        tempMax = Vector3.Distance(bobberGameObject.transform.position, hybrid.transform.position);
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
