using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomGravityXR : MonoBehaviour
{
    public CharacterController characterController;

    void Start()
    {
        characterController.GetComponent<Rigidbody>().useGravity = true; // Enable gravity
        characterController.GetComponent<Rigidbody>().drag = 1f; // Optional: Add drag for more natural movement
    }
}
    
       
       
           
            
           

            
          