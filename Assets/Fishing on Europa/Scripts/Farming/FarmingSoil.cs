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
            if (transform.childCount < positions.Length)
            {
                cropsPlanted.Add(collision.gameObject.GetComponent<CropBehaviour>());

                foreach (CropBehaviour crop in cropsPlanted)
                {
                    crop.Plant();
                }

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
