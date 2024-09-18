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
        DailyEventHandler.InvokeDailyEvent(DailyEvents.Midnight);
    }
    public void CallGameEventsExsample2()
    {
        DailyEventHandler.InvokeDailyEvent(DailyEvents.Sunrise);
    }

    private void OnEnable()
    {
        DailyEventHandler.SubscribeDailyEvent(DailyEvents.Midnight, LogSunriseCount);
    }
    private void OnDisable()
    {
        DailyEventHandler.UnsubscribeDailyEvent(DailyEvents.Midnight, LogSunriseCount);
    }

    private void LogSunriseCount(bool wasObserved, int cycles)
    {
        Debug.Log($"Sunrise Debug! Was Observed: {wasObserved}, Cycles: {cycles}.");
    }
}
