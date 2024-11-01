using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonPushToFarm : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>().selectEntered.AddListener(x => SendPlayerToWorld());
    }

    public void SendPlayerToWorld()
    {
        print("Button Pressed I would now send the player to the world, but ive not done that yet :) ");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
