using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerSingletonLink : Singleton<PlayerControllerSingletonLink>
{
    [Header("Player Controller")]
    public Rigidbody player;


    public void UpdatePlayerTransform(Transform playerTransform)
    {
        player.transform.position = playerTransform.position;
        player.transform.rotation = playerTransform.rotation;
    }
}
