using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAway : MonoBehaviour
{
    public string targetTag = "Seed"; // Tag of objects to push away
    public float pushDistance = 1.0f; // The distance within which the objects will be pushed
    public float pushForce = 10.0f; // The force applied to push the object away

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Find all objects with the specified tag
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject target in targets)
        {
            // Calculate the distance between this object and the target
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance < pushDistance)
            {
                // Get the Rigidbody component of the target object
                Rigidbody targetRb = target.GetComponent<Rigidbody>();

                if (targetRb != null)
                {
                    // Calculate direction to push away from this object
                    Vector3 pushDirection = target.transform.position - transform.position;
                    pushDirection.Normalize(); // Normalize to get the direction only

                    // Apply a force to push the target object away
                    targetRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                }
            }
        }
    }
}
