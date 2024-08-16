using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    private Rigidbody rb;

    public bool isFloating = true; // Default to true
    [SerializeField] private float targetHeight = 1.0f; // Target height above the ground
    [SerializeField] private float springForce = 5.0f; // Force applied to reach the target height
    [SerializeField] private float damping = 1.0f; // Damping factor to control oscillation
    [SerializeField] private float sineWaveAmplitude = 0.1f; // Amplitude of the sine wave
    [SerializeField] private float sineWaveFrequency = 1.0f; // Frequency of the sine wave
    [SerializeField] private LayerMask groundLayerMask; // LayerMask to filter the raycast

    private Vector3 targetPosition;
    private float sineWaveOffset;
    private float distanceToGround;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (isFloating)
        {
            rb.useGravity = false; // Gravity should be off while floating
            targetPosition = transform.position;
        }

        // Add a random phase offset to the sine wave
        sineWaveOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        if (isFloating)
        {
            if (rb == null)
            {
                isFloating = false;
                return;
            }

            // Update the sine wave offset
            sineWaveOffset += Time.deltaTime * sineWaveFrequency;
            float sineWave = Mathf.Sin(sineWaveOffset) * sineWaveAmplitude;

            // Get the distance to the ground
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
            {
                distanceToGround = hit.distance;

                // Calculate the target position with sine wave offset
                float desiredHeight = targetHeight + sineWave; // The desired height above the ground, including sine wave effect
                float targetYPosition = hit.point.y + desiredHeight;

                // Calculate the force to apply
                Vector3 forceDirection = new Vector3(0, targetYPosition - transform.position.y, 0);
                Vector3 force = forceDirection * springForce - rb.velocity * damping;

                // Apply the force to the Rigidbody
                rb.AddForce(force);

                // Ensure the Rigidbody doesn’t have unwanted rotation
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    public void Release()
    {
        isFloating = true;
        rb.useGravity = false; // Gravity should be off while floating
    }

    public void PickUp()
    {
        isFloating = false;
        rb.useGravity = true; // Gravity should be on when picked up
        rb.velocity = Vector3.zero;
        sineWaveOffset = 0.0f; // Reset the sine wave offset when picked up
    }
}