using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeSettings", menuName = "Europa/TimeSettings", order = 1)]

public class TimeSettings : ScriptableObject
{
    public float timeMultipler = 2000;
    public float startHour = 12;
    public float sunriseHour = 6;
    public float sunsetHour = 18;
}
