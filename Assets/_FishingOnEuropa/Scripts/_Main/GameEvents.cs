using System;
using UnityEngine;


public static class GameEvents
{
    public static event Action<String> OnExsampleEventString = null;
    public static event Action<GameObject> OnExsampleEventGameObject = null;

    public static event Action<int,int,int,int> OnTempMorningEvent = null;

    public static void ExsampleGameEventString(String _ExsampleText)
    {
        OnExsampleEventString?.Invoke(_ExsampleText);
    }

    public static void ExsampleGameEventGO(GameObject _ExsampleGO)
    {
        OnExsampleEventGameObject?.Invoke(_ExsampleGO);
    }

    public static void TempMorningEvent(int _Day,  int _Hour, int _Minute, int _Second)
    {
        OnTempMorningEvent?.Invoke(_Day,_Hour,_Minute,_Second);
    }

}
