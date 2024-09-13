using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum CropState
{
    Seed, Sprout, Adolecent, Mature, Harvested
}

public class CropData
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
    private List<CropData> cropsPlanted;
    [SerializeField]
    private float seedRadius = 0.3f; // Minimum distance between child objects
    [SerializeField]
    private SeedsSO[] seedSOS;

    private void OnEnable()
    {
        GameEvents.OnTempMorningEvent += GameEvents_OnTempMorningEvent;
        loadPlants();
    }

    private void OnDisable()
    {
        GameEvents.OnTempMorningEvent -= GameEvents_OnTempMorningEvent;
        SavePlants();
    }


    private void GameEvents_OnTempMorningEvent(int _Day, int _Hour, int _Min, int _Seconds)
    {
        GrowPlants();
    }

    [ContextMenu("GrowPlants")]
    public void TempGrowPlants()
    {
        GameEvents.TempMorningEvent(1, 1, 1, 1);
       
    }

    public IEnumerator PlantCrop(FOEItem_Seed _Seed, int index)
    {
        ableToPlant = false;
        yield return CheckIfCanPlant(_Seed.europaItemSO.itemTransform.position, _Seed.europaItemSO.ItemRB);

        if (ableToPlant == true)
        {
            CropData newitem = new CropData();
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
            foreach (CropData _Crop in cropsPlanted)
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

    private void GrowPlants()   //change to an Enumerator so each for loop is spaced out by a few seconds
    {
        foreach (CropData _crop in cropsPlanted)
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

    private void SpawnNewObject(GameObject _go , CropData _crop)
    {
        _OPM.ReturnObjectToPool(_crop.go);
        _crop.go = _OPM.SpawnObject(_go, _crop.seedTransform.position, _crop.seedTransform.rotation, PoolType.Plants);
    }

    private void loadPlants()
    {
         if(_TSM.currentSave.cropPlanted == null)
            return;

        for (int index = 0; index < _TSM.currentSave.cropPlanted.Count; index++) 
        {
            CropData loadItem = new CropData();

            foreach(SeedsSO seedID in seedSOS)
            {
                if (seedID.ItemID == _TSM.currentSave.cropPlanted[index].cropItem.itemID.Value)
                {
                    loadItem.ItemID = seedID.ItemID;
                    loadItem.seedSO = seedID;

                    loadItem.seedTransform.position = _TSM.currentSave.cropPlanted[index].cropItem.itemPosition.Value;
                    loadItem.seedTransform.rotation = Quaternion.Euler(_TSM.currentSave.cropPlanted[index].cropItem.itemRotation.Value);
                    loadItem.cropState = (CropState)_TSM.currentSave.cropPlanted[index].growthStage.Value;
                    loadItem.farnIndex = _TSM.currentSave.cropPlanted[index].cropItem.itemSaveLocation.Value;

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
        if(_TSM.currentSave.cropPlanted == null)
            return;

        _TSM.currentSave.cropPlanted.Clear();
        

        if (cropsPlanted == null)
            return;

        for (int i = 0; i < cropsPlanted.Count; i++)
        {
            CurrentCrop _temp = new CurrentCrop();
            _temp.cropItem.itemID.Value = cropsPlanted[i].ItemID;
            _temp.cropItem.itemPosition.Value = cropsPlanted[i].seedTransform.position;
            _temp.cropItem.itemRotation.Value = cropsPlanted[i].seedTransform.rotation.eulerAngles;
            _temp.cropItem.itemSaveLocation.Value = cropsPlanted[i].farnIndex;
            _temp.growthStage.Value = (int)cropsPlanted[i].cropState;


            _TSM.currentSave.cropPlanted.Add(_temp);
        }

    }
}
