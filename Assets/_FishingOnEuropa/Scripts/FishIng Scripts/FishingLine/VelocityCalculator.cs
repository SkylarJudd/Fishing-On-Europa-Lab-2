using Obvious.Soap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityCalculator : MonoBehaviour
{
    [SerializeField] private Vector3Reference rodEndPostion; // The Current Position of the rods end point
    [SerializeField] private FloatReference rodEndCurrentSpeed; // Speed magnitude
    [SerializeField] private Vector3Reference rodEndPointForceDirection;  // Direction of movement

    [SerializeField] private BoolReference playerCastInput;

    private Vector3 endPointSpeed; // Velocity (position change over time)
    private Vector3 lastPos;   // Last position of the object

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        if (playerCastInput.Value == true)
        {
            CaculateFishingRodEndPoint();
        }
        rodEndPostion.Value = transform.position;
    }

    private void CaculateFishingRodEndPoint()
    {
        // Calculate velocity (change in position over time)
        endPointSpeed = (transform.position - lastPos) / Time.deltaTime;

        // Store the current position as last position for the next frame
        lastPos = transform.position;

        // Calculate the direction of movement (normalized velocity)
        rodEndPointForceDirection.Value = endPointSpeed.normalized;

        rodEndCurrentSpeed.Value = endPointSpeed.magnitude;

        // Draw a line in the Scene view to represent the velocity direction
        // Start from the fishing rod's end point (transform.position)
        // The line will extend in the direction of the movement, scaled by the speed (for visualization purposes)
        Debug.DrawLine(transform.position, transform.position + endPointSpeed, Color.green, 0.1f);
    }


}



