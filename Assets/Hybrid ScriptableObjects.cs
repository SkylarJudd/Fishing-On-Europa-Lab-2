using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HybridScriptableObjects", menuName = "Europa/Fish", order = 2)]

public class HybridScriptableObjects2 : ScriptableObject
{

    public int FishID; // for the save system
    public GameObject HybridPrefab;
    public PoolType HybridPoolType;

    public string HybridName;
    public string HybridDescription;

    public float SpawnChance;
    public float DamageToRod;


}
