using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // Make the UI element face the camera
            transform.LookAt(mainCamera.transform);
            // Adjust the rotation to keep the UI upright
            transform.Rotate(0, 180f, 0); // Rotate around the Y-axis to face correctly
        }
    }
}
