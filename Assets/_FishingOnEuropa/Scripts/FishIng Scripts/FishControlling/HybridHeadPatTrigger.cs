using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridHeadPatTrigger : GameBehaviour
{
    GameObject objectInTrigger;

    /// <summary>
    /// Returns hand in trigger, if no hand in trigger it returns null
    /// </summary>
    public Transform ReturnHand()
    {
        if(objectInTrigger == null) return null;

        if (objectInTrigger.tag == "LeftHandTag")
        {
            return _PLAYER.leftHand;

        }
        else if (objectInTrigger.tag == "RightHandTag")
        {
            return _PLAYER.rightHand;
        }

        return null;


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHandTag") || other.CompareTag("RightHandTag"))
        {
            objectInTrigger = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LeftHandTag") || other.CompareTag("RightHandTag"))
        {
            objectInTrigger = null;
        }
    }
}
