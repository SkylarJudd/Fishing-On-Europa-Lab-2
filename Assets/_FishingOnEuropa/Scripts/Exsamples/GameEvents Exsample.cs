using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Europa.GameEvents;
using System;
using NaughtyAttributes;

public class GameEventsExsample : MonoBehaviour
{
    [Button("Call Game Events Exsmaple")]
    public void CallGameEventsExsmaple()
    {
        GameEvents.InvokeDailyEvent(DailyEvents.Sunrise, true, 1);
    }
    public void CallGameEventsExsample2()
    {
        GameEvents.InvokeDailyEvent(DailyEvents.MiddayEclipse, false, 3);
    }

    private void OnEnable()
    {
        GameEvents.SubscribeDailyEvent(DailyEvents.Sunrise, LogSunriseCount);
    }
    private void OnDisable()
    {
        GameEvents.UnsubscribeDailyEvent(DailyEvents.Sunrise, LogSunriseCount);
    }

    private void LogSunriseCount(bool wasObserved, int cycles)
    {
        Debug.Log($"Sunrise Debug! Was Observed: {wasObserved}, Cycles: {cycles}.");
    }
}
