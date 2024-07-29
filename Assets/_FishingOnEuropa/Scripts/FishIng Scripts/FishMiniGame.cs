using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class FishMiniGame : MonoBehaviour
    {
    public Transform centerOfRotation; // The center around which the rotation occurs
    public float rotationSpeed = 45f;  // Speed of rotation in degrees per second
    [Range(-1f, 1f)] public float rotationAngle = 0f; // Rotation angle controlled in the Inspector

    private void Update()
    {
        // Ensure that centerOfRotation is assigned
        if (centerOfRotation == null)
        {
            Debug.LogError("Center of rotation is not assigned!");
            return;
        }

        // Calculate the rotation axis (up vector in world space)
        Vector3 rotationAxis = Vector3.up;

        // Calculate the rotation angle based on the speed and time
        float rotationDelta = rotationSpeed * rotationAngle * Time.deltaTime;

        // Rotate the object around the center of rotation
        transform.RotateAround(centerOfRotation.position, rotationAxis, rotationDelta);

        // Optionally, clamp the rotation within a certain range
        ClampRotation();
    }

    void ClampRotation()
    {
        // Calculate the current rotation angle around the up vector
        float currentRotation = Vector3.SignedAngle(centerOfRotation.forward, transform.position - centerOfRotation.position, Vector3.up);

        // Clamp the rotation angle within the specified range
        float clampedRotation = Mathf.Clamp(currentRotation, -rotationSpeed, rotationSpeed);

        // Calculate the difference between the clamped and current rotation
        float rotationDifference = clampedRotation - currentRotation;

        // Rotate the object back by the difference to stay within the clamp range
        transform.RotateAround(centerOfRotation.position, Vector3.up, rotationDifference);
    }
}



