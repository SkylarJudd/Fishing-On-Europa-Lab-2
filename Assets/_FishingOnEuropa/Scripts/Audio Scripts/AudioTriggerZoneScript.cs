using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTriggerZoneScript : MonoBehaviour
{
    public int objectCount = 0;

    //Number of max objects allowed in the zone
    private const int maxObjects = 20;

    //initial and min scale
    private const float initialScale = 10f;
    private const float minScale = 1f;

    private void Start()
    {
        // Set the initial scale of the trigger
        transform.localScale = new Vector3(initialScale, initialScale, initialScale);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Increment the object count
        objectCount++;
        AdjustTriggerScale();
    }

    private void OnTriggerExit(Collider other)
    {
        // Decrement the object count
        if (objectCount > 0)
        {
            objectCount--;
            AdjustTriggerScale();
        }
    }

    private void AdjustTriggerScale()
    {
        // Calculate the new scale based on the object count
        if (objectCount > maxObjects)
        {
            float scaleReduction = (objectCount - maxObjects) * (initialScale - minScale) / (maxObjects);
            float newScale = Mathf.Max(minScale, initialScale - scaleReduction);
            transform.localScale = new Vector3(newScale, newScale, newScale);
        }
        else
        {
            // Reset to initial scale if the count is within limits
            transform.localScale = new Vector3(initialScale, initialScale, initialScale);
        }
    }

}
