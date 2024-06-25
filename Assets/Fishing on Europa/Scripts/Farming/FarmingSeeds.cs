using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingSeeds : MonoBehaviour
{
    public Seeds seed;


    private void Start()
    {
        Debug.Log(seed.growthTime);
    }
}
