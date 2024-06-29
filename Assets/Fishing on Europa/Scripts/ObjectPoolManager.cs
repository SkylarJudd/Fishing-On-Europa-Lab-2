using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;


public enum PoolType
{
    ZoneOneHybrids,
    ZoneTwoHybrids,
    ZoneThreeHybrids,
    ZoneFarmHybrids,
    Seeds,
    Plants,
    Food,
    Particales,
    Sounds,
    None,

}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager OPM_Instance { get; private set; }
    public List<PooledObjectInfo> objectPools = new List<PooledObjectInfo>();

    #region ObjectPoolEmpites
    private GameObject _objectPoolEmptyHolder;

    private GameObject _hybridsEmptyHolder;
    private GameObject _farmEmptyHolder;

    private GameObject _zoneOneHybridsEmpty;
    private GameObject _zoneTwoHybridsEmpty;
    private GameObject _zoneThreeHybridsEmpty;
    private GameObject _zoneFarmHybridsEmpty;

    private GameObject _seedsEmpty;
    private GameObject _plantsEmpty;
    private GameObject _foodEmpty;

    private GameObject _particalesEmpty;
    private GameObject _sounds;
    #endregion

   

    public PoolType PoolingType;

    private void Awake()
    {
        if (OPM_Instance != null && OPM_Instance != this)
        {
            Destroy(this);
        }
        else
        {
            OPM_Instance = this;
        }

        SetUpEmpties();
    }

    private void SetUpEmpties()
    {

        #region CreatingObjectPoolEmpties
        _objectPoolEmptyHolder = new GameObject("PooledObjects");

        // Initialize these objects before using them
        _hybridsEmptyHolder = new GameObject("HybridsPool");
        _hybridsEmptyHolder.transform.SetParent(_objectPoolEmptyHolder.transform);

        _farmEmptyHolder = new GameObject("FarmingPool");
        _farmEmptyHolder.transform.SetParent(_objectPoolEmptyHolder.transform);

        // Hybrids
        CreateAndParent(ref _zoneOneHybridsEmpty, _hybridsEmptyHolder, "ZoneOne_Hybrids");
        CreateAndParent(ref _zoneTwoHybridsEmpty, _hybridsEmptyHolder, "ZoneTwo_Hybrids");
        CreateAndParent(ref _zoneThreeHybridsEmpty, _hybridsEmptyHolder, "ZoneThree_Hybrids");
        CreateAndParent(ref _zoneFarmHybridsEmpty, _hybridsEmptyHolder, "ZoneFarm_Hybrids");

        // Farming
        CreateAndParent(ref _seedsEmpty, _farmEmptyHolder, "SeedsPools");
        CreateAndParent(ref _plantsEmpty, _farmEmptyHolder, "PlantsPools");
        CreateAndParent(ref _foodEmpty, _farmEmptyHolder, "FoodPools");

        // Other
        CreateAndParent(ref _particalesEmpty, _objectPoolEmptyHolder, "ParticalPools");
        CreateAndParent(ref _sounds, _objectPoolEmptyHolder, "SoundsPools");
        #endregion



    }

    private void CreateAndParent(ref GameObject child, GameObject parent, string name)
    {
        child = new GameObject(name);
        child.transform.SetParent(parent.transform);
    }

    private GameObject SetParentObject(PoolType poolType)
    {
        #region PoolAssignmentNotes
        //ZoneOneHybrids = _zoneOneHybridsEmpty
        //ZoneTwoHybrids = _zoneTwoHybridsEmpty
        //ZoneThreeHybrids  = _zoneThreeHybridsEmpty
        //ZoneFarmHybrids  = _zoneFarmHybridsEmpty
        //Seeds = _seedsEmpty
        //Plants = _plantsEmpty
        //Food =  _foodEmpty
        //Particales = _particalesEmpty
        //Sounds = _sounds
        //None
        #endregion

        switch (poolType)
        {
            case PoolType.ZoneOneHybrids:
                return _zoneOneHybridsEmpty;

            case PoolType.ZoneTwoHybrids:
                return _zoneTwoHybridsEmpty;

            case PoolType.ZoneThreeHybrids:
                return _zoneThreeHybridsEmpty;

            case PoolType.ZoneFarmHybrids:
                return _zoneFarmHybridsEmpty;

            case PoolType.Seeds:
                return _seedsEmpty;

            case PoolType.Plants:
                return _plantsEmpty;

            case PoolType.Food:
                return _foodEmpty;

            case PoolType.Particales:
                return _particalesEmpty;

            case PoolType.Sounds:
                return _sounds;

            case PoolType.None:
                return null;

            default:
                return null;

        }
    }

    public string CallTest()
    {
        return "Bitch we working";
    }



    public GameObject spawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
    {
        Debug.Log("SpawnObject Called");
        PooledObjectInfo pool = objectPools.Find(p => p.LookUpString == objectToSpawn.name);

        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookUpString = objectToSpawn.name };
            objectPools.Add(pool);
            Debug.Log("Created a new Pool");
        }

        // Check if there are any inactive objects in the pool
        GameObject spawnableObj = pool.inactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            Debug.Log("No Pooled Object To Pull");
            // Find the Parent of the empty Object
            GameObject parentObject = SetParentObject(poolType);

            // If there is no active object, create a new one
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
            Debug.Log($"Created a new GameObject: {spawnableObj}");

            if (parentObject != null)
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }
        }
        else
        {
            Debug.Log("Found Pooled Object");
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.inactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name.Substring(0, obj.name.Length - 7); // by taking off the 7, we are removing the (clone) from the passed in Obj

        PooledObjectInfo pool = objectPools.Find(p => p.LookUpString == goName);

        if ( pool == null)
        {
            Debug.LogWarning("Trying To Releace An Object That is Not Pooled: " + obj.name);
        }
        else
        {
            obj.SetActive(false);
            pool.inactiveObjects.Add(obj);
        }
    }

    [Serializable]
    public class PooledObjectInfo
    {
        public string LookUpString;
        public List<GameObject> inactiveObjects = new List<GameObject>();
    }

}
