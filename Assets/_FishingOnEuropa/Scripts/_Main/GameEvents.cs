using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


public static class GameEvents
{
    public static event Action<String> OnExsampleEventString = null;
    public static event Action<GameObject> OnExsampleEventGameObject = null;

    //MOVE THESE TO GAMEEVENTS LATER
    //public static event Action OnSunrise = delegate { };
    //public static event Action OnSunset = delegate { };
    //public static event Action OnHourChange = delegate { };
    //public static event Action OnMinuteChange = delegate { };
    //public static event Action OnNewDay = delegate { };   


    public static void ExsampleGameEventString(String _ExsampleText)
    {
        OnExsampleEventString?.Invoke(_ExsampleText);
    }

    public static void ExsampleGameEventGO(GameObject _ExsampleGO)
    {
        OnExsampleEventGameObject?.Invoke(_ExsampleGO);
    }

   
}
