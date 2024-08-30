using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RodCast : MonoBehaviour
{
    public GameObject lure;
    public GameObject fishingLine;
    public Transform spawnPoint;
    public Rigidbody LureRB;
    

    public VelocityCaculator CurrentVelocity;

     
    public float yThreshold = -7.036f; // Adjust this threshold as needed
    public float delay = 5.0f; // Delay in seconds

    public LerpAndHide resetScript;
    public LureEnterCheck[] lureCheck;
    public ClosestObjectsFinder cloestObjectScript;

    private Coroutine delayedFunctionCall;

    private bool castprime = false;
    public bool casted = false;
    public bool fishCaught = false;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable fishingRod = GetComponent<XRGrabInteractable>();
        fishingRod.activated.AddListener(CastLine);
        fishingRod.deactivated.AddListener(Cast);
        lure.SetActive(false);
        fishingLine.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(casted == true)
        {
            CheckYValueThreshold();
        }
        
    }
    public void Cast(DeactivateEventArgs arg)
    {
        //print("Trigger up");
        if (castprime == true && casted == false)
        {
            //unhides game objects
            lure.SetActive(true);
            fishingLine.SetActive(true);
            //sets the lures location to the end of the fishing rod
            lure.transform.position = spawnPoint.position;
            //adds a force to the lure, 
            //print(CurrentVelocity.ballCurrentSpeed);
            LureRB.AddForce(spawnPoint.forward * CurrentVelocity.ballCurrentSpeed, ForceMode.VelocityChange);

            // Check if there's a previous delayed function call, and cancel it
            if (delayedFunctionCall != null)
            {
                StopCoroutine(delayedFunctionCall);
            }

            // Start a new delayed function call
            delayedFunctionCall = StartCoroutine(DelayedFunctionCall());
        }
        castprime = false;
        casted = true;
        //print("casted = " + casted);


    }


    public void CastLine(ActivateEventArgs arg)
    {
       //print("Trigger Down");
        castprime = true;

        //if (casted == true && cloestObjectScript.fishCaught == true) 
        //{
        //    resetScript.StartLerp();
        //    //print("LerpCalled");
        //}
    }

    

    IEnumerator DelayedFunctionCall()
    {
        yield return new WaitForSeconds(delay);

        // Call a function when the timer runs out
        TimerFunction();
    }

    void TimerFunction()
    {
        
        //Debug.Log("Function called after delay");

        bool allLuresOutside = true;

        foreach (var lure in lureCheck)
        {
            if (lure.isLureInsideCheck())
            {
                allLuresOutside = false;
                break;  // No need to continue checking once one lure is inside
            }
        }

        if (allLuresOutside)
        {
            resetScript.StartLerp();
            //print("LerpCalled");
        }
        else
        {
            //cloestObjectScript.FindClosestObjects(); //REMOVED SINCE ClosestObjectsFinder IS BEING REMOVED

            //print("Calling Find Closest objects after delay");
        }

    }


    void CheckYValueThreshold()
    {
        if (lure != null && lure.transform.position.y < yThreshold)
        {
            
            //print("YthresholdMet");
            ThresholdFunction();
        }
    }

    void ThresholdFunction()
    {
        
        //Debug.Log("Function called when y-value meets the threshold");

        if (lureCheck[0].isLureInsideCheck() == false)
        {

            resetScript.StartLerp();
            //print("LerpCalled");

        }
    }
}



