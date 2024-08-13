using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerSingletonLink : Singleton<PlayerControllerSingletonLink>
{
    [Header("Player Controller")]
    public Rigidbody player;
    public AutoHandPlayer playerController;
    public Transform playerHead;
    public Transform leftHand;
    public Transform rightHand;

    public FOEItem_Food leftHandFood;
    public FOEItem_Food rightHandFood;
    public FOEItem_Plushie leftHandPlushie;
    public FOEItem_Plushie rightHandPlushie;
    public bool hasItem;



    public void UpdatePlayerTransform(Transform playerTransform)
    {
        player.transform.position = playerTransform.position;
        player.transform.rotation = playerTransform.rotation;
    }

    public void OnFoodPickUp(bool _Hand, FOEItem_Food _Food)
    {
        print(_Hand);
        if (_Hand) leftHandFood = _Food;
        else if (!_Hand) rightHandFood = _Food;

        hasItem = true;
    }

    public void OnFoodDrop(bool _Hand)
    {
        print( _Hand );
        if (_Hand) leftHandFood = null;
        else if (!_Hand) rightHandFood = null;

        if (leftHandFood == null && rightHandFood == null) hasItem = false;

    }

    public void OnPlushiePickUp(bool _Hand, FOEItem_Plushie _Plushie)
    {
        if (_Hand) leftHandPlushie = _Plushie;
        else if (!_Hand) rightHandPlushie = _Plushie;

        hasItem = true;
    }

    public void OnPlushieDrop(bool _Hand)
    {
        if (_Hand) leftHandFood = null;
        else if (!_Hand) rightHandFood = null;

        if (leftHandFood == null && rightHandFood == null) hasItem = false;

    }


}
