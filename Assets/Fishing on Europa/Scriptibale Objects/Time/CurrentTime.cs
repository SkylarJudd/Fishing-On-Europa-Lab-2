using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentTime", menuName = "Europa/Time", order = 1)]
public class CurrentTime : ScriptableObject
{
    public int day;
    public int hour;
    public int minute;
    public string currentTimeString;

}
