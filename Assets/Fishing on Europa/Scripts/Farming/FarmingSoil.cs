using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingSoil : MonoBehaviour
{
    public GameObject parentObject;
    public Vector3[] positions;

    private void Start()
    {
        int childCount = transform.childCount;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Seed"))
        {
            if (transform.childCount < positions.Length)
            {
                // Make the collided GameObject a child of parentObject
                collision.transform.parent = parentObject.transform;
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);

                    // Check if current index is within bounds of the positions array
                    if (i < positions.Length)
                    {
                        // Move the child to the specified position
                        child.localPosition = positions[i];
                    }
                }
            }
        }
    }


}
