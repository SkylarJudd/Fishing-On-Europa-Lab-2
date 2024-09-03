
using UnityEngine;
using System.Collections.Generic;

public class GameBehaviour : MonoBehaviour
{
    protected static ExsampleSingleton _ES { get { return ExsampleSingleton.instance; } }

    protected static GameMannager _GM { get { return GameMannager.instance; } }
    protected static ObjectPoolManager _OPM { get { return ObjectPoolManager.instance; } }
    protected static TempSaveMannager _TSM { get { return TempSaveMannager.instance; } }
    protected static SceneController _SC { get { return SceneController.instance; } }
    protected static PlayerControllerSingletonLink _PLAYER { get { return PlayerControllerSingletonLink.instance; } }
    protected static SettingsMannager _SETM { get { return SettingsMannager.instance; } }

    protected static FishNavigationManager _FNAVM { get { return FishNavigationManager.instance; } }

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

    


}
