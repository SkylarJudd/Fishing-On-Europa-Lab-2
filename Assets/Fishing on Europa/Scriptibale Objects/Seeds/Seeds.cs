using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Seeds", menuName = "Europa/Seeds", order = 1)]

public class Seeds : ScriptableObject
{
    public string seedName;
    public string seedDesc;
    public string foodProduced;

    public int spawnLocation; // 1 = Lake, 2 = River, 3 = Pond
    public float growthTime;
    public int price;


    public Sprite artwork;

    [Header("Plant Stages")]
    public GameObject seedling;
    public GameObject sprout;
    public GameObject adolecent;
    public GameObject mature;
}
