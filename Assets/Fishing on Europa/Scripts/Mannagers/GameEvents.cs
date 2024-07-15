using System;
using UnityEngine;


public static class GameEvents
{
    public static event Action<GameObject> OnEnermyHit = null;
    public static event Action<GameObject> OnEnermyDie = null;

    public static event Action<GameObject> OnChickenEnemyHit = null;
    public static event Action<GameObject> OnChickenEnemyDie = null;

    public static void ReportOnEnemyHit(GameObject go)
    {
        OnEnermyHit?.Invoke(go);
    }

    public static void ReportOnEnemyDie(GameObject go)
    {
        OnEnermyDie?.Invoke(go);
    }

    public static void ReportOnChickenEnemyHit(GameObject go)
    {
        Debug.Log("ChickenHitEvent Fired");
        OnChickenEnemyHit?.Invoke(go);
        
    }

    public static void ReportOnChickenEnemyDie(GameObject go)
    {
        Debug.Log("ChickenDiedEvent Fired");
        OnChickenEnemyDie?.Invoke(go);
        
    }
}
