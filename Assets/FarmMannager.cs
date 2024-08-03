using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishSpawnerGeyser;

[Serializable]
public class FarmHybridData
{
    [Header("FarmFishData")]
    public int hybridID;
    public string hybridName;
    public bool shiney;

    [Header("Hybrid GameObject")]
    public GameObject hybridGameObject;
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
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] FishNavigationManager fishNavigationManager;



    private void Start()
    {
        GetHybridsFromSave();
    }

    private void GetHybridsFromSave()
    {
        for (int i = 0; i < _TSM.HybridsInFarm.Count; i++)
        {
            foreach (HybridScriptableObjects _HI in hybridInfos)
            {
                if (_TSM.HybridsInFarm[i] == _HI.fishID)
                {
                    FarmHybridData farmHybridData = new FarmHybridData();


                    farmHybridData.hybridID = _TSM.HybridsInFarm[i];
                    farmHybridData.hybridName = _TSM.HybridsNames[i];
                    farmHybridData.shiney = _TSM.HybridsShiney[i];


                    farmHybridData.hybridGameObject = _HI.hybridPrefab;
                    farmHybridData.poolType = _HI.poolType;


                    farmHybridData.hybridSpeed = _HI.fishSpeed;
                    farmHybridData.hybridRotationSpeed = _HI.rotationSpeed;


                    farmHybridData.foodEaten = _HI.foodEaten;
                    farmHybridData.favFood = _HI.favFood;
                    farmHybridData.favToy = _HI.favToy;



                    AddHybridToFarm(farmHybridData);
                    spawnHybridInFarm(farmHybridData);
                }
            }

        }
    }

    private void AddHybridToFarm(FarmHybridData _hybridInfo)
    {
        hybridInFarm.Add(_hybridInfo);
        
    }

    private void spawnHybridInFarm(FarmHybridData _hybridInfo)
    {
        GameObject hybridFromManager = ObjectPoolManager._OPM.spawnObject(_hybridInfo.hybridGameObject, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position, new Quaternion(0, UnityEngine.Random.Range(0,360), 0 , 0), _hybridInfo.poolType);
        fishNavigationManager.addHybridToPondList(hybridFromManager, HybridState.HybridFlying);
    }
    private void RemoveHybridFromFarm(FarmHybridData _hybridInfo)
    {

    }
}
