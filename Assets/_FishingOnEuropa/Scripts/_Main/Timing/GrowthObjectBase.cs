using System;
using UnityEngine;
using Europa.GameEvents;
/// <summary>
/// The abstract base class for all of the growable objects in the game.
/// </summary>
[Serializable]
public abstract class GrowthObjectBase : MonoBehaviour, IGrowable
{
    [SerializeField]
    private GrowthDataSO growthDataSO;

    string IGrowable.GetGrowthZone()
    {
        // for the time being lets just use the scene name ^_^
        return gameObject.scene.name;
    }

    void IGrowable.OnGrowth(bool wasObserved, int cycles)
    {
        throw new NotImplementedException();
    }
}

