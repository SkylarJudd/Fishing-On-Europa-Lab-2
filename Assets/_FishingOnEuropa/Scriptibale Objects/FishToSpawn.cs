using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishToSpawn", menuName = "Europa/Fishing/Fish", order = 1)]
public class FishToSpawnSO : ScriptableObject
{
    public List<FOEItem_Hybrid> FishToSpawnList;
}
