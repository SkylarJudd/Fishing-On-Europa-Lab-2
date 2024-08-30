using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishNavigationScript : GameBehaviour
{
    //from old FishingMiniGameManager script
    public enum FishStates
    {
        FishIdle,
        FishFlocking,
        FishMoveToLure,
        FishTiredPull,
        FishPullingFight,
        FishCaught



    }

    //public int fishBehaviorState; // 1 idel, 2 flock, 3 catch
    public float speed = 0.1f;
    [SerializeField] float rotationSpeed = 8.0f;
    Vector3 averageHeading;
    Vector3 averagePosition;
    float neighbourDistance = 4.0f; //lower number = less likely to flock

    [SerializeField] GameObject fishData;
    [SerializeField] GameObject fishSpawner;
    [SerializeField] BoxCollider swimAreaCollider;

    [SerializeField] GameObject turnTarget;
    [SerializeField] GameObject Lure;
    [SerializeField] float waterLevel = -0.89f;
    public bool arrived = false;
    [SerializeField] ClosestObjectsFinder closestObjectFinder;
    [SerializeField] RodPullLure canCatch;
    [SerializeField] FishSpawner fishSpawnerScript;

    [SerializeField] Transform centerOfRotation; // The center around which the rotation occurs
    [SerializeField] float fishRotationSpeed = 45f;  // Speed of rotation in degrees per second
    [Range(-1f, 1f)] public float rotationAngle = 0f; // Rotation angle controlled in the Inspector

    public bool fishCatchable = false;
    [SerializeField] FishStates fishBehaviorState;


    public Transform closestLand;

    bool done = false;
    float currentResetTime;
    float targetResetTime = 20f;
    bool resetTimerActive;


    // Start is called before the first frame update
    void Start()
    {
        waterLevel = -5.52f;
        fishBehaviorState = FishStates.FishFlocking;
        speed = Random.Range(0.5f, 1f);
        swimAreaCollider = fishSpawner.GetComponent<BoxCollider>();
        fishSpawnerScript = fishSpawner.GetComponent<FishSpawner>();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            centerOfRotation = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found. Make sure the tag is correct.");
        }

    }


    // Update is called once per frame
    private void LateUpdate()
    {
        switch (fishBehaviorState)
        {
            case FishStates.FishIdle:
                
                break;

            case FishStates.FishFlocking:
                Swim();
                break;

            case FishStates.FishMoveToLure:
                AttachToLure();
                break;




            case FishStates.FishTiredPull:
                LineAttach();
                break;

            case FishStates.FishPullingFight:
                FishPulling();
                break;

            case FishStates.FishCaught:
                FishCaught();
                break;
        }

        //if (closestObjectFinder.fishCaught == true && canCatch.canCatch == true && closestObjectFinder.caughtFish == gameObject)
        //{
        //    fishBehaviorState = FishStates.FishCaught;
        //}

    }

    void Swim()
    {
        //print("SwimCalled");
        // Calculate the current position of the fish
        Vector3 currentPosition = transform.position;

        // Define the boundaries of the swim area
        float minX = fishSpawner.transform.position.x - swimAreaCollider.size.x / 2;
        float maxX = fishSpawner.transform.position.x + swimAreaCollider.size.x / 2;
        float minZ = fishSpawner.transform.position.z - swimAreaCollider.size.z / 2;
        float maxZ = fishSpawner.transform.position.z + swimAreaCollider.size.z / 2;
        float minY = fishSpawner.transform.position.y - swimAreaCollider.size.y / 2;
        float maxY = fishSpawner.transform.position.y + swimAreaCollider.size.y / 2;

        // Check if the fish is outside the swim area
        if (currentPosition.x < minX || currentPosition.x > maxX || currentPosition.z < minZ || currentPosition.z > maxZ || currentPosition.y < minY || currentPosition.y > maxY)
        {
            //print("Going To Mid");

            // Calculate a vector pointing towards the turn target within the swim area

            Vector3 toCenter = turnTarget.transform.position - currentPosition;

            // Adjust the rotation to steer the fish towards the center
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toCenter), rotationSpeed * Time.deltaTime);

            // Move the fish forward
            transform.Translate(0, 0, Time.deltaTime * speed / 4);
        }
        else
        {
            //print("Heading not gay");
            // If the fish is inside the swim area, just move it forward
            transform.Translate(0, 0, Time.deltaTime * speed);
        }

        //prevents this functions being called every frame, to make it look more natural and increase performance
        if (Random.Range(0, 10) < 1)
        {
            //print("Applying Flocking");
            ApplyFlockRules();
        }
    }

    void ApplyFlockRules()
    {
        //bc we said the array to static we are able to refrance it here and pull the fish from it
        List<GameObject> gos = fishSpawnerScript.GetFishToDepawnList();



        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = FishSpawner.goalPos;

        float dist;

        int groupSize = 0;
        gSpeed = 0;
        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    vCentre += go.transform.position;
                    groupSize++;

                    if (dist < 3.5f)
                    {
                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }
                    FishNavigationScript anotherFlock = go.GetComponent<FishNavigationScript>();
                    gSpeed += anotherFlock.speed;
                }
            }
        }
        if (groupSize > 0)
        {
            //finds the avarge of the group center
            vCentre = vCentre / groupSize + (goalPos - this.transform.position); //+ new Vector3(Random.Range(1,-1), Random.Range(1, -1), Random.Range(1, -1)));
                                                                                 // Calculate the average speed for the group
                                                                                 //print("Gspeed before avarge" + gSpeed);
            gSpeed = gSpeed / groupSize;
            //print("group size = " + groupSize);
            // Update the fish's speed
            speed = gSpeed;
            // print("fish Is going " + speed);

            //updates the direction the fish needs to turn based on the avoid and the center values
            Vector3 direction = (vCentre + vAvoid) - transform.position;

            //slowly turns the fish 
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            }
        }
    }

    void AttachToLure()
    {

        Vector3 directionToTarget = _FMGM.bobberGameObject.transform.position - transform.position;
        float yOffset = 0.5f;
        directionToTarget = new Vector3(directionToTarget.x, directionToTarget.y - yOffset, directionToTarget.z);

        // Rotate towards the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Move towards the target
        Vector3 targetPosition = _FMGM.bobberGameObject.transform.position;

        if (targetPosition.y > waterLevel)
        {
            targetPosition.y = waterLevel;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y - yOffset, targetPosition.z), Time.deltaTime * (speed * 2));

        // Check if the object has reached the target position
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float threshold = 1f; // Adjust the threshold as needed

        if (distanceToTarget < threshold && arrived == false)
        {

            arrived = true;

            //change bobber state
            _FMGM.bobberState = FishingMiniGameManager.BobberState.AttachedFish;

        }
    }

    //void Catch()
    //{
    //    Vector3 directionToTarget = Lure.transform.position - transform.position;
    //    float yOffset = 0.5f;
    //    directionToTarget = new Vector3(directionToTarget.x, directionToTarget.y - yOffset, directionToTarget.z);

    //    // Rotate towards the target
    //    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

    //    // Move towards the target
    //    Vector3 targetPosition = Lure.transform.position;

    //    if (targetPosition.y > waterLevel)
    //    {
    //        targetPosition.y = waterLevel;
    //    }

    //    transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y - yOffset, targetPosition.z), Time.deltaTime * (speed * 2));

    //    // Check if the object has reached the target position
    //    float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
    //    float threshold = 1f; // Adjust the threshold as needed

    //    if (distanceToTarget < threshold && arrived == false)
    //    {

    //        //Debug.Log("Object has reached the target!");
    //        //fishBehaviorState = FishStates.FishIdle;
    //        arrived = true;
    //        closestObjectFinder.ArrivedUpdate();
    //    }
    //}

    public void ToggleToCatch(FishStates changeStateTo)
    {
        fishBehaviorState = changeStateTo;
    }

    public FishStates GetStats()
    {
        return fishBehaviorState;
    }



    void LineAttach()
    {

        //print("Attached to line");
        Vector3 directionToTarget = Lure.transform.position - transform.position;
        float yOffset = 0.5f;
        directionToTarget = new Vector3(directionToTarget.x, directionToTarget.y - yOffset, directionToTarget.z);
        // Rotate towards the target
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Move towards the target
        Vector3 targetPosition = Lure.transform.position;

        if (targetPosition.y > waterLevel)
        {
            targetPosition.y = waterLevel;
        }
        
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y - yOffset, targetPosition.z), Time.deltaTime * speed);
    }

    void FishPulling()
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
        float rotationDelta = fishRotationSpeed * rotationAngle * Time.deltaTime;

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
        float clampedRotation = Mathf.Clamp(currentRotation, -40f, 40f);

        // Calculate the difference between the clamped and current rotation
        float rotationDifference = clampedRotation - currentRotation;

        // Rotate the object back by the difference to stay within the clamp range
        transform.RotateAround(centerOfRotation.position, Vector3.up, rotationDifference);
    }

    public void UpdateFishDirection(float rotation)
    {
        rotationAngle = rotation;
        //print("Rotation = " + rotation);
    }

    void FishCaught()
    {
        
        if (done == false)
        {
            fishSpawnerScript.RemoveFishFromList(gameObject);
            findCloestLand();
            gameObject.transform.position = closestLand.position;
            //closestObjectFinder.FullFishReset(); //REMOVED SINCE ClosestObjectsFinder IS BEING REMOVED
            done = true;
            resetTimerActive = true;
        }

        if (resetTimerActive == true)
        {
            currentResetTime += Time.deltaTime;

            if (currentResetTime >= targetResetTime)
            {
                resetTimerActive = false;
                done = false;
                fishBehaviorState = FishStates.FishFlocking;
                currentResetTime = 0;
                fishSpawnerScript.AddFishToDespwnList(gameObject);
                //closestObjectFinder.RemoveFromFishCaughtList(gameObject); //REMOVED SINCE ClosestObjectsFinder IS BEING REMOVED

                Vector3 currentPosition = transform.position;

                // Use ClosestPointOnBounds to find the closest point on the collider
                Vector3 closestPoint = swimAreaCollider.ClosestPointOnBounds(currentPosition);

                // Set the object's position to the closest point
                transform.position = closestPoint;

                

            }
        }
        

        
       
    }

    void findCloestLand()
    {
        //add system to find cloasest point the fish can spawn
    }
}  


