using UnityEngine;
using FMODUnity;

public class MovementSoundController : MonoBehaviour
{
    public StudioEventEmitter movementSoundEvent; // Drag and drop your movement sound event here
    public StudioEventEmitter idleSoundEvent; // Drag and drop your idle sound event here

    private Rigidbody rb;
    private bool isMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckMovement();
    }

    void CheckMovement()
    {
        // Check if the Rigidbody's velocity is greater than a small threshold
        if (rb.velocity.magnitude > 0.1f) // Adjust threshold as necessary
        {
            if (!isMoving)
            {
                // Start playing the movement sound
                PlayMovementSound();
                isMoving = true;
            }
        }
        else
        {
            if (isMoving)
            {
                // Start playing the idle sound
                PlayIdleSound();
                isMoving = false;
            }
        }
    }

    void PlayMovementSound()
    {
        if (movementSoundEvent != null)
        {
            movementSoundEvent.Play();
        }
    }

    void PlayIdleSound()
    {
        if (idleSoundEvent != null)
        {
            idleSoundEvent.Play();
        }
    }
}
