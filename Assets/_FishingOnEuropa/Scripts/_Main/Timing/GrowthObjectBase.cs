using System;
using UnityEngine;
using Europa.GameEvents;
/// <summary>
/// The abstract base class for all of the growable objects in the game.
/// </summary>
[Serializable]
public abstract class GrowthObjectBase : MonoBehaviour
{
    /// <summary>
    /// Called when this object grows. Growth is handled by the <see cref="UnobservedTimePassageCalculator"/>.
    /// From <see cref="GrowthObjectBase.OnUnobservedGrowth(int)"/>.
    /// </summary>
    /// <param name="wasObserved">Is true if the object is loaded when a daily event fires,
    /// false if the object has just been loaded in after daily events occurred during the object's inactivity.</param>
    /// <param name="cycles">For when <paramref name="wasObserved"/> is false, this is the amount of times the growth occurred in the background.
    /// Eg: if an apple tree is unloaded for 3 mornings, we'll need to make it grow for 3 stages when it is loaded in.</param>
    protected abstract void GrowthObjectBase_OnGrowth(bool wasObserved = true, int cycles = 1);

    /// <summary>
    /// Registers this growable with the growth handler.
    /// Needs to be called when the growable is loaded in.
    /// </summary>
    public void GrowthObjectBase_EnableGrowable()
    {
        var growthSO = GrowthObjectBase_GetGrowthSettings();
        ulong timeLastUnloaded = GrowthObjectBase_GetTimeLastUnloaded();
        HandleMissingTimeAndSubscribeToTimeEvents(growthSO, timeLastUnloaded);
    }

    /// <summary>
    /// Disables the growable object.
    /// </summary>
    protected void GrowthObjectBase_DisableGrowable()
    {
        var growthSO = GrowthObjectBase_GetGrowthSettings();
        switch (growthSO.EventType)
        {
            case GrowthDataSO.EventTypeEnum.DailyEvent:
            case GrowthDataSO.EventTypeEnum.DailyEventCustomHour:
                DailyEventHandler.UnsubscribeDailyEvent(GrowthObjectBase_OnGrowth);
                break;
            case GrowthDataSO.EventTypeEnum.HourlyTimer:
            case GrowthDataSO.EventTypeEnum.MinuteTimer:
                IGrowableTimedEventDictionary.UnsubscribeGrowthTimerEvents(this);
                break;
        }
    }

    /// <summary>
    /// Gets when this item was last unloaded.
    /// </summary>
    /// <returns>Returns when this item was last unloaded.</returns>
    protected abstract ulong GrowthObjectBase_GetTimeLastUnloaded();

    /// <summary>
    /// Gets the growth settings for this growable.
    /// </summary>
    /// <returns>The growth settings.</returns>
    protected abstract GrowthDataSO GrowthObjectBase_GetGrowthSettings();

    /// <summary>
    /// Subscribes to daily events that trigger growth as well as handles the unobserved daily events.
    /// </summary>
    /// <param name="timeLastUnloaded">When was this object last unloaded?</param>
    /// <param name="eventHours">The hours at which the event is triggered.</param>
    private void SubscribeDailyEvent(ulong timeLastUnloaded, params int[] eventHours)
    {
        int cycles = UnobservedTimePassageCalculator.GetDailyEventsSinceZoneUnloaded(timeLastUnloaded, eventHours);
        GrowthObjectBase_OnGrowth(false, cycles);
        DailyEventHandler.SubscribeDailyEvent(eventHours, GrowthObjectBase_OnGrowth);
    }

    /// <summary>
    /// Subscribes to minute events that trigger growth as well as handles the unobserved minute events.
    /// </summary>
    /// <param name="timeLastUnloaded">When was this object last unloaded?</param>
    /// <param name="minutesPerCycle">How many minutes are there per cycle?</param>
    private void SubscribeToMinuteEvents(ulong timeLastUnloaded, int minutesPerCycle)
    {
        int minutesMissed = UnobservedTimePassageCalculator.GetMinutesSinceZoneUnloaded(timeLastUnloaded);
        int unloadedCycles = minutesMissed / minutesPerCycle;
        int remainder = minutesMissed % minutesPerCycle;

        GrowthObjectBase_OnGrowth(false, unloadedCycles);

        int minuteCounter = remainder;

        void OnMinuteEvent(int minutes)
        {
            minuteCounter++;
            minuteCounter %= minutesPerCycle;
            if (minuteCounter == 0)
            {
                GrowthObjectBase_OnGrowth(true, 1);
            }
        }

        IGrowableTimedEventDictionary.SubscribeGrowthTimerEvent(this, OnMinuteEvent);
    }

    /// <summary>
    /// Handles all of the time subscription and missing time calculations.
    /// </summary>
    /// <param name="growthSO">The growth settings.</param>
    /// <param name="timeLastUnloaded">When was this object last unloaded?</param>
    private void HandleMissingTimeAndSubscribeToTimeEvents(GrowthDataSO growthSO, ulong timeLastUnloaded)
    {
        switch (growthSO.EventType)
        {
            case GrowthDataSO.EventTypeEnum.DailyEvent:
                SubscribeDailyEvent(timeLastUnloaded, growthSO.DailyEventEnum.GetHoursForDailyEvents());
                break;
            case GrowthDataSO.EventTypeEnum.DailyEventCustomHour:
                SubscribeDailyEvent(timeLastUnloaded, growthSO.DailyEventHour);
                break;
            case GrowthDataSO.EventTypeEnum.HourlyTimer:
                SubscribeToMinuteEvents(timeLastUnloaded, growthSO.HourlyTime * TimeConstants.MINUTES_IN_HOUR);
                break;
            case GrowthDataSO.EventTypeEnum.MinuteTimer:
                SubscribeToMinuteEvents(timeLastUnloaded, growthSO.MinuteTime);
                break;
        }
    }
}

