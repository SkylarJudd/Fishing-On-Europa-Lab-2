using Obvious.Soap;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class ReelCounter : MonoBehaviour
    {
        [SerializeField] private BoolReference fishingMiniGameActive;
        [SerializeField] private BoolReference fishingRodCasted;
        [SerializeField] private FloatReference lureCurrentMaxDistance;
        [SerializeField] private FloatReference lureMaxDistance;


        [SerializeField] private float convertFromDegToDisScale = 0.1f;  // Scale to convert degrees to distance

        private float lastRotationX;
        private Queue<float> rotationHistory = new Queue<float>();  // Store recent rotation values
        [SerializeField] private int smoothingWindow = 5;  // How many frames to smooth over

        private bool handOnReel;

        private void Start()
        {
            // Initialize with the current X rotation of the game object
            lastRotationX = GetWorldRotationX();
        }

        private void Update()
        {
            // Check to see if the line has been cast and the mini-game is not running
            if (!fishingMiniGameActive.Value && fishingRodCasted.Value && handOnReel)
            {
                UpdateLineMaxDistance();
            }
        }

        /// <summary>
        /// Updates the current line length based on the rotation of this game object on the X-axis.
        /// It adds to the line length when rotating clockwise and subtracts when rotating counterclockwise.
        /// </summary>
        private void UpdateLineMaxDistance()
        {
            // Get the current world space X rotation and calculate relative movement
            float currentRotationX = GetWorldRotationX();

            // Add the current rotation to the history
            AddRotationToHistory(currentRotationX);

            // Get the smoothed rotation delta
            float smoothedRotationDelta = GetSmoothedRotationDelta(currentRotationX);

            smoothedRotationDelta = Mathf.Clamp(smoothedRotationDelta, -10, 10);

            Debug.Log($"Current Rotation of X {currentRotationX} Smoothed Rotation delta = {smoothedRotationDelta}");

            // Use the smoothed rotation delta instead of the raw one
            if (smoothedRotationDelta > 0)
            {
                OnRotateClockwise(smoothedRotationDelta);
            }
            else if (smoothedRotationDelta < 0)
            {
                OnRotateCounterClockwise(smoothedRotationDelta);
            }

            lastRotationX = currentRotationX;
        }

        /// <summary>
        /// Adds the current rotation value to the history, maintaining the smoothing window size.
        /// </summary>
        private void AddRotationToHistory(float currentRotationX)
        {
            rotationHistory.Enqueue(currentRotationX);

            // Keep the history size within the smoothing window
            if (rotationHistory.Count > smoothingWindow)
            {
                rotationHistory.Dequeue();
            }
        }

        /// <summary>
        /// Calculates the smoothed rotation delta by averaging the recent rotation values.
        /// </summary>
        private float GetSmoothedRotationDelta(float currentRotationX)
        {
            // Calculate the average rotation from the history
            float averageRotation = 0f;
            foreach (float rotation in rotationHistory)
            {
                averageRotation += rotation;
            }
            averageRotation /= rotationHistory.Count;

            // Return the delta between the current rotation and the average of recent rotations
            return currentRotationX - averageRotation;
        }

        private float GetWorldRotationX()
        {
            Vector3 worldUp = transform.up;  // Get this object's "up" direction in world space
            Vector3 parentUp = transform.parent.up;  // Get the parent's "up" direction

            // Find the angle between the reel's up vector and the parent's up vector
            float angle = Vector3.SignedAngle(parentUp, worldUp, transform.right);

            return NormalizeAngle(angle);
        }

        /// <summary>
        /// Normalizes the angle to be within -180 to 180 degrees to handle rotation wraparound issues.
        /// </summary>
        /// <param name="angle">The angle to normalize.</param>
        /// <returns>Normalized angle between -180 and 180 degrees.</returns>
        private float NormalizeAngle(float angle)
        {
            angle = angle % 360;
            if (angle > 180) angle -= 360;
            return angle;
        }

        /// <summary>
        /// Called when the object is rotated clockwise on the X-axis.
        /// </summary>
        /// <param name="rotationAmount">The amount the X rotation increased by.</param>
        private void OnRotateClockwise(float rotationAmount)
        {
            
            // Calculate the distance to add based on the rotation amount
            lureCurrentMaxDistance.Value += (rotationAmount * convertFromDegToDisScale);
        }

        /// <summary>
        /// Called when the object is rotated counterclockwise on the X-axis.
        /// </summary>
        /// <param name="rotationAmount">The amount the X rotation decreased by.</param>
        private void OnRotateCounterClockwise(float rotationAmount)
        {
            
            // Calculate the distance to subtract based on the rotation amount
            lureCurrentMaxDistance.Value += rotationAmount * convertFromDegToDisScale;
        }

        /// <summary>
        /// This function is called from the handle event when the player grabs and releases the handle. 
        /// </summary>
        /// <param name="_handOnReel"></param>
        public void SetHandOnReel(bool _handOnReel)
        {
            handOnReel = _handOnReel;
        }
    }
}