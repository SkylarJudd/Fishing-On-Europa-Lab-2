using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Europa.GameEvents;
public enum CropState
{
    Seed, Sprout, Adolecent, Mature, Harvested
}

public class Crop
{
    public GameObject go;
    public int ItemID;
    public SeedsSO seedSO;
    public Transform seedTransform;
    public CropState cropState;
    public int farnIndex;
}


public class CropFarmingMannager : GameBehaviour
{
    [Header("Farming Mannager")]
    [SerializeField]
    private GameObject Soil;

    private bool ableToPlant;


    [Header("Crops")]
    [SerializeField]
    private List<Crop> cropsPlanted;
    [SerializeField]
    private float seedRadius = 0.3f; // Minimum distance between child objects
    [SerializeField]
    private SeedsSO[] seedSOS;

    private void OnEnable()
    {
        GameEvents.SubscribeDailyEvent(DailyEvents.Sunrise, GameEvents_OnTempMorningEvent);
        loadPlants();
    }

    private void OnDisable()
    {
        GameEvents.UnsubscribeDailyEvent(DailyEvents.Sunrise, GameEvents_OnTempMorningEvent);
        SavePlants();
    }

    private void GameEvents_OnTempMorningEvent(bool wasObserved, int cycles)
    {
        for (int i = 0; i < cycles; i++)
        {
            GrowPlants();
        }
    }

    [ContextMenu("GrowPlants")]
    public void TempGrowPlants()
    {
        GameEvents.InvokeDailyEvent(DailyEvents.Sunrise, true, 1);

    }

    public IEnumerator PlantCrop(FOEItem_Seed _Seed, int index)
    {
        ableToPlant = false;
        yield return CheckIfCanPlant(_Seed.europaItemSO.itemTransform.position, _Seed.europaItemSO.ItemRB);

        if (ableToPlant == true)
        {
            Crop newitem = new Crop();
            newitem.go = _Seed.gameObject;
            newitem.ItemID = _Seed.europaItemSO.itemID;
            newitem.seedTransform = _Seed.europaItemSO.itemTransform;
            newitem.cropState = CropState.Seed;
            newitem.farnIndex = index;


            _Seed.europaItemSO.ItemRB.isKinematic = true;
            _Seed.europaItemSO.ItemRB.useGravity = false;
            _Seed.europaItemSO.ItemGrabbable.isGrabbable = false;
            _Seed.transform.rotation = new Quaternion(0, 0, 0, 0);

            cropsPlanted.Add(newitem);

        }
        else
        {
            _Seed.bubbleMovement.Release();
        }
    }

    private IEnumerator CheckIfCanPlant(Vector3 _PlantSeedPos, Rigidbody _rb)
    {
        int attempts = 0;
        while (attempts < 4)
        {
            foreach (Crop _Crop in cropsPlanted)
            {
                Vector3 _CropSeedPos = _Crop.seedTransform.position;
                float _distance = Vector3.Distance(_CropSeedPos, _PlantSeedPos);
                if (_distance < seedRadius)
                {
                    float _force = 0.1f;
                    _rb.AddExplosionForce(_force, _CropSeedPos, seedRadius);
                    attempts++;
                }
                else
                {
                    ableToPlant = true;
                    break;
                }
            }
            if (ableToPlant)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void GrowPlants()   //change to an Enumarator so each for loop is spaced out by a few seconds
    {
        foreach (Crop _crop in cropsPlanted)
        {
            switch (_crop.cropState)
            {
                case CropState.Seed:
                    _crop.cropState = CropState.Sprout;
                    SpawnNewObject(_crop.seedSO.sprout, _crop);
                    break;
                case CropState.Sprout:
                    SpawnNewObject(_crop.seedSO.adolecent, _crop);
                    _crop.cropState = CropState.Adolecent;
                    break;
                case CropState.Adolecent:
                    _crop.cropState = CropState.Mature;
                    SpawnNewObject(_crop.seedSO.mature, _crop);
                    break;
                case CropState.Mature:
                    _crop.cropState = CropState.Harvested;
                    SpawnNewObject(_crop.seedSO.harvested, _crop);
                    break;
                case CropState.Harvested:
                    _OPM.ReturnObjectToPool(_crop.go);
                    cropsPlanted.Remove(_crop);
                    break;

            }
        }
    }

    private void SpawnNewObject(GameObject _go, Crop _crop)
    {
        _OPM.ReturnObjectToPool(_crop.go);
        _crop.go = _OPM.SpawnObject(_go, _crop.seedTransform.position, _crop.seedTransform.rotation, PoolType.Plants);
    }

    private void loadPlants()
    {
        if (_TSM.currentSave.itemsInFarm == null)
            return;

        for (int index = 0; index < _TSM.currentSave.itemsInFarm.Count; index++)
        {
            Crop loadItem = new Crop();

            foreach (SeedsSO seedID in seedSOS)
            {
                if (seedID.ItemID == _TSM.currentSave.itemsInFarm[index])
                {
                    loadItem.ItemID = seedID.ItemID;
                    loadItem.seedSO = seedID;

                    loadItem.seedTransform.position = _TSM.currentSave.plantLocations[index];
                    loadItem.seedTransform.rotation = new Quaternion(0, 0, 0, 0);
                    loadItem.cropState = (CropState)_TSM.currentSave.growthStage[index];
                    loadItem.farnIndex = _TSM.currentSave.farmLocatedIn[index];

                    switch (loadItem.cropState)
                    {
                        case CropState.Seed:
                            loadItem.go = _OPM.SpawnObject(loadItem.seedSO.seed, loadItem.seedTransform.position, loadItem.seedTransform.rotation, PoolType.Seeds);
                            break;
                        case CropState.Sprout:
                            loadItem.go = _OPM.SpawnObject(loadItem.seedSO.sprout, loadItem.seedTransform.position, loadItem.seedTransform.rotation, PoolType.Plants);
                            break;
                        case CropState.Adolecent:
                            loadItem.go = _OPM.SpawnObject(loadItem.seedSO.adolecent, loadItem.seedTransform.position, loadItem.seedTransform.rotation, PoolType.Plants);
                            break;
                        case CropState.Mature:
                            loadItem.go = _OPM.SpawnObject(loadItem.seedSO.mature, loadItem.seedTransform.position, loadItem.seedTransform.rotation, PoolType.Plants);
                            break;
                        case CropState.Harvested:
                            loadItem.go = _OPM.SpawnObject(loadItem.seedSO.harvested, loadItem.seedTransform.position, loadItem.seedTransform.rotation, PoolType.Plants);
                            break;
                    }
                }
            }
        }
    }

    [ContextMenu("Save")]
    private void SavePlants()
    {
        if (_TSM.currentSave.itemsInFarm == null)
            return;

        _TSM.currentSave.itemsInFarm.Clear();
        _TSM.currentSave.plantLocations.Clear();
        _TSM.currentSave.growthStage.Clear();
        _TSM.currentSave.farmLocatedIn.Clear();

        if (cropsPlanted == null)
            return;

        for (int i = 0; i < cropsPlanted.Count; i++)
        {
            _TSM.currentSave.itemsInFarm.Add(cropsPlanted[i].ItemID);
            _TSM.currentSave.plantLocations.Add(cropsPlanted[i].seedTransform.position);
            _TSM.currentSave.growthStage.Add((int)cropsPlanted[i].cropState);
            _TSM.currentSave.farmLocatedIn.Add(cropsPlanted[i].farnIndex);
        }

    }
}
