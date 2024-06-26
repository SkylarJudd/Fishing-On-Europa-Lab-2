using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class FishSpawnerGeyser : MonoBehaviour
{
    [SerializeField] FishToSpawn fishToSpawn;
    [SerializeField] Transform spawnPos;
    [SerializeField] Transform targetPos;
    [SerializeField] Transform randomLocation;

    [SerializeField] List<HybridsToSpawn> fishList;

    [Serializable]
    public class HybridsToSpawn
    {
        public float spawnChance;
        public float newSpawnChance;
        public GameObject hybridGameObject;
    }

    float forceMultiplier = 10f;

    private void Start()
    {

        int total = 0;

        for (int i = 0; i < fishToSpawn.FishToSpawnList.Count; i++)
        {
            HybridsToSpawn newEntry = new HybridsToSpawn();
            newEntry.spawnChance = fishToSpawn.FishToSpawnList[i].spawnChance;
            newEntry.hybridGameObject = fishToSpawn.FishToSpawnList[i].hybridPrefab;
            total += fishToSpawn.FishToSpawnList[i].spawnChance;
            fishList.Add(newEntry);
        }

        //print(total);

        //print(FishList[1].spawnChance / total);

        for (int i = 0; i < fishToSpawn.FishToSpawnList.Count; i++)
        {
            fishList[i].newSpawnChance = (fishList[i].spawnChance / total) * 100;
        }

        fishList.Sort((x, y) => x.newSpawnChance.CompareTo(y.newSpawnChance));

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //print("Mouse1Pressed");
            GameObject Hybrid = Instantiate(GetFishToSpawn(), spawnPos.position, Quaternion.LookRotation(targetPos.transform.position));
            MoveToTarget(Hybrid, GetRandomPositionAround(targetPos));
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //print("Mouse2Pressed");
            GameObject Hybrid = Instantiate(GetFishToSpawn(), spawnPos.position, Quaternion.LookRotation(targetPos.transform.position));
            MoveToTarget(Hybrid, targetPos.position);
        }
    }

    public void MoveToTarget(GameObject fishHybrid, Vector3 target)
    {
        Rigidbody _Rb = fishHybrid.GetComponent<Rigidbody>();
        if (_Rb == null)
        {
            Debug.LogError("No Rigidbody found on the instantiated fish object.");
            return;
        }

        Vector3 distance = target - fishHybrid.transform.position;

        float height = distance.y;
        Vector3 horizontalDistance = new Vector3(distance.x, 0, distance.z);

        // Debugging output to check calculated values
        //Debug.Log($"Distance: {distance}");
        //Debug.Log($"Height: {height}");
        //Debug.Log($"Horizontal Distance: {horizontalDistance}");

        float g = Physics.gravity.y;
        float Vy;
        Vector3 VXZ;

        if (height > 0)
        {
            // Target is above the spawn point
            Vy = Mathf.Sqrt(-2 * g * height);
            float timeToReachApex = Vy / -g;
            float totalTime = timeToReachApex * 2; // Time to reach the target when height > 0
            VXZ = horizontalDistance / totalTime;
        }
        else
        {
            // Target is below or at the same level as the spawn point
            Vy = Mathf.Sqrt(2 * -g * -height);
            float timeToReachTarget = Mathf.Sqrt(2 * -height / -g);
            VXZ = horizontalDistance / timeToReachTarget;
        }

        // Check for NaN values
        if (float.IsNaN(Vy) || float.IsNaN(VXZ.x) || float.IsNaN(VXZ.z))
        {
            Debug.LogError($"Calculated velocities contain NaN values: Vy={Vy}, VXZ={VXZ}");
            return;
        }

        // Set the velocity of the Rigidbody
        _Rb.velocity = new Vector3(VXZ.x, Vy, VXZ.z);
        _Rb.useGravity = true;

        // Debugging output to check assigned velocity
        //Debug.Log($"Assigned velocity: {_Rb.velocity}");
    }

    private GameObject GetFishToSpawn()
    {
        float randomNumber = UnityEngine.Random.Range(0, 100);
        if ( 100 == UnityEngine.Random.Range(0, 100))
        {
            bool isShiney = true;
        }

        print(fishList.Count);
        print("Random number = " + randomNumber);

        float currentChanceValue = 0;

        foreach (HybridsToSpawn hybrids in fishList)
        {

            currentChanceValue += hybrids.newSpawnChance;

            if (randomNumber <= currentChanceValue)
            {
                return hybrids.hybridGameObject;
            }
        }
        Debug.LogError($"Hybrid was outside the random range with a value of {randomNumber} , spawning {fishList[1].hybridGameObject}");
        return fishList[1].hybridGameObject;
    }

    public Vector3 GetRandomPositionAround(Transform centerTransform)
    {
        float radius = 5f;

        float randomAngle = UnityEngine.Random.Range(0f, 360f);
        float x = centerTransform.position.x + radius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float z = centerTransform.position.z + radius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);
        float y = centerTransform.position.y;

        randomLocation.transform.position = new Vector3(x, y, z);

        return new Vector3(x, y, z);
    }
}