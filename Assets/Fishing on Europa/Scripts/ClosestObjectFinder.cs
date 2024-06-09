using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestObjectsFinder : MonoBehaviour
{
    public string targetTag = "Fish";
    public int maxObjectsToFind = 5;

    List<GameObject> closestObjects = new List<GameObject>();
    List<int> fishRoll = new List<int>(); // Sounds Yummy :D, oh no now im hungry
    GameObject[] taggedObjects;
    public LerpAndHide lerpAndHide;


    public bool fishCaught = false;
    List<GameObject> caughtFishesList = new List<GameObject>();
    public GameObject caughtFish;
    [SerializeField] GameObject net;
    [SerializeField] FollowCaughtFish netScript;
    //[SerializeField] LockPlayerScript lockplayer;
    [SerializeField] RodPullLure rodPullLure;
    [SerializeField] RodAncorScript rodAncorScript;
    [SerializeField] FishingMiniGameMannager fishingMiniGameMannager;
    int catchAttempt;
    bool allFound = false;
    bool allArrived = false;
    int arrivedCount = 0;
    bool countFinished = false;

    public float targetNumber = 100f;
    public float countSpeed = 10f;
    private float currentNumber = 0f;

    private void Update()
    {
        FishCatchAnimation();
    }

    public void FindClosestObjects()
    {
        //call script to set max distance
        //print("Calling update max distance");
        rodPullLure.UpdateMaxDistance();
        rodAncorScript.UpdateRodAnchor();

        taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);
        Transform thisTransform = transform;
        //bool shouldContinueOuterLoop = false;

        for (int i = 0; i < maxObjectsToFind; i++)
        {
            //print(i);
            GameObject closestObject = null;
            float closestDistance = float.MaxValue;

            float objectCount = 0;
            foreach (GameObject taggedObject in taggedObjects)
            {
                objectCount += 1;
                //print(objectCount);
                float distance = Vector3.Distance(thisTransform.position, taggedObject.transform.position);

                if (!closestObjects.Contains(taggedObject) && distance < closestDistance)
                {
                    //print("objects in list " + caughtFishesList.Count);
                    if (caughtFishesList.Count == 0)
                    {
                        //print("List has 0 fish");
                        closestObject = taggedObject;
                        closestDistance = distance;
                    }
                    else
                    {

                        if (!caughtFishesList.Contains(taggedObject))
                        {
                            //print("There is an object in the list, but this is not it");
                            closestObject = taggedObject;
                            closestDistance = distance;
                        }
                        else
                        {
                            //print("Dupe Found");
                            closestDistance = distance;
                        }


                       // print("List has fish checking for dupes");
                       //foreach (GameObject caughtFishList in caughtFishesList)
                       //{
                       // if (taggedObject != caughtFishList)
                       // {
                       // closestObject = taggedObject;
                       //   closestDistance = distance;
                       // }
                       // else
                       //   {
                       //     print("Dupe Found");
                       //Loop again from the start dont do the code below
                       //shouldContinueOuterLoop = true;
                       //    closestDistance = distance;

                            // }
                            // }
                    }     
                }
            }

            
                if (closestObject != null)
                {
                    if (closestObject != caughtFish)
                    {
                        //print("Added " + closestObjects + " to list");
                        closestObjects.Add(closestObject);
                        OnFishFound(closestObject); // Call a function when a fish is found
                    }
                }
                else
                {
                    //print("Loop broke before 5 objects where found");
                    allFound = true;
                    break; // No more objects to find
                }
            
                
            


        }
        allFound = true;
    }

    void OnFishFound(GameObject fish)
    {
        //Debug.Log("Found fish: " + fish.name);
        fishStats fishStat = fish.GetComponent<fishStats>();
        float catchChance = fishStat.fish.catchChance;
        FishNavigationScript fishNavigationScript = fish.GetComponent<FishNavigationScript>();
        //FishNavigationScript.ToggleToCatch();
        //print("Fish Set To Move To Lure");
        fishNavigationScript.ToggleToCatch(FishStates.FishMoveToLure);

        int randomChance = Random.Range(1, 100);

        if (randomChance >= catchChance && fishCaught == false)
        {
            FishCaught();


        }
        else if ( randomChance < catchAttempt && fishCaught == false)
        {
                 //Debug.Log("Found fish: " + fish.name + " This fish has a " + catchChance + " Chance of catch.");
                catchAttempt = catchAttempt + 1;
                print(catchAttempt);

            if (catchAttempt == 5 )
            {
                FishCaught();

            }
        }


    void FishCaught()
    {
            //Debug.Log("Found fish: " + fish.name + " This fish was caught with a " + catchChance + " Chance of catch.");
            //targetNumber = catchChance;
            fishCaught = true;
            caughtFish = fish;
            //print("Added " + fish + " to the list");
            caughtFishesList.Add(fish);

         // FishNavigationScript FishNavScript = fish.GetComponent<FishNavigationScript>();
         //FishNavScript.fishBehaviorState = 4;

        }

        //Debug.Log("Found fish: " + fish.name + " This fish has a " + catchChance + " Chance of catch.");


    }

    void FishCatchAnimation()
    {
        if (allArrived == true && countFinished != true)
        {
            
            // Increment the current number based on the countSpeed
            currentNumber += countSpeed * Time.deltaTime;

            // Ensure that the current number doesn't exceed the target number
            currentNumber = Mathf.Min(currentNumber, targetNumber);

            

            foreach(GameObject closestObject in closestObjects)
            {
                fishStats fishStat = closestObject.GetComponent<fishStats>();
                //print("Current Nuber = " + currentNumber + "Object " + closestObject + "Catch Chance = " + (fishStat.fish.catchChance) + "To " + (Mathf.Abs(fishStat.fish.catchChance - 100)));
                if (Mathf.FloorToInt(currentNumber) == Mathf.Abs(fishStat.fish.catchChance - 100) )
                {
                    if (closestObject != caughtFish)
                    {
                        FishNavigationScript FishNavScript = closestObject.GetComponent<FishNavigationScript>();
                        //FishNavScript.fishBehaviorState = 2;
                        //print("Fish Set To Flocking");
                        FishNavScript.ToggleToCatch(FishStates.FishFlocking);
                    }
                    else
                    {
                        FishNavigationScript FishNavScript = closestObject.GetComponent<FishNavigationScript>();
                        //FishNavScript.fishBehaviorState = 4;
                        //print("Fish Set To Idel");
                        FishNavScript.ToggleToCatch(FishStates.FishIdel);

                        net.SetActive(true);
                        fishingMiniGameMannager.ToggleTimer(true);
                        netScript.TrackFish(caughtFish);
                        //lockplayer
                        //lockplayer.StopPlayerMovement();
                        FishReset();
                    }
                }
            }

            // Check if the target number is reached
            if (currentNumber >= targetNumber)
            {
                //Debug.Log("Target Number Reached!");
                countFinished = true;
                
            }
        }
    }

    public void FishReset()
    {
        if (taggedObjects != null)
        {
            foreach (GameObject taggedObject in taggedObjects)
            {
                if (taggedObject != caughtFish)
                {
                    FishNavigationScript fishNavigationScript = taggedObject.GetComponent<FishNavigationScript>();
                    //FishNavigationScript.fishBehaviorState = 2;
                    //print("Fish Set To Flocking");
                    fishNavigationScript.ToggleToCatch(FishStates.FishFlocking);
                    fishNavigationScript.arrived = false;
                }

            }
        }


        //FishNavigationScript FishNavigationScript = fish.GetComponent<FishNavigationScript>();
    }
    public void FullFishReset()
    {
        //print("Full reset triggerd");
        fishCaught = false;
        allFound = false;
        allArrived = false;
        arrivedCount = 0;
        countFinished = false;
        closestObjects.Clear();
        catchAttempt = 0;
        currentNumber = 0;
        lerpAndHide.StartLerp();
        net.SetActive(false);
        netScript.StopTracking();
        fishingMiniGameMannager.ToggleTimer(false);
        //lockplayer.StartPlayerMovement();

    }

    public void ArrivedUpdate()
    {
        arrivedCount += 1;
        if (allFound == true && arrivedCount == closestObjects.Count)
        {
            allArrived = true;
            //print("All Arrived");
        }
    }
    public void RemoveFromFishCaughtList(GameObject fish)
    {
        caughtFishesList.Remove(fish);
    }
}