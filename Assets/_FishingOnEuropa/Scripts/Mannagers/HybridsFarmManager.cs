using System;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;

namespace Europa
{


    public class HybridsFarmManager : GameBehaviour
    {
        [SerializeField] private FishToSpawnSO fishToSpawn;

        [SerializeField] List<FOEItem_Hybrid> hybridInfos = new List<FOEItem_Hybrid>();
        [SerializeField] Transform[] tankOneSpawnPoints;
        [SerializeField] Transform[] tankTwoSpawnPoints;
        [SerializeField] Transform[] defaultGroundSpawnPoints;

        [SerializeField]
        private float _mainTankWaterHight = 1f;
        [SerializeField]
        private float _windowTankWaterHight = 1f;

        [SerializeField]
        private ScriptableListFOEItem_Hybrid _hybridsToNavList;

        [SerializeField]
        private ScriptableListFOEItem_Hybrid _hybridsSwimmingList;


        [SerializeField]
        private ScriptableListFOESaveItem_Hybrid _hybridInFarm;

        private void Awake()
        {
            _hybridInFarm.OnItemAdded += _hybridInFarm_OnItemAdded;
            _hybridInFarm.OnItemRemoved += _hybridInFarm_OnItemRemoved;
        }



        private void OnDisable()
        {
            _hybridInFarm.OnItemAdded -= _hybridInFarm_OnItemAdded;
            _hybridInFarm.OnItemRemoved -= _hybridInFarm_OnItemRemoved;
        }

        private void _hybridInFarm_OnItemAdded(FOESaveItem_Hybrid _hybrid)
        {
            
                SpawnHybridInTank(_hybrid);
         

        }

        private void _hybridInFarm_OnItemRemoved(FOESaveItem_Hybrid _hybrid)
        {
            RemoveHybridFromTank(_hybrid);
        }

        public void OnFarmSceneLoaded()
        {
            foreach (var _hybrid in _hybridInFarm)
            {
                SpawnHybridInTank(_hybrid);
            }
        }

        public void OnFarmSceneUnLoaded()
        {
            foreach (var _hybrid in _hybridInFarm)
            {
                RemoveHybridFromTank(_hybrid);
            }
        }

        private void SpawnHybridInTank(FOESaveItem_Hybrid _hybrid)
        {
            Transform[] spawnPoints = null;
            Transform spawnPoint = tankOneSpawnPoints[0];
            float waterHight = 0;
            bool spawnHybrid = false;

            switch (_hybrid.itemLocation)
            {
                case ItemLocation.Tank1:
                    spawnPoints = tankOneSpawnPoints;
                    spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform;
                    spawnPoint.rotation = new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0);
                    waterHight = _mainTankWaterHight;
                    spawnHybrid = true;
                    break;

                case ItemLocation.Tank2:
                    spawnPoints = tankTwoSpawnPoints;
                    spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform;
                    spawnPoint.rotation = new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0);
                    waterHight = _windowTankWaterHight;
                    spawnHybrid = true;
                    break;
                case ItemLocation.World:
                    spawnPoint = _hybrid.itemPos;
                    waterHight = 0;
                    spawnHybrid = true;
                    break;

                default:

                    break;

            }

            if (spawnHybrid == false)
                return;

            FOEItem_Hybrid _baseHybridItem = GetHybridFromID(_hybrid.itemID, fishToSpawn);

            _hybrid.itemGO = _OPM.SpawnObject(_baseHybridItem.europaItemSO.itemPrefab, spawnPoint.position, spawnPoint.rotation, _baseHybridItem.europaItemSO.poolType);

            FOEItem_Hybrid _newHybridItem = _hybrid.itemGO.GetComponent<FOEItem_Hybrid>();

            _hybridsToNavList.Add(_newHybridItem);

            _newHybridItem.InitSavedHybrid(_hybrid, spawnPoint, waterHight, _hybrid.itemLocation, HybridState.HybridFlocking, HybridVisualsState.World);

            _hybridsSwimmingList.Add(_newHybridItem);

            //_FNAVM.AddHybridTolist(_newHybridItem, HybridState.HybridFlocking);

        }

        private void RemoveHybridFromTank(FOESaveItem_Hybrid _hybrid)
        {
            _OPM.ReturnObjectToPool(_hybrid.itemGO);
        }
    }
}






