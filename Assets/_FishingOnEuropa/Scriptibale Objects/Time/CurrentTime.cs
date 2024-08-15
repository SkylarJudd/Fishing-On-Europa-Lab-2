using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentTime", menuName = "Europa/Time", order = 1)]
public class CurrentTime : ScriptableObject
{
    [Header("Europian Time")]
    public int day = 0;
    public int hour;
    public int minute;
    [Header("Earth Time")]
    public DateTime earthTime;

}
