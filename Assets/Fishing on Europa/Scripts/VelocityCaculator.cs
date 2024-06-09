using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityCaculator : MonoBehaviour
{
    private Vector3 ballSpeed;
    private Vector3 lastPos;
    public float ballCurrentSpeed;
 
    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        if (lastPos != transform.position)
        {
            ballSpeed = transform.position - lastPos;
            ballSpeed /= Time.deltaTime;
            lastPos = transform.position;
        }
        ballCurrentSpeed = ballSpeed.magnitude;
        //print(ballSpeed.magnitude);
    }

    
}
