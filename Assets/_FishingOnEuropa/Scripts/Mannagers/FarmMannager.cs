using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static FishSpawnerGeyser;

[Serializable]
public class FarmHybridData
{
    [Header("FarmFishData")]
    public int hybridID;
    public string hybridName;
    public bool shiney;
    public int tank;
    public Vector3 lastLocation;

    [Header("Hybrid GameObject")]
    public GameObject hybridGameObjectPrefab;
    public GameObject hybridGameObjectSpawnned;
    public PoolType poolType;

    [Header("Hybrid Nav")]
    public float hybridSpeed;
    public float hybridRotationSpeed;

    [Header("Hybrid Diet")]
    public FoodType foodEaten;
    public FoodList favFood;
    public ToyList favToy;


}

public class FarmMannager : GameBehaviour
{
    [SerializeField] List<HybridScriptableObjects> hybridInfos = new List<HybridScriptableObjects>();
    [SerializeField] List<FarmHybridData> hybridInFarm = new List<FarmHybridData>();
    [SerializeField] Transform[] tankOneSpawnPoints;
    [SerializeField] Transform[] tankTwoSpawnPoints;
    [SerializeField] Transform[] defaultGroundSpawnPoints;
    [SerializeField] FishNavigationManager fishNavigationManager;



    

    private void Start()
    {
        Invoke( "LoadHybrids" , 0.1f);
    }

    private void OnDisable()
    {
        DespawnHybrids();
    }

    private void LoadHybrids()
    {
        for (int i = 0; i < _TSM.saveDatas[0].HybridsInFarm.Count; i++)
        {
            foreach (HybridScriptableObjects _HI in hybridInfos)
            {
                if (_TSM.saveDatas[0].HybridsInFarm[i] == _HI.fishID)
                {
                    FarmHybridData farmHybridData = new FarmHybridData();

                    farmHybridData = GetDataFromSaveMannager(farmHybridData, i);
                    farmHybridData = GetDataFromScrptibleObject(farmHybridData, _HI);

                    AddHybridToFarm(farmHybridData);
                    spawnHybridInFarm(farmHybridData);
                }
            }

        }
    }
    /// <summary>
    /// Gets the Data From the Inputted Scriptable Object and Adds it to the FarmHybridData
    /// </summary>
    /// <param name="_farmHybridData"></param>
    /// <param name="_HybridInfo"></param>
    /// <returns></returns>
    private FarmHybridData GetDataFromScrptibleObject(FarmHybridData _farmHybridData, HybridScriptableObjects _HybridInfo )
    {
        _farmHybridData.hybridGameObjectPrefab = _HybridInfo.hybridPrefab;
        _farmHybridData.poolType = _HybridInfo.poolType;


        _farmHybridData.hybridSpeed = _HybridInfo.fishSpeed;
        _farmHybridData.hybridRotationSpeed = _HybridInfo.rotationSpeed;


        _farmHybridData.foodEaten = _HybridInfo.foodEaten;
        _farmHybridData.favFood = _HybridInfo.favFood;
        _farmHybridData.favToy = _HybridInfo.favToy;

        return _farmHybridData;
    }
    /// <summary>
    /// Gets the Data from the Save Manager and Adds it to the FarmHybridData
    /// </summary>
    /// <param name="_farmHybridData"></param>
    /// <param name="_Index"></param>
    /// <returns></returns>
    private FarmHybridData GetDataFromSaveMannager(FarmHybridData _farmHybridData, int _Index)
    {
        _farmHybridData.hybridID = _TSM.currentSave.HybridsInFarm[_Index];
        _farmHybridData.hybridName = _TSM.currentSave.HybridsNames[_Index];
        _farmHybridData.shiney = _TSM.currentSave.HybridsShiney[_Index];
        _farmHybridData.tank = _TSM.currentSave.HybridTank[_Index];
        _farmHybridData.lastLocation = _TSM.currentSave.HybridLastPos[_Index];

        return _farmHybridData;
    }

    /// <summary>
    /// adds a Hybrid to the farm
    /// </summary>
    /// <param name="_farmHybridData"></param>
    private void AddHybridToFarm(FarmHybridData _farmHybridData)
    {
        hybridInFarm.Add(_farmHybridData);
    }

    /// <summary>
    /// Gets the Hybrids From the Object pool Mannager and spawns them inside the correct tank at a random spawn Point
    /// </summary>
    /// <param name="_farmHybridData"></param>
    private void spawnHybridInFarm(FarmHybridData _farmHybridData)
    {
        Transform[] spawnPoints = null;
        Vector3 spawnTransform;

        switch (_farmHybridData.tank)
        {
            case 0:
                spawnPoints = tankOneSpawnPoints;
                spawnTransform = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
                break;
            case 1:
                spawnPoints = tankTwoSpawnPoints;
                spawnTransform = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
                break;
            case 2:
                spawnTransform = _farmHybridData.lastLocation;
                break;
            default:
                spawnPoints = tankOneSpawnPoints;
                spawnTransform = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
                break;

        }


        GameObject hybridFromManager = ObjectPoolManager._OPM.spawnObject(_farmHybridData.hybridGameObjectPrefab, spawnTransform, new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0), PoolType.ZoneFarmHybrids);
        _farmHybridData.hybridGameObjectSpawnned = hybridFromManager;
        fishNavigationManager.addHybridToPondList(hybridFromManager, HybridState.HybridFlying);
    }

    /// <summary>
    /// Despawns all of the Hybrids from the farm, Sends their Info back to the save Mannager
    /// </summary>
    private void DespawnHybrids()
    {
        int Index = 0;
        ClearSaveLists();

        foreach (FarmHybridData _farmHybridData in hybridInFarm)
        {

            AddHybridsToSave(_farmHybridData);

            if (_farmHybridData.hybridGameObjectSpawnned != null)
            {
                _OPM.ReturnObjectToPool(_farmHybridData.hybridGameObjectSpawnned);
            }
            Index++;
        }
    }

    /// <summary>
    /// clears the Hybrid Data Out of the SaveMannager
    /// </summary>
    private void ClearSaveLists()
    {
        _TSM.currentSave.HybridsInFarm.Clear();
        _TSM.currentSave.HybridsShiney.Clear();
        _TSM.currentSave.HybridsNames.Clear();
        _TSM.currentSave.HybridTank.Clear();
        _TSM.currentSave.HybridLastPos.Clear();
    }

    /// <summary>
    /// Adds a hybrid to the save List
    /// </summary>
    /// <param name="_farmHybridData"></param>
    private void AddHybridsToSave(FarmHybridData _farmHybridData)
    {
        _TSM.currentSave.HybridsInFarm.Add(_farmHybridData.hybridID);
        _TSM.currentSave.HybridsShiney.Add(_farmHybridData.shiney);
        _TSM.currentSave.HybridsNames.Add(_farmHybridData.hybridName);
        _TSM.currentSave.HybridTank.Add(_farmHybridData.tank);
        _TSM.currentSave.HybridLastPos.Add(_farmHybridData.hybridGameObjectSpawnned.transform.position);
    }

    private void RemoveHybridFromFarm(FarmHybridData _hybridInfo)
    {

    }
}
