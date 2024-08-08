using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsExsample : MonoBehaviour
{
    public void CallGameEventsExsmaple()
    {
        GameEvents.ExsampleGameEventString("Hello World");
    }
    public void CallGameEventsExsample2()
    {
        GameEvents.ExsampleGameEventGO(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnExsampleEventString += GameEvents_OnExsampleEventString;
        GameEvents.OnExsampleEventGameObject += GameEvents_OnExsampleEventGameObject;
    }
    private void OnDisable()
    {
        GameEvents.OnExsampleEventString -= GameEvents_OnExsampleEventString;
        GameEvents.OnExsampleEventGameObject -= GameEvents_OnExsampleEventGameObject;
    }

    private void GameEvents_OnExsampleEventGameObject(GameObject _gameObject)
    {
        print(_gameObject.ToString());
    }

    private void GameEvents_OnExsampleEventString(string _string)
    {
        print(_string);
    }

   
}
