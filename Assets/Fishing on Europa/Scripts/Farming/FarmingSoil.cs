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

    //The crop currently planted in the land. 
    public CropBehaviour cropPlanted;

    [Header("Time PlaceHolder")]
    public float hours;

    private void Update()
    {
        if (hours > 0)
        {
            hours -= Time.deltaTime;

        }
        else
        {
            hours = 24;
            foreach(GameObject crop in cropList)
            {
                print("Name");
                cropPlanted.Grow();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Seed"))
        {
            if (transform.childCount < positions.Length)
            {
                cropList.Add(collision.gameObject);
                collision.gameObject.GetComponent<CropBehaviour>();
                cropPlanted = collision.gameObject.GetComponent<CropBehaviour>();
                cropPlanted.Plant();

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
