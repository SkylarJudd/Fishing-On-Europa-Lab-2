using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;


public class LockPlayerScript : MonoBehaviour
{
    private DynamicMoveProvider dynamicMoveProvider;


    void Start()
    {
        dynamicMoveProvider = GetComponent<DynamicMoveProvider>();

        // Check if the script is found
        if (dynamicMoveProvider == null)
        {
            Debug.LogError("DynamicMoveProvider script not found on the same game object.");
        }
    }

    public void StopPlayerMovement()
    {
        dynamicMoveProvider.enabled = false;
    }

    public void StartPlayerMovement()
    {
        dynamicMoveProvider.enabled = true;
    }
}
