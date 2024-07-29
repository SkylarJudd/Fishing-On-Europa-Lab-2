using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;

    //the values defined in this constructor will be the defult values the game starts with when there is no data to load

    public GameData()
    {
        playerPosition = Vector3.zero;
    }
}
