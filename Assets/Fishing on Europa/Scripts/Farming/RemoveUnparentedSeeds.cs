using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveUnparentedSeeds : MonoBehaviour
{
    //Type of objects to remove
    public GameObject [] seedList;

    public int appleCount;
    public int bananaCount;
    public int carrotCount;
    public int orangeCount;
    public int potatoCount;
    public int watermelonCount;

    // This method will be called when the script is run
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (GameObject seed in seedList)
            {
                DestroyUnparentedObjectsOfType(seed);
            }
        }
    }

    // This method finds and destroys all objects of the specified type that are not parented
    void DestroyUnparentedObjectsOfType(GameObject type)
    {
        // Find all objects of the specified type
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in objects)
        {
            // Check if the object is of the specified type
            if (obj.GetType() == type.GetType())
            {
                // Check if the object has no parent
                if (obj.transform.parent == null)
                {
                    // Destroy the object
                    Destroy(obj);
                }
            }
        }
    }
}
