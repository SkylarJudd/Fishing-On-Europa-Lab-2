using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static CropBehaviour;
using static Unity.VisualScripting.Metadata;

public class FarmingSoil : MonoBehaviour
{
    public GameObject parentObject;
    public Vector3[] positions;
    public int maxCrops;
    public float minDistance = 2.0f; // Minimum distance between child objects

    [Header("Crops")]
    //The crops currently planted on the land

    public List<CropBehaviour> cropsPlanted;

    [Header("Time PlaceHolder")]
    public float hours;

    public GameObject seed;

    private void Update()
    {
        RemoveCropBehaviour();

        if (hours > 0)
        {
            hours -= Time.deltaTime;

        }
        else
        {
            hours = 24;

           foreach (CropBehaviour crop in cropsPlanted )
            {
                crop.Grow();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(seed, new Vector3 (0,5,0), Quaternion.identity);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Seed"))
        {
            if (transform.childCount < maxCrops /*positions.Length*/)
            {
                cropsPlanted.Add(collision.gameObject.GetComponent<CropBehaviour>());

                foreach (CropBehaviour crop in cropsPlanted)
                {
                    crop.Plant();
                }

                // Make the collided GameObject a child of parentObject
                collision.transform.parent = parentObject.transform;

                // Get all child objects
                Transform[] children = GetComponentsInChildren<Transform>();

                // Iterate through all child objects
                for (int i = 2; i < children.Length; i++) // Start at 1 to skip the parent object itself
                {
                    Transform childA = children[i];

                    // Check distance with other children
                    for (int j = i + 1; j < children.Length; j++)
                    {
                        Transform childB = children[j];

                        // Calculate distance between childA and childB
                        float distance = Vector3.Distance(childA.position, childB.position);

                        // If the distance is less than the minimum required, move childB away
                        if (distance < minDistance)
                        {
                            // Calculate direction to move childB away from childA
                            Vector3 direction = (childB.position - childA.position).normalized;

                            // Calculate the amount to move childB
                            float moveAmount = minDistance - distance;

                            // Move childB away from childA
                            childB.position += direction * moveAmount;
                        }
                    }

                    foreach (Transform child in transform)
                    {
                        // Set the rotation to (0, 0, 0)
                        child.rotation = Quaternion.identity;

                        // Freeze the Rigidbody position and rotation if a Rigidbody is attached
                        Rigidbody rb = child.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.constraints = RigidbodyConstraints.FreezeAll;
                        }
                    }

                    /*for (int i = 0; i < transform.childCount; i++)
                    {
                        Transform child = transform.GetChild(i);

                        // Check if current index is within bounds of the positions array
                        if (i < positions.Length)
                        {
                            // Move the child to the specified position
                            child.localPosition = positions[i];
                        }
                    }*/
                }
            }

            if (transform.childCount == 0)
            {
                return; ;
            }
        }
    }

    private void RemoveCropBehaviour()
    {
        for (int i = cropsPlanted.Count - 1; i > -1; i--)
        {
            if (cropsPlanted[i] == null)
            {
                cropsPlanted.RemoveAt(i);
            }
        }
    }
}
