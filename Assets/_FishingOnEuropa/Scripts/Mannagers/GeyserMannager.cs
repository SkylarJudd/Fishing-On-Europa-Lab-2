using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
   

    public class GeyserMannager : MonoBehaviour
    {
        [Header("Pond Data")]
        public Transform waterHeight;
        public HybridLocation pondType;

        [Header("Geyser List")]
        [SerializeField]
        private List<GameObject> geysers = new List<GameObject>();

        [Header("Spawning")]
        [SerializeField]
        private float spawnCooldown = 1f;
        [SerializeField]
        private int startSpawnAmout = 25;


        private Coroutine spawnHybridsOnstart;

        private void Start()
        {
            Inizilize();
        }


        /// <summary>
        /// Finds all of the Geysers that are of a child of this gameobject and adds them to an array and starts the spawn Coroutine
        /// </summary>
        private void Inizilize()
        {
            // Find all objects of type FishSpawnerGeyser and convert the array to a list
            FishSpawnerGeyser[] geyserArray = GetComponentsInChildren<FishSpawnerGeyser>();

            // Clear the existing list
            geysers.Clear();

            // Add the found objects to the geysers list
            foreach (FishSpawnerGeyser geyser in geyserArray)
            {
                geysers.Add(geyser.gameObject);
            }

            spawnHybridsOnstart = StartCoroutine(SpawnAllHybrids(startSpawnAmout, spawnCooldown));
        }

        /// <summary>
        /// A Corutine that will spawns the amout of inputted Hybrids
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnAllHybrids(int _spawnAmount, float _coolDown)
        {
            float _spawnCooldown = 1f;


            for (int i = 0; i < _spawnAmount; i++)
            {
                _spawnCooldown = UnityEngine.Random.Range(_coolDown * 0.5f, _coolDown * 1.5f);
                yield return new WaitForSeconds(_spawnCooldown);
                //print($"Spawn Amout = {_startSpawnAmout} i = {i}");
                SpawnAHybrid();
            }
        }

        /// <summary>
        /// Sends a message to a random geyser to spawn a hybrid
        /// </summary>
        private void SpawnAHybrid()
        {

            FishSpawnerGeyser fishSpawnerGeyser = geysers[UnityEngine.Random.Range(0, geysers.Count)].GetComponent<FishSpawnerGeyser>();
            if (fishSpawnerGeyser == null)
            {
                Debug.LogError("Unable to find FishSpawnerGeyser");
            }

            fishSpawnerGeyser.SpawnHybrid();
        }
    }
}

