using System;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;



namespace Europa
{
    public class FishSpawnerGeyser : GameBehaviour
    {
        [SerializeField]
        private FishToSpawnSO fishToSpawn;
        [SerializeField]
        private Transform spawnPos;
        [SerializeField]
        private Transform targetPos;
        [SerializeField]
        private Transform randomLocation;

        [SerializeField]

        private ScriptableListFOEItem_Hybrid _hybridsFlyingList;
        [SerializeField]
        private ScriptableListFOEItem_Hybrid _hybridsToNavList;

        [SerializeField]
        private List<HybridsToSpawn> fishList;

        [Serializable]
        public class HybridsToSpawn
        {
            public float spawnChance;
            public float newSpawnChance;
            public PoolType poolType;
            public GameObject hybridGameObject;
        }

        [SerializeField] GeyserController geyserMannager;

        private void Start()
        {

            int total = 0;

            for (int i = 0; i < fishToSpawn.FishToSpawnList.Count; i++)
            {
                HybridsToSpawn newEntry = new HybridsToSpawn();
                newEntry.spawnChance = fishToSpawn.FishToSpawnList[i].hybridSO.spawnChance;
                newEntry.hybridGameObject = fishToSpawn.FishToSpawnList[i].europaItemSO.itemPrefab;
                newEntry.poolType = fishToSpawn.FishToSpawnList[i].hybridSO.hybridPoolType;
                total += fishToSpawn.FishToSpawnList[i].hybridSO.spawnChance;
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

        public void SpawnHybrid()
        {

            HybridsToSpawn hybridToSpawn = GetFishToSpawn();

            Vector3 direction = targetPos.position - spawnPos.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            //Debug.Log($"Spawning {hybridToSpawn.hybridGameObject.name} at {spawnPos.position} looking towards {targetPos.position} with rotation {lookRotation.eulerAngles}");
            GameObject hybridFromManager = _OPM.SpawnObject(hybridToSpawn.hybridGameObject, spawnPos.transform.position, lookRotation, hybridToSpawn.poolType);
            //Debug.Log($"Spawned object {hybridFromManager.name} with rotation {hybridFromManager.transform.rotation.eulerAngles}");

            FOEItem_Hybrid _hybrid = hybridFromManager.GetComponent<FOEItem_Hybrid>();

            _hybrid.InitHybrid(hybridFromManager, geyserMannager.waterHeight.position.y, geyserMannager.pondType, HybridState.HybridFlying, HybridVisualsState.World);

            _hybridsFlyingList.Add(_hybrid);
            _hybridsToNavList.Add(_hybrid);


            MoveToTarget(hybridFromManager, GetRandomPositionAround(targetPos));

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
            _Rb.linearVelocity = new Vector3(VXZ.x, Vy, VXZ.z);
            _Rb.useGravity = true;

            // Apply rotational force
            Vector3 rotationalForce = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)); // This will rotate the object around its Y-axis

            _Rb.angularVelocity = rotationalForce;
        }

        private HybridsToSpawn GetFishToSpawn()
        {
            //print("GetFishToSpawn Called");

            if (100 == UnityEngine.Random.Range(0, 100))
            {
                //bool isShiny = true;
            }

            float randomNumber = UnityEngine.Random.Range(0, 100);
            float currentChanceValue = 0;

            foreach (HybridsToSpawn hybrids in fishList)
            {
                currentChanceValue += hybrids.newSpawnChance;

                if (randomNumber <= currentChanceValue)
                {
                    return hybrids;
                }
            }

            Debug.LogError($"Hybrid was outside the random range with a value of {randomNumber}, spawning {fishList[1].hybridGameObject}");
            return fishList[1];
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
}
