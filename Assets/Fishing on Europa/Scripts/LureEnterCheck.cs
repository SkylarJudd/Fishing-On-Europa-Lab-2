using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureEnterCheck : MonoBehaviour
{
    private bool isLureInside = false;
    public string lureTag = "Lure";
    public RodPullLure setDistance;

   

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(lureTag))
        {
            isLureInside = true;
            //print("Lure in safe zone = " + isLureInside);
            //setDistance.UpdateMaxDistance();
            //print("SetDistance Called");
            // You can add any additional logic here when a "lure" enters the collider.
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(lureTag))
        {
            isLureInside = false;
            //print("Lure in safe zone = " + isLureInside);
            // You can add any additional logic here when a "lure" exits the collider.
        }
    }

    public bool isLureInsideCheck()
    {
        return isLureInside;
    }

    // You can access the 'isLureInside' variable from other scripts to check if a lure is inside.
}
