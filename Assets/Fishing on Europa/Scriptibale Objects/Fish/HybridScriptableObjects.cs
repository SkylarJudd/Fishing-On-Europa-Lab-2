using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnLocation { Zone1, Zone2, Zone3 }; 
public enum Rarity { Common, Uncommon, Rare, Epic };
[CreateAssetMenu(fileName = "HybridScriptableObjects", menuName = "Europa/Fish", order = 1)]

public class HybridScriptableObjects : ScriptableObject
{
    //Hybrid Info
    public int fishID;
    public string fishName;
    public string description;
    public GameObject hybridPrefab;

    //Spawning
    public PoolType hybridPoolType;
    public Rarity rarity;
    public SpawnLocation spawnLocation;
    public PoolType poolType;
    public int spawnChance;

    //Fishing MiniGame
    public float damageToRod;
    public int catchChance;

    //Hybrid AI

    public float fishSpeed;
    public float rotationSpeed;
    public FoodType foodEaten;
    public FoodList favFood;
    public ToyList favToy;

}
    

