using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserMannager : MonoBehaviour
{
    [SerializeField] List<GameObject> geysers = new List<GameObject>();
    [SerializeField] GameObject fishMannager;

    [SerializeField] int _startSpawnAmout = 1;

    public Transform waterHight;

    Coroutine spawnHybridsOnstart;

    private void Start()
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

        spawnHybridsOnstart = StartCoroutine(SpawnHybridsOnStart());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SpawnAHybrid();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SpawnAHybrid();
        }
    }
    private IEnumerator SpawnHybridsOnStart()
    {
        float _spawnCooldown = 1f;
        

        for (int i = 0; i < _startSpawnAmout; i++)
        {
            _spawnCooldown = UnityEngine.Random.Range(0.1f, 0.5f);
            yield return new WaitForSeconds(_spawnCooldown);
            //print($"Spawn Amout = {_startSpawnAmout} i = {i}");
            SpawnAHybrid();
        }
    }

    public void SpawnAHybrid()
    {

        FishSpawnerGeyser fishSpawnerGeyser = geysers[UnityEngine.Random.Range(0, geysers.Count)].GetComponent<FishSpawnerGeyser>();
        if(fishSpawnerGeyser == null)
        {
            Debug.LogError("Unable to find FishSpawnerGeyser");
        }

        fishSpawnerGeyser.spawnHybrid();
    }
}
