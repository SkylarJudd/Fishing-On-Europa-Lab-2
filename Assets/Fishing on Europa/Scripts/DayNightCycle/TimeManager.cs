using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager
{
    //date time struct
    readonly TimeSettings settings;
    DateTime currentTime;
    readonly TimeSpan sunriseTime; //duration
    readonly TimeSpan sunsetime;

    //observable publishes an evevnt when the value of what it is observing changes
    readonly IObservable<bool> isDayTime;
    readonly IObservable<int> currentHour;

    public TimeManager(TimeSettings settings) 
    {
        this.settings = settings; //initalise

        //set to todays date and starting hour
        currentTime = DateTime.Now + TimeSpan.FromHours(settings.startHour);

        //set sunrise and sunset time
        sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
        sunsetime = TimeSpan.FromHours(settings.sunsetHour);
    }

    //update time as playing
    public void UpdateTime(float deltaTime)
    {
        currentTime = currentTime.AddSeconds(deltaTime * settings.timeMultipler);
    }

    //check if its daytime or nighttime, if true daytime
    bool isDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetime;

    //calculate difference between two timespans
    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan difference = to - from;
        //may need to change 24 as europa is 75 hour day
        //if value is negative, add 24 to account for time difference being the next day
        return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
    }
 
}
