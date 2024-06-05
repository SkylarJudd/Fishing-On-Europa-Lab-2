using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class WheelCounter : MonoBehaviour
{
    public float currentValue = 0.5f; // Initial value, set it to your starting value
    public float sensitivity = 0.1f; // Adjust this value to control the counting speed
    public XRKnob Reel;

    private float lastRotation;
    private float previousReelValue;
    public RodPullLure rodPullLureScript;

    void Update()
    {
        float rotationInput = Reel.value;

        // Calculate the change in rotation
        float rotationChange = rotationInput - previousReelValue;
        previousReelValue = rotationInput;

        // Handle the case where the reel wraps around from 1 to 0
        if (rotationChange > 0)
        {
            if (rotationInput < lastRotation)
            {
                rotationChange = 1.0f - lastRotation + rotationInput;
            }
        }

        // Update the last rotation for the next frame
        lastRotation = rotationInput;

        // Update the current value based on the rotation change
        float previousValue = currentValue;
        currentValue += rotationChange * sensitivity;

        

        // Check if the value increased or decreased
        if (currentValue > previousValue)
        {
            OnIncrease();
        }
        else if (currentValue < previousValue)
        {
            OnDecrease(previousValue);
        }

        //print(currentValue);
    }

    // Function to call when the value increases
    void OnIncrease()
    {
        //Debug.Log("Value increased: " + currentValue);
        // Add your code here for the increase event
    }

    // Function to call when the value decreases
    void OnDecrease(float perviousValue)
    {
       // Debug.Log("Value decreased: " + currentValue);
        // Add your code here for the decrease event
        float diffrence = currentValue - perviousValue;
        float posativeDif = Mathf.Abs(diffrence);
        //Debug.Log("Value diffrence: " + posativeDif);
        posativeDif *= 10;
        rodPullLureScript.AddForce(posativeDif);
        //print("CallMaxDistance");
        rodPullLureScript.UpdateMaxDistance();
    }
}

