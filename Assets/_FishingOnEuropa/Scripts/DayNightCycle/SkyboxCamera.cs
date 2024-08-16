using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class SkyboxCamera : GameBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] float skyboxScale;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.localPosition = playerCam.position / skyboxScale;
        transform.rotation = _PLAYER.playerHead.transform.rotation;
    }

}
