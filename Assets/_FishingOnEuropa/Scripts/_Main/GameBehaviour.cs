
using UnityEngine;
using System.Collections.Generic;
using Obvious.Soap.Example;

namespace Europa
{
    public class GameBehaviour : MonoBehaviour
    {
        protected static ExsampleSingleton _ES { get { return ExsampleSingleton.instance; } }

        protected static GameManager _GM { get { return GameManager.instance; } }
        protected static ObjectPoolManager _OPM { get { return ObjectPoolManager.instance; } }
        protected static SaveManager _TSM { get { return SaveManager.instance; } }
        protected static SceneController _SC { get { return SceneController.instance; } }
        protected static PlayerControllerSingletonLink _PLAYER { get { return PlayerControllerSingletonLink.instance; } }
        protected static SettingsMannager _SETM { get { return SettingsMannager.instance; } }

        protected static FishNavigationManager _FNAVM { get { return FishNavigationManager.instance; } }
        protected static TrustManager _TM { get { return TrustManager.instance; } }
        protected static EconomyManager _EM { get { return EconomyManager.instance; } }


        //protected static FishingMiniGameManager _FMGM { get { return FishingMiniGameManager.instance; } }

      
        public Transform getClosestEnermy(Transform _origin, List<GameObject> _objects)
        {
            if (_objects == null || _objects.Count == 0)
                return null;

            float distance = Mathf.Infinity;
            Transform closest = null;

            foreach (GameObject go in _objects)
            {
                float currentDistance = Vector3.Distance(_origin.transform.position, go.transform.position);
                if (currentDistance < distance)
                {
                    closest = go.transform;
                    distance = currentDistance;
                }
            }
            return closest;
        }

        public FOEItem_Hybrid GetHybridFromID(int _ID , FishToSpawnSO allHybrids)
        {
            foreach (FOEItem_Hybrid _hybrid in allHybrids.FishToSpawnList)
            {
                if (_hybrid.europaItemSO.itemID == _ID)
                {
                    return _hybrid;
                }
            }
            return null;
        }

        public PoolType GetPoolTypeFromID(int _ID, AllItemsSO allItems)
        {
            foreach (FOEItem _Item in allItems.items)
            {
                if (_Item.europaItemSO.itemID == _ID)
                {
                    return _Item.europaItemSO.poolType;
                }
            }
            return PoolType.None;
        }

        public FOEItem GetItemFromID(int _ID, AllItemsSO allItems)
        {
            foreach (FOEItem _Item in allItems.items)
            {
                if (_Item.europaItemSO.itemID == _ID)
                {
                    return _Item;
                }
            }
            return null;
        }
    }
}

