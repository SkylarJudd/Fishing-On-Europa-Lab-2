using System;
using UnityEngine;


public static class GameEvents
{
    public static event Action<String> OnExsampleEventString = null;
    public static event Action<GameObject> OnExsampleEventGameObject = null;

    
    public static void ExsampleGameEventString(String _ExsampleText)
    {
        OnExsampleEventString?.Invoke(_ExsampleText);
    }

    public static void ExsampleGameEventGO(GameObject _ExsampleGO)
    {
        OnExsampleEventGameObject?.Invoke(_ExsampleGO);
    }

   
}
