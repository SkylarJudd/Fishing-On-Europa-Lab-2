using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerOnLift : MonoBehaviour
{

    [SerializeField] Rigidbody elevatorTransform;
    //[SerializeField] Transform PlayerTransform;
    [SerializeField] float targetYPosition = -2.03f;
    [SerializeField] float startingYPosition = -5.163831f;
    [SerializeField] float smoothSpeed = 2f;
    //[SerializeField] float playerOffset = 2f;


    
    





    bool isPlayerOnLift = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnLift = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnLift = false;
        }
    }

    private void FixedUpdate()
    {
        if (isPlayerOnLift)
        {
            MoveElevator(targetYPosition,false);
        }
        else
        {
            MoveElevator(startingYPosition,true);
        }
    }

    void MoveElevator(float targetPositionY, bool noYeetPlayer)
    {
        Vector3 targetPosition = new Vector3(elevatorTransform.position.x, targetPositionY, elevatorTransform.position.z);
        elevatorTransform.MovePosition(Vector3.MoveTowards(elevatorTransform.position, targetPosition, smoothSpeed * Time.fixedDeltaTime));

        //Vector3 targetPosition = new Vector3(elevatorTransform.position.x, targetPositionY, elevatorTransform.position.z);
        //elevatorTransform.position = Vector3.Lerp(elevatorTransform.position, targetPosition, smoothSpeed * Time.deltaTime);

        if (noYeetPlayer == false)
        {


           // Vector3 targetPosition2 = new Vector3(PlayerTransform.position.x, targetPositionY + playerOffset, PlayerTransform.position.z);
            //PlayerTransform.MovePosition(Vector3.MoveTowards(PlayerTransform.position, targetPosition2, smoothSpeed * Time.fixedDeltaTime));
           // PlayerTransform.position = Vector3.Lerp(PlayerTransform.position, targetPosition2, smoothSpeed * Time.deltaTime);

        }
       
    }
}
