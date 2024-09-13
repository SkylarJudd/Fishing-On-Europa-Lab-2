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
    public ItemSaveLocation tank;
    public Transform hybridTransform;

    [Header("Hybrid GameObject")]
    public FOEItem hybridGameObjectPrefab;
    public GameObject hybridGameObjectSpawnned;
    public PoolType poolType;

    [Header("Hybrid Nav")]
    public float hybridSpeed;
    public float hybridRotationSpeed;

    [Header("Hybrid Diet")]
    public FoodType[] foodEaten;
    public FoodList favFood;
    public ToyList favToy;


}

public class FarmMannager : GameBehaviour
{
    [SerializeField] List<FOEItem_Hybrid> hybridInfos = new List<FOEItem_Hybrid>();
    [SerializeField] List<FarmHybridData> hybridInFarm = new List<FarmHybridData>();
    [SerializeField] Transform[] tankOneSpawnPoints;
    [SerializeField] Transform[] tankTwoSpawnPoints;
    [SerializeField] Transform[] defaultGroundSpawnPoints;
    



    

    private void Start()
    {
        //Invoke( "LoadHybrids" , 0.1f);
    }

    private void OnDisable()
    {
        DespawnHybrids();
    }

    private void LoadHybrids()
    {
        for (int i = 0; i < _TSM.currentSave.hybridFarmList.Count; i++)
        {
            foreach (FOEItem_Hybrid _HI in hybridInfos)
            {
                if (_TSM.currentSave.hybridFarmList[i].hybridItem.itemID.Value == _HI.europaItemSO.itemID)
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
    private FarmHybridData GetDataFromScrptibleObject(FarmHybridData _farmHybridData, FOEItem_Hybrid _HybridInfo )
    {
        _farmHybridData.hybridGameObjectPrefab = _HybridInfo.europaItemSO.worldObject;
        _farmHybridData.poolType = _HybridInfo.hybridSO.hybridPoolType;


        _farmHybridData.hybridSpeed = _HybridInfo.hybridSO.fishSpeed;
        _farmHybridData.hybridRotationSpeed = _HybridInfo.hybridSO.rotationSpeed;


        _farmHybridData.foodEaten = _HybridInfo.hybridSO.foodEaten;
        _farmHybridData.favFood = _HybridInfo.hybridSO.favFood;
        _farmHybridData.favToy = _HybridInfo.hybridSO.favToy;

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
        _farmHybridData.hybridID = _TSM.currentSave.hybridFarmList[_Index].hybridItem.itemID.Value;
        _farmHybridData.hybridName = _TSM.currentSave.hybridFarmList[_Index].hybridName.Value;
        _farmHybridData.shiney = _TSM.currentSave.hybridFarmList[_Index].hybridShiny.Value;
        _farmHybridData.tank =(ItemSaveLocation)_TSM.currentSave.hybridFarmList[_Index].hybridItem.itemSaveLocation.Value;
        _farmHybridData.hybridTransform.position = _TSM.currentSave.hybridFarmList[_Index].hybridItem.itemPosition.Value;
        _farmHybridData.hybridTransform.rotation = Quaternion.Euler(_TSM.currentSave.hybridFarmList[_Index].hybridItem.itemRotation.Value);

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
    /// Gets the Hybrids From the Object pool Manager and spawns them inside the correct tank at a random spawn Point
    /// </summary>
    /// <param name="_farmHybridData"></param>
    private void spawnHybridInFarm(FarmHybridData _farmHybridData)
    {
        Transform[] spawnPoints = null;
        Transform spawnTransform;

        switch (_farmHybridData.tank)
        {
            case ItemSaveLocation.Tank1:
                spawnPoints = tankOneSpawnPoints;
                spawnTransform = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform;
                spawnTransform.rotation = new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0);
                break;
            case ItemSaveLocation.Tank2:
                spawnPoints = tankTwoSpawnPoints;
                spawnTransform = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform;
                spawnTransform.rotation = new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0);
                break;
            case ItemSaveLocation.World:
                spawnTransform = _farmHybridData.hybridTransform;
                break;
            default:
                spawnPoints = tankOneSpawnPoints;
                spawnTransform = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform;
                spawnTransform.rotation = new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0);
                break;

        }


        GameObject hybridFromManager = ObjectPoolManager._OPM.SpawnObject(_farmHybridData.hybridGameObjectPrefab.gameObject, spawnTransform.position, spawnTransform.rotation, PoolType.ZoneFarmHybrids);
        _farmHybridData.hybridGameObjectSpawnned = hybridFromManager;
        _FNAVM.addHybridToPondList(hybridFromManager, HybridState.HybridFlying, 0 , PondType.Farm);
    }

    /// <summary>
    /// Despawns all of the Hybrids from the farm, Sends their Info back to the save Manager
    /// </summary>
    private void DespawnHybrids()
    {
        int Index = 0;
        //ClearSaveLists();

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
        _TSM.currentSave.hybridFarmList.Clear();

    }

    /// <summary>
    /// Adds a hybrid to the save List
    /// </summary>
    /// <param name="_farmHybridData"></param>
    private void AddHybridsToSave(FarmHybridData _farmHybridData)
    {
        CurrentHybrid _temp = new CurrentHybrid();
        _temp.hybridItem.itemID.Value = _farmHybridData.hybridID;
        _temp.hybridName.Value = _farmHybridData.hybridName;
        _temp.hybridShiny.Value = _farmHybridData.shiney;
        _temp.hybridItem.itemPosition.Value = _farmHybridData.hybridGameObjectSpawnned.transform.position;
        _temp.hybridItem.itemRotation.Value = _farmHybridData.hybridGameObjectSpawnned.transform.rotation.eulerAngles;
        _temp.hybridItem.itemSaveLocation.Value = (int)_farmHybridData.tank;

        _TSM.currentSave.hybridFarmList.Add(_temp);
        
    }

    private void RemoveHybridFromFarm(FarmHybridData _hybridInfo)
    {

    }
}
