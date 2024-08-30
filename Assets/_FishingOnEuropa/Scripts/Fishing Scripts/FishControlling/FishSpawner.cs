using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] FishGameObjects;

    [SerializeField] private List<GameObject> fishToDepawnList = new List<GameObject>();

    public float rateOfSpawn = 2;
    public float rateOfSpawnRange = 1;
    public float maxFish = 20;

    public float rateOfGoalUpdate = 4;
    public float rateOfGoalUpdateRange = 3;

    private int randomFishSpawnerValue;
    private int fishRareity;
    private int commonValue = 64; //64%
    private int uncommonValue = 89; //25%
    private int rareValue = 99;  //10%
    //private int epicValue = 100;  //1%

    private float nextSpawn = 0;
    private float nextGoal = 0;
    public GameObject goal;

    public static Vector3 goalPos = Vector3.zero;

    public static int fishInArray = 0;

    public BoxCollider boxCollider;
    public ClosestObjectsFinder fishcaught;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < maxFish; i++)
        {
            SpawnFish();
            //print("SpawnFishCalled");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn) //&& fishcaught.fishCaught == false (REMOVED AS ClosestObjectsFinder IS BEING REMOVED)
        {
            SpawnFish();
            
        }
        if (Time.time > nextGoal)
        {
            UpdateFishGoal();
        }
    }

    private void SpawnFish()
    {
        //print("SpawnFishEnterd");
        nextSpawn = Time.time + Random.Range(rateOfSpawn - rateOfSpawnRange, rateOfSpawn + rateOfSpawnRange);

        Vector3 boxSize = boxCollider.size;
        Vector3 boxCenter = boxCollider.transform.TransformPoint(boxCollider.center);

        Vector3 rndPosWithin = new Vector3(
            Random.Range(boxCenter.x - boxSize.x * 0.5f, boxCenter.x + boxSize.x * 0.5f),
            Random.Range(boxCenter.y - boxSize.y * 0.5f, boxCenter.y + boxSize.y * 0.5f),
            Random.Range(boxCenter.z - boxSize.z * 0.5f, boxCenter.z + boxSize.z * 0.5f)
        );

        //print("FishSelectionCalled");
        FishSelection();
       // print("FishSpawnerComponentCalled");
        FishGameObjects[fishRareity].GetComponent<SpawnCallerFish>().FishSpwaner(rndPosWithin);
    }

    private void UpdateFishGoal()
    {
        nextGoal = Time.time + Random.Range(rateOfGoalUpdate - rateOfGoalUpdateRange, rateOfGoalUpdate + rateOfGoalUpdateRange);

        Vector3 boxSize = boxCollider.size;
        Vector3 boxCenter = boxCollider.transform.TransformPoint(boxCollider.center);

        Vector3 rndPosWithin = new Vector3(
            Random.Range(boxCenter.x - boxSize.x * 0.5f, boxCenter.x + boxSize.x * 0.5f),
            Random.Range(boxCenter.y - boxSize.y * 0.5f, boxCenter.y + boxSize.y * 0.5f),
            Random.Range(boxCenter.z - boxSize.z * 0.5f, boxCenter.z + boxSize.z * 0.5f)
        );

        goalPos = rndPosWithin;
        goal.transform.position = goalPos;
    }




    private void FishSelection()
    {
        //print("FishSelectionEnterd");
        randomFishSpawnerValue = Random.Range(1, 101);
        //print("Random Value = " + randomFishSpawnerValue);
        if (randomFishSpawnerValue < commonValue)
        {
            //print("Returned 0");
            fishRareity = Random.Range(0,4);
            return;
        }
        if (randomFishSpawnerValue < uncommonValue)
        {
            //print("Returned 1");
            fishRareity = Random.Range(4, 7);
            return;
        }
        if (randomFishSpawnerValue < rareValue)
        {
            //print("Returned 2");
            fishRareity = Random.Range(7, 9);
            return;
        }
        //print("Returned 3");
        fishRareity = 9;


    }

    // add fish to despawn list is called from each of the "SpawCallerFish"  scripts, its job is to loop though the array adding each fish to the array untill there are 20 fish in the pond. 

    public void AddFishToDespwnList(GameObject theFishToDespwn)
    {
        //print("AddFishToDespawnListEnterd");
       // print("DespawnFishCalled");
        DespawnFish();
        fishToDepawnList.Add(theFishToDespwn);
    }


    //despawn fish is called to despawn fish onces the total amout of fish reaches the amount in the array it also loops though the array and moves each fish down one space each time a fish is spawned. 
    //public bool skip = false;

    private void DespawnFish()
    {
        //print("DespawnFishEnterd");
        if (fishToDepawnList.Count >= maxFish)
        {
            //print("FishDespawned");
            fishToDepawnList[0].SetActive(false);
            fishToDepawnList.Remove(fishToDepawnList[0]);
        }

    }

    public void RemoveFishFromList(GameObject FishToRemove)
    {
        fishToDepawnList.Remove(FishToRemove);
    }

    public List<GameObject> GetFishToDepawnList()
    {
        return fishToDepawnList;
    }


}

