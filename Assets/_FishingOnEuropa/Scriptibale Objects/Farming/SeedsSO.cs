using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Seeds", menuName = "Europa/Farming/Seeds", order = 1)]

public class SeedsSO : ScriptableObject
{
    public int ItemID;
    public float growthTime;

    public FOEItem foodProduced;

    [Header("Plant Stages")]
    public GameObject seed;
    public GameObject sprout;
    public GameObject adolecent;
    public GameObject mature;
    public GameObject harvested;
    public FoodList foodItem;


}
