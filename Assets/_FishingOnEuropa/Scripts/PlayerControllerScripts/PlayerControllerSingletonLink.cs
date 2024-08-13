using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerSingletonLink : Singleton<PlayerControllerSingletonLink>
{
    [Header("Player Controller")]
    public Rigidbody player;
    public AutoHandPlayer playerController;

    public FOEItem_Food leftHandIFood;
    public FOEItem_Food rightHandFood;
    public FOEItem_Plushie leftHandPlushie;
    public FOEItem_Plushie rightHandPlushie;
    public bool hasItem;



    public void UpdatePlayerTransform(Transform playerTransform)
    {
        player.transform.position = playerTransform.position;
        player.transform.rotation = playerTransform.rotation;
    }

    
}
