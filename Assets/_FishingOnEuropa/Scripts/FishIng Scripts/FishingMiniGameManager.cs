using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingMiniGameManager : Singleton<FishingMiniGameManager>
{
 

    [Header("Bobber")]
    [SerializeField] float bobberRange = 3; //detection range of nearby hybrids when cast
    [SerializeField] float minBobberReelDistance; //how close the bobber needs to be before reeling is complete
    [SerializeField] GameObject bobberGameObject;

    public enum BobberState { Withdrawn, Cast, AttachedFish }
    public BobberState bobberState;

    [Header("Hybrids")]
    GameObject targetHybrid; //hybrid used for fishing encounter
    HybridSO targetHybridSO;
    [SerializeField] LayerMask fishMask;

    [Header("Handle")]
    [SerializeField] float handleVelocity; //speed fishing rod handle is moving
    [SerializeField] float minPullAngle; //min rod angle to count as pulling in corrrect direction



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        switch(bobberState)
        {
            case BobberState.Cast:

                if(targetHybrid == null)
                {
                    //Get nearby hybrids
                    var nearbyHybrids = Physics.OverlapSphere(bobberGameObject.transform.position, bobberRange, fishMask);

                    if (nearbyHybrids.Length >= 5)
                    {
                        //Determine 5 closest hybrids
                        nearbyHybrids = ReturnClosestHybrids(nearbyHybrids, 5);


                    }
                }
                

                break;
            case BobberState.AttachedFish:
                break;
        }


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

    //GameObject CalculateHybridWithHighestCatchChance(Collider[] _hybridColliderArray)
    //{

    //    //get all hybrid catch chances
    //    List<int> hybridCatchChances = new();

    //    //percentage to spawn in this group
    //    List<int> hybridCatchChancePercentage = new();

    //    //total of all catch chances
    //    int catchChanceTotal = 0;


    //    foreach (var hybrid in _hybridColliderArray)
    //    {
    //        hybridCatchChances.Add(hybrid.GetComponent<HybridInfo>().hybridInfo.catchChance);
    //        catchChanceTotal += hybrid.GetComponent<HybridInfo>().hybridInfo.catchChance;
    //    }

    //    //calculate percentage
    //    foreach (var item in collection)
    //    {

    //    }
    //}
}
