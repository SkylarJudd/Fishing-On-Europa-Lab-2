using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    AtZoneOne,
    AtZoneTwo,
    AtZoneThree,
    AtCave,
    Paused,
    InInventory,
}

public class GameMannager : Singleton<GameMannager>
{
    public GameState gameState;

    public override void Awake()
    {
        base.Awake();
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        _TSM.StartGame();
        yield return new WaitForEndOfFrame();
        //_SETM.LoadSettings();
    }
}


