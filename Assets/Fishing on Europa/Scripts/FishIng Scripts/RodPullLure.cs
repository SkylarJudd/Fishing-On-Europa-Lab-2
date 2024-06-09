using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodPullLure : MonoBehaviour
{
    public Transform rodTopTransform;
    public Transform lureTransform;
    public float minDistanceToCallFunction = 1.0f; // Adjust this distance threshold
    public Rigidbody LureRB;
    public float pullMultiplayer = 1.0f; //Adjust this force multiplyer 
    public float catchDistance = 3.0f;
    public bool canCatch = false;
    public float maxDistance = 10;
    public RodCast casted;
    //public FishNavigationScript fishNavScript;


    private float previousDistance;

    public bool isPulling;

    private void Start()
    {
        maxDistance = 10;
        //isPulling = false;
        if (rodTopTransform != null && lureTransform != null)
        {
            previousDistance = Vector3.Distance(rodTopTransform.position, lureTransform.position);
        }
    }

    private void Update()
    {
        if (rodTopTransform != null && lureTransform != null)
        {
            if (isPulling == true && casted.casted == true)
            {
                //print("if enterd");
                float currentDistance = Vector3.Distance(rodTopTransform.position, lureTransform.position);

                if (currentDistance >= maxDistance)
                {
                    //print("Current Distance = " + currentDistance + " Max distance = " + maxDistance);
                    if (currentDistance > (previousDistance + minDistanceToCallFunction))
                    {
                        // Calculate the difference between the previous distance and current distance
                        float distanceDifference = currentDistance - previousDistance;


                        AddForce(distanceDifference * 5);
                    }
                }

                canCatch = currentDistance < catchDistance;

                previousDistance = currentDistance;
            }
        }
    }

    public void AddForce(float distanceDifference)
    {



        //Debug.Log("adding Force " + distanceDifference);
        lureTransform.transform.LookAt(rodTopTransform);
        LureRB.AddForce(lureTransform.forward * distanceDifference * pullMultiplayer, ForceMode.VelocityChange);



    }
    public void UpdateMaxDistance()
    {
        //print("UpdateMaxDistanceAnswerd"); 
        maxDistance = Vector3.Distance(rodTopTransform.position, lureTransform.position) + 1;
        //print("Max Distance " + maxDistance);
    }

   
}

