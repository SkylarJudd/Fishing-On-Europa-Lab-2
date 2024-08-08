using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class TimeService
{
    //date time struct
    readonly TimeSettings settings;
    DateTime currentTime;
    readonly TimeSpan sunriseTime; //duration
    readonly TimeSpan sunsetime;

    //public property to get current time whenever we want
    public DateTime CurrentTime => currentTime;

    //observable publishes an evevnt when the value of what it is observing changes
    readonly Observable<bool> isDayTime;
    readonly Observable<int> currentHour;

    //MOVE THESE TO GAMEEVENTS LATER
    public event Action OnSunrise = delegate { };
    public event Action OnSunset = delegate { };
    public event Action OnHourChange = delegate { };

    //constructor
    public TimeService(TimeSettings settings) 
    {
        this.settings = settings; //initalise

        //set to todays date and starting hour
        currentTime = DateTime.Now + TimeSpan.FromHours(settings.startHour);

        //set sunrise and sunset time
        sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
        sunsetime = TimeSpan.FromHours(settings.sunsetHour);

        isDayTime = new Observable<bool>(IsDayTime());
        currentHour = new Observable<int>(currentTime.Hour);

        //when event is fired, make decision based on value to fire onsunrise or onsunset
        isDayTime.ValueChanged += day => (day ? OnSunrise : OnSunset)?.Invoke();
        //on hour change
        currentHour.ValueChanged += _ => OnHourChange?.Invoke();
    }

    //update time as playing
    public void UpdateTime(float deltaTime)
    {
        currentTime = currentTime.AddSeconds(deltaTime * settings.timeMultipler);
        
        isDayTime.Value = IsDayTime();
        currentHour.Value = currentTime.Hour;
    }

    public float CalculateSunAngle()
    {
        bool isDay = IsDayTime();
        //0 straight down, 100 straight up
        float startDegree = isDay ? 0 : 180;

        //if its daytime start from sunrise and vis versa
        TimeSpan start = isDay ? sunriseTime : sunsetime;
        TimeSpan end = isDay ? sunsetime : sunriseTime;

        //get difference between start and end
        TimeSpan totalTime = CalculateDifference(start, end);
        //get difference from start to now
        TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);

        //percantage of how the sun has travelled in this arch
        double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
        return Mathf.Lerp(startDegree, startDegree + 180, (float)percentage);

    }

    //check if its daytime or nighttime, if true daytime
    bool IsDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetime;

    //calculate difference between two timespans
    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan difference = to - from;
        //may need to change 24 as europa is 75 hour day
        //if value is negative, add 24 to account for time difference being the next day
        //CHANGE 24 FOR A FULL DAY LATER
        return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
    }
 
}
