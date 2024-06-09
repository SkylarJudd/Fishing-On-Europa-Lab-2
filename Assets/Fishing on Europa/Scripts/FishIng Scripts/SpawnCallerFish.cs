using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCallerFish : MonoBehaviour
{
    public GameObject[] fishPool;

    private int fishPoolCount;
    private Vector3 spawnLocation;

    public GameObject FishSpawner;

    public void FishSpwaner(Vector3 whereToSpawnFish)
    {
        //print("FishSpawnerComponentEnterd");
        //Sets the fish difined by fishPoolCount to active
        fishPool[fishPoolCount].SetActive(true);
        // Give the fish a location to spawn 
        fishPool[fishPoolCount].transform.position = whereToSpawnFish;
        // Give fish a random rotation when spawned
        fishPool[fishPoolCount].transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        // Set the initial scale to 0.1
        fishPool[fishPoolCount].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Smoothly interpolate the scale to 1 over a certain duration (e.g., 1 second)
        float scaleDuration = 1.0f;
        StartCoroutine(ScaleFishToFullSize(fishPool[fishPoolCount], scaleDuration));

        fishPoolCount += 1;
        if (fishPoolCount == fishPool.Length)
        {
            fishPoolCount = 0;
        }

        //print("AddFishToDespawnListCalled");
        FishSpawner.GetComponent<FishSpawner>().AddFishToDespwnList(fishPool[fishPoolCount]);

        
    }


    private IEnumerator ScaleFishToFullSize(GameObject fish, float duration)
    {
        float elapsedTime = 0;
        Vector3 initialScale = fish.transform.localScale;
        Vector3 targetScale = new Vector3(1f, 1f, 1f);

        while (elapsedTime < duration)
        {
            //print(fish + " Is being Scaled");
            //print("Current Local Scale = " + fish.transform.localScale);
            fish.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //print(fish + " Finished Scale");
        fish.transform.localScale = targetScale; // Ensure the final scale is exactly 1
    }
}
