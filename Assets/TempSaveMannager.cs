using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSaveMannager : Singleton<TempSaveMannager>
{
    public List<int> HybridsInFarm = new List<int>();
    public List<bool> HybridsShiney = new List<bool>();
    public List<string> HybridsNames = new List<string>();
    public List<int> HybridTank = new List<int>();
    public List<Vector3> HybridLastPos = new List<Vector3>();
}
