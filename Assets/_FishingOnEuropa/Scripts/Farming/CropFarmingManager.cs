using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Obvious.Soap;
using Europa.GameEvents;

namespace Europa
{
    public class CropFarmingManager : GameBehaviour
    {
        [Header("Farming Manager")]
        [SerializeField]
        private GameObject Soil;

        private bool ableToPlant;


        [Header("Crops")]
        [SerializeField]
        private ScriptableListCropData cropList;
        [SerializeField]
        private float seedRadius = 0.3f; // Minimum distance between child objects
        [SerializeField]
        private SeedsSO[] seedsSOs;

        private void OnEnable()
        {
            DailyEventHandler.SubscribeDailyEvent(DailyEvents.Sunrise, GameEvents_OnTempMorningEvent);

            cropList.OnItemAdded += CropList_OnItemAdded;
            cropList.OnItemRemoved += CropList_OnItemRemoved;


        }

        private void OnDisable()
        {
            cropList.OnItemAdded -= CropList_OnItemAdded;
            cropList.OnItemRemoved -= CropList_OnItemRemoved;


        }

        public void OnFarmSceneLoaded()
        {
            foreach (var _crop in cropList)
            {
                SpawnCrop(_crop);
            }
        }

        public void OnFarmSceneUnLoaded()
        {
            foreach (var _crop in cropList)
            {
                RemoveCrop(_crop);
            }
        }

        private void CropList_OnItemAdded(FOEItem_Crop _crop)
        {
            SpawnCrop(_crop);
        }

        private void CropList_OnItemRemoved(FOEItem_Crop _crop)
        {
            RemoveCrop(_crop);
        }

        private void SpawnCrop(FOEItem_Crop _crop)
        {
            foreach (SeedsSO seedID in seedsSOs)
            {
                if (seedID.ItemID == _crop.europaItemSO.itemID)
                {
                    _crop.go = _OPM.SpawnObject(seedID.growthStages[(int)_crop.cropState], _crop.itemTransform.position, _crop.itemTransform.rotation, PoolType.Plants);
                    if (_crop.go.TryGetComponent<PlantGrowth>(out PlantGrowth growthObjectExample))
                    {
                        growthObjectExample.ItemLoaded(_crop.ItemLastUnloadedTime, _crop.growthData);
                    }
                }
            }
        }

        private void RemoveCrop(FOEItem_Crop _crop)
        {
            _OPM.ReturnObjectToPool(_crop);

            if (_crop.go.TryGetComponent<PlantGrowth>(out PlantGrowth growthObjectExample))
            {
                growthObjectExample.ItemUnloaded();
            }
        }


        private void GameEvents_OnTempMorningEvent(bool wasObserved, int cycles = 1)
        {
            GrowPlants();
        }

        [ContextMenu("GrowPlants")]
        public void TempGrowPlants()
        {
            DailyEventHandler.InvokeDailyEvent(DailyEvents.Sunrise);

        }

        public IEnumerator PlantCrop(FOEItem_Seed _Seed, int index)
        {
            ableToPlant = false;
            yield return CheckIfCanPlant(_Seed.europaItemData.itemTransform.position, _Seed.europaItemData.itemRB);

            if (ableToPlant == true)
            {
                FOEItem_Crop newitem = new FOEItem_Crop();
                //newitem.go = _Seed.gameObject;
                newitem.europaItemSO.itemID = _Seed.europaItemSO.itemID;
                newitem.itemTransform = _Seed.europaItemData.itemTransform;
                newitem.cropState = CropState.Seed;
                newitem.farmIndex = index;


                _Seed.europaItemData.itemRB.isKinematic = true;
                _Seed.europaItemData.itemRB.useGravity = false;
                _Seed.europaItemData.itemGrabbable.isGrabbable = false;
                //_Seed.transform.rotation = new Quaternion(0, 0, 0, 0);

                cropList.Add(newitem);

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
                foreach (FOEItem_Crop _Crop in cropList)
                {
                    Vector3 _CropSeedPos = _Crop.itemTransform.position;
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
            foreach (FOEItem_Crop _crop in cropList)
            {
                switch (_crop.cropState)
                {
                    case CropState.Seed:
                        _crop.cropState = CropState.Sprout;
                        SpawnNewObject(_crop.seedSO.growthStages[1], _crop);
                        break;
                    case CropState.Sprout:
                        _crop.cropState = CropState.Flowering;
                        SpawnNewObject(_crop.seedSO.growthStages[2], _crop);
                        break;
                    case CropState.Flowering:
                        _crop.cropState = CropState.Fruited;
                        SpawnNewObject(_crop.seedSO.growthStages[3], _crop);
                        break;
                    case CropState.Fruited:
                        _crop.cropState = CropState.Harvested;
                        SpawnNewObject(_crop.seedSO.growthStages[4], _crop);
                        break;
                    case CropState.Harvested:
                        cropList.Remove(_crop);
                        break;

                }
            }
        }

        private void SpawnNewObject(GameObject _go, FOEItem_Crop _crop)
        {
            _OPM.ReturnObjectToPool(_crop);
            _crop.go = _OPM.SpawnObject(_go, _crop.itemTransform.position, _crop.itemTransform.rotation, PoolType.Plants);
        }

    }
}

