using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class SkyboxCamera : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] float skyboxScale;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localPosition = playerCam.position / skyboxScale;
        transform.rotation = playerCam.rotation;
    }

}
