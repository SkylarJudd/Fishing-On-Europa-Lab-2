using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public FoodList favFood;
    public int catchChance;
    public float damageToRod;
    public ToyList favToy;

}
    

