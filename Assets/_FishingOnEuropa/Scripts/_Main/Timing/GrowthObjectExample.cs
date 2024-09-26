using Europa.GameEvents;
using System;
using UnityEngine;
/// <summary>
/// Example growth script thing
/// </summary>
public class GrowthObjectExample : GrowthObjectBase
{
    private GrowthDataSO growthDataSO;
    private ulong timeLastUnloaded;
   
    public void ItemLoaded(ulong itemLastUnloadedTime, GrowthDataSO growthData)
    {
        timeLastUnloaded = itemLastUnloadedTime;
        growthDataSO = growthData;

        EnableGrowable();
    }
    public void ItemUnloaded()
    {
        DisableGrowable();
    }
    protected override ulong GetTimeLastUnloaded()
    {
        return timeLastUnloaded;
    }

    protected override GrowthDataSO GetGrowthSettings()
    {
       return growthDataSO;
    }

    protected override void OnGrowth(bool wasObserved, int cycles)
    {
        Debug.Log($"Growth Occurred. Observed: {wasObserved}, Cycles: {cycles}");
    }

}

