using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Seeds", menuName = "Europa/Seeds", order = 1)]

public class Seeds : ScriptableObject
{
    public string seedName;
    public string seedDesc;
    public int spawnLocation; // 1 = Lake, 2 = River, 3 = Pond
    public float growTime;
    public string foodProduced;
}
