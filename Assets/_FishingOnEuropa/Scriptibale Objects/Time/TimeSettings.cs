using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeSettings", menuName = "Europa/TimeSettings", order = 1)]

public class TimeSettings : ScriptableObject
{
    //0.5s = 1min results in a day length of 37.5 IRL minutes
    public float lengthOfMinute = 0.5f;//seconds

    public int startHour = 12;
    public int sunriseHour = 6;
    public int sunsetHour = 12;
}
