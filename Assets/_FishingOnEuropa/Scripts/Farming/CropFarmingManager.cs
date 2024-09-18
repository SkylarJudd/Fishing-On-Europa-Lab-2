using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Obvious.Soap;

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
            GameEvents.OnTempMorningEvent += GameEvents_OnTempMorningEvent;
            cropList.OnItemAdded += CropList_OnItemAdded;
            cropList.OnItemRemoved += CropList_OnItemRemoved;

            
        }

        private void OnDisable()
        {
            GameEvents.OnTempMorningEvent -= GameEvents_OnTempMorningEvent;
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

        private void CropList_OnItemAdded(CropData _crop)
        {
            SpawnCrop(_crop);
        }

        private void CropList_OnItemRemoved(CropData _crop)
        {
            RemoveCrop(_crop);
        }

        private void SpawnCrop(CropData _crop)
        {
            foreach (SeedsSO seedID in seedsSOs)
            {
                if (seedID.ItemID == _crop.itemID)
                {
                    _crop.go = _OPM.SpawnObject(seedID.growthStages[(int)_crop.cropState], _crop.itemTransform.position, _crop.itemTransform.rotation, PoolType.Plants);
                }
            }
        }

        private void RemoveCrop(CropData _crop)
        {
            _OPM.ReturnObjectToPool(_crop.go);
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
            yield return CheckIfCanPlant(_Seed.europaItemSO.itemTransform.position, _Seed.europaItemSO.itemRB);

            if (ableToPlant == true)
            {
                CropData newitem = new CropData();
                newitem.go = _Seed.gameObject;
                newitem.itemID = _Seed.europaItemSO.itemID;
                newitem.itemTransform = _Seed.europaItemSO.itemTransform;
                newitem.cropState = CropState.Seed;
                newitem.farmIndex = index;


                _Seed.europaItemSO.itemRB.isKinematic = true;
                _Seed.europaItemSO.itemRB.useGravity = false;
                _Seed.europaItemSO.itemGrabbable.isGrabbable = false;
                _Seed.transform.rotation = new Quaternion(0, 0, 0, 0);

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
                foreach (CropData _Crop in cropList)
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
            foreach (CropData _crop in cropList)
            {
                switch (_crop.cropState)
                {
                    case CropState.Seed:
                        _crop.cropState = CropState.Sprout;
                        SpawnNewObject(_crop.seedSO.growthStages[1], _crop);
                        break;
                    case CropState.Sprout:
                        _crop.cropState = CropState.Adolecent;
                        SpawnNewObject(_crop.seedSO.growthStages[2], _crop);
                        break;
                    case CropState.Adolecent:
                        _crop.cropState = CropState.Mature;
                        SpawnNewObject(_crop.seedSO.growthStages[3], _crop);
                        break;
                    case CropState.Mature:
                        _crop.cropState = CropState.Harvested;
                        SpawnNewObject(_crop.seedSO.growthStages[4], _crop);
                        break;
                    case CropState.Harvested:
                        cropList.Remove(_crop);
                        break;

                }
            }
        }

        private void SpawnNewObject(GameObject _go, CropData _crop)
        {
            _OPM.ReturnObjectToPool(_crop.go);
            _crop.go = _OPM.SpawnObject(_go, _crop.itemTransform.position, _crop.itemTransform.rotation, PoolType.Plants);
        }

    }
}

