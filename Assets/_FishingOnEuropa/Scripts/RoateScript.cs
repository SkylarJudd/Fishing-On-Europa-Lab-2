using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoateScript : MonoBehaviour
{
    [SerializeField] private bool roating;
    [SerializeField] private float rotationSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (roating == true)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}
