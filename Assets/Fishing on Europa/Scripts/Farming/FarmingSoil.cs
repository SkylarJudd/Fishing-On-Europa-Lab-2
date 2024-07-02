using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static CropBehaviour;

public class FarmingSoil : MonoBehaviour
{
    public GameObject parentObject;
    public Vector3[] positions;

    [Header("Crops")]
    //The crops currently planted on the land
    public List<GameObject> cropList;

    public Seeds seedToGrow;

    [Header("Time PlaceHolder")]
    public float hours;

    private void Start()
    {
        int childCount = transform.childCount;
    }

    private void Update()
    {
        if (hours > 0)
        {
            hours -= Time.deltaTime;

        }
        else
        {
            hours = 24;
            foreach(GameObject go in cropList)
            {
                //CropBehaviour.Grow();
            }
        }
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

                    //Access the CropBehaviour script
                    foreach (GameObject go in cropList)
                    {
                        child.gameObject.GetComponent<CropBehaviour>();
                    }

                    // Check if current index is within bounds of the positions array
                    if (i < positions.Length)
                    {
                        //turn off box collider and Rigidbody
                        BoxCollider myBC = child.gameObject.GetComponent<BoxCollider>();
                        myBC.enabled = false;

                        Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
                        rb.isKinematic = true;

                        // Move the child to the specified position
                        child.localPosition = positions[i];

                        cropList.Add(child.gameObject);
                    }
                }
            }
        }
    }

    /*private void OnCollisionEnter(Collision collision)
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

                    //Access the CropBehaviour script
                    foreach (GameObject go in cropList)
                    {
                        child.gameObject.GetComponent<CropBehaviour>();
                    }

                    // Check if current index is within bounds of the positions array
                    if (i < positions.Length)
                    {
                        //turn off box collider and Rigidbody
                        BoxCollider myBC = child.gameObject.GetComponent<BoxCollider>();
                        myBC.enabled = false;

                        Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
                        rb.isKinematic = true;

                        // Move the child to the specified position
                        child.localPosition = positions[i];

                        cropList.Add(child.gameObject);
                    }
                }
            }
        }
    }*/
}
