using Europa.GameEvents;
using System;
using UnityEngine;
/// <summary>
/// Example growth script thing
/// </summary>
public class PlantGrowth : GrowthObjectBase
{
    [SerializeField]
    private GrowthDataSO growthDataSO;
    [SerializeField]
    private ulong timeLastUnloaded;
    [SerializeField]
    private ulong currentTime;

    private void OnEnable()
    {
        ItemLoaded(timeLastUnloaded, growthDataSO);
    }
    private void OnDisable()
    {
        ItemUnloaded(); 
    }

    public void ItemLoaded(ulong itemLastUnloadedTime, GrowthDataSO growthData)
    {
        timeLastUnloaded = itemLastUnloadedTime;
        growthDataSO = growthData;

        GrowthObjectBase_EnableGrowable();
    }
    public void ItemUnloaded()
    {
        GrowthObjectBase_DisableGrowable();
    }
    protected override ulong GrowthObjectBase_GetTimeLastUnloaded()
    {
        return timeLastUnloaded;
    }

    protected override GrowthDataSO GrowthObjectBase_GetGrowthSettings()
    {
       return growthDataSO;
    }

    protected override void GrowthObjectBase_OnGrowth(bool wasObserved, int cycles)
    {
        Debug.Log($"Growth Occurred. Observed: {wasObserved}, Cycles: {cycles}");

        currentTime = TimeHandler.GetTotalMinutes();
    }

}

