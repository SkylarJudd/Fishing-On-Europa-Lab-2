using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishToSpawn", menuName = "Europa/Fish", order = 1)]
public class FishToSpawn : ScriptableObject
{
    public List<HybridScriptableObjects> FishToSpawnList;
}
