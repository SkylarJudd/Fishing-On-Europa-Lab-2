using System;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;

namespace Europa
{


    public class HybridsFarmManager : GameBehaviour
    {
        [SerializeField] List<FOEItem_Hybrid> hybridInfos = new List<FOEItem_Hybrid>();
        [SerializeField] Transform[] tankOneSpawnPoints;
        [SerializeField] Transform[] tankTwoSpawnPoints;
        [SerializeField] Transform[] defaultGroundSpawnPoints;

        [SerializeField]
        private float _mainTankWaterHight;
        [SerializeField]
        private float _windowTankWaterHight;

        [SerializeField]
        private ScriptableListFOEItem_Hybrid _hybridsToNavList;

        [SerializeField]
        private ScriptableListFOEItem_Hybrid _hybridInFarm;

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

        private void _hybridInFarm_OnItemAdded(FOEItem_Hybrid _hybrid)
        {
            if (!_hybridInFarm.Contains(_hybrid))
            {
                SpawnHybridInTank(_hybrid);
            }
            Debug.LogWarning($"{_hybrid.europaItemSO.itemName} Is already in the list");

        }

        private void _hybridInFarm_OnItemRemoved(FOEItem_Hybrid _hybrid)
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

        private void SpawnHybridInTank(FOEItem_Hybrid _hybrid)
        {

            Transform[] spawnPoints = null;
            Transform spawnTransform;
            float waterHight;

            switch (_hybrid.navigationData.hybridLocation)
            {
                case HybridLocation.Tank1:
                    spawnPoints = tankOneSpawnPoints;
                    spawnTransform = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform;
                    spawnTransform.rotation = new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0);
                    waterHight = _mainTankWaterHight;
                    break;

                case HybridLocation.Tank2:
                    spawnPoints = tankTwoSpawnPoints;
                    spawnTransform = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform;
                    spawnTransform.rotation = new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0);
                    waterHight = _windowTankWaterHight;
                    break;
                case HybridLocation.World:
                    spawnTransform = _hybrid.europaItemSO.itemTransform;
                    waterHight = 0;
                    break;

                default:
                    spawnPoints = tankOneSpawnPoints;
                    spawnTransform = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform;
                    spawnTransform.rotation = new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0);
                    waterHight = _mainTankWaterHight;
                    break;

            }

            GameObject hybridFromManager = _OPM.SpawnObject(_hybrid.europaItemSO.itemPrefab, spawnTransform.position, spawnTransform.rotation, PoolType.ZoneFarmHybrids);

            FOEItem_Hybrid hybridData = hybridFromManager.GetComponent<FOEItem_Hybrid>();

            hybridData.InitSavedHybrid(_hybrid, spawnTransform, waterHight, _hybrid.navigationData.hybridLocation, _hybrid.navigationData.hybridState, HybridVisualsState.World);

            _hybridsToNavList.Add(hybridData);

        }

        private void RemoveHybridFromTank(FOEItem_Hybrid _hybrid)
        {
            _OPM.ReturnObjectToPool(_hybrid.europaItemSO.itemGO);
        }
    }
}






