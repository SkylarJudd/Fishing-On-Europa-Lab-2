using Europa;
using Obvious.Soap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class VelocityCalculator : GameBehaviour
    {
        [SerializeField] private Vector3Reference rodEndPostion; // The Current Position of the rods end point
        [SerializeField] private FloatReference rodEndCurrentSpeed; // Speed magnitude
        [SerializeField] private Vector3Reference rodEndPointForceDirection;  // Direction of movement

        [SerializeField] private BoolReference playerCastInput;
        [SerializeField] private BoolReference miniGameActive;
        [SerializeField] private BoolReference miniGameRodPullDirection; //true = right , false = left


        [SerializeField] private float movementThreshold = 0.01f;  // Sensitivity threshold for movement detection

        

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

            CheckObjectSide();
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

        private void CheckObjectSide()
        {
            if(miniGameActive == true)
            {
                // Vector from the player to the object
                Vector3 playerToObject = transform.position - _PLAYER.playerHead.transform.position;

                // Get the player's right direction (relative to the player's rotation)
                Vector3 playerRight = _PLAYER.playerHead.transform.right;

                // Project the vector onto the player's right vector using the dot product
                float dotProduct = Vector3.Dot(playerToObject, playerRight);

                // Check if the object is on the left or right side of the player
                if (dotProduct > 0)
                {
                    Debug.Log("The object is on the right side of the player!");
                    miniGameRodPullDirection.Value = true;
                }
                else if (dotProduct < 0)
                {
                    Debug.Log("The object is on the left side of the player!");
                    miniGameRodPullDirection.Value = false;
                }
                else
                {
                    Debug.Log("The object is directly in front or behind the player!");
                }
            }
        }
    }
}





