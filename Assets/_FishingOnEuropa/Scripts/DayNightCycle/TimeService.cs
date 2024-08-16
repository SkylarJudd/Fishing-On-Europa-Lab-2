using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;


public class TimeService
{
    CurrentTime currentTimeSO;

    //observable publishes an evevnt when the value of what it is observing changes
    readonly Observable<bool> isDayTime;
    readonly Observable<int> currentHour;
    readonly Observable<int> currentMinute;
    readonly Observable<int> currentDay;

    //MOVE THESE TO GAMEEVENTS LATER
    public event Action OnSunrise = delegate { };
    public event Action OnSunset = delegate { };
    public event Action OnHourChange = delegate { };
    public event Action OnMinuteChange = delegate { };
    public event Action OnNewDay = delegate { };    //ADD THING HERERERERERE 

    ////date time struct
    readonly TimeSettings settings;

    //DateTime currentTime;
    readonly int sunriseTime; //duration
    readonly int sunsetime;

   // bool IsDayTime() => currentTimeSO.hour > sunriseTime && currentTimeSO.hour < sunsetime;

    public TimeService(TimeSettings settings, CurrentTime currentTime)
    {
        this.settings = settings; //initalise
        this.currentTimeSO = currentTime; //initalise

        //set sunrise and sunset time
        sunriseTime = settings.sunriseHour;
        sunsetime = settings.sunsetHour;


        currentHour = new Observable<int>(currentTimeSO.hour);
        currentMinute = new Observable<int>(currentTimeSO.minute);
        currentDay = new Observable<int>(currentTimeSO.day);

        //when event is fired, make decision based on value to fire onsunrise or onsunset
        //isDayTime.ValueChanged += day => (day ? OnSunrise : OnSunset)?.Invoke();

        //on hour change
        currentHour.ValueChanged += _ => OnHourChange?.Invoke();
        currentMinute.ValueChanged += _ => OnHourChange?.Invoke();
        currentDay.ValueChanged += _ => OnNewDay?.Invoke();

    }

    ////check if its daytime or nighttime, if true daytime

    //calculate difference between two timespans
    int CalculateDifference(int from, int to)
    {
        int difference = to - from;

        return difference;
    }

}
