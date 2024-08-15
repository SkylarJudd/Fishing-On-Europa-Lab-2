using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<<< HEAD:Assets/Fishing on Europa/Scriptibale Objects/Fish/Fish.cs
public enum SpawnLocation { Zone1, Zone2, Zone3 }; //Zone1 = Lake, Zone2 = River, Zone3 = Pond
public enum Rarity { Common, Uncommon, Rare, Epic };
[CreateAssetMenu(fileName = "Fish", menuName = "Europa/Fish", order = 1)]

public class Fish : ScriptableObject
{
    public int fishID;
    public string fishName;
    public string description;
    public float fishSpeed;
    public Rarity rarity;
    public SpawnLocation spawnLocation;
    public FoodType foodEaten;
========
public enum SpawnLocation { Zone1, Zone2, Zone3 }; 

[CreateAssetMenu(fileName = "HybridScriptableObjects", menuName = "Europa/Items/Hybrids", order = 1)]

public class HybridSO : ScriptableObject
{
    
    [Header("Spawning")]
    public PoolType hybridPoolType;
    public ItemRarity rarity;
    public SpawnLocation spawnLocation;
    public int spawnChance;
    public GameObject inWorldVisuals;

    [Header("Fishing MiniGame")]
    public float damageToRod;
    public int catchChance;
     
    [Header("Hybrid AI")]
    public float fishSpeed;
    public float rotationSpeed;
    public FoodType[] foodEaten;
>>>>>>>> main:Assets/_FishingOnEuropa/Scriptibale Objects/Fish/HybridSO.cs
    public FoodList favFood;
    public int catchChance;
    public float damageToRod;
    public ToyList favToy;

}
    

