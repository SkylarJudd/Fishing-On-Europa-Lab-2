using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum FishStates
{
    FishIdel,
    FishFlocking,
    FishMoveToLure,
    FishTiredPull,
    FishPullingFight,
    FishCaught



}


public class FishingMiniGameMannager : MonoBehaviour
{


    [SerializeField] GameObject caughtFish;
    [SerializeField] GameObject lure;
    [SerializeField] FishNavigationScript fishNavScipt;
    [SerializeField] ClosestObjectsFinder closestObjectsFinder;
    [SerializeField] FollowFish LureFollowFish;
    [SerializeField] RodAncorScript rodAnchorScript;
    [SerializeField] fishStats fishStat;
    [SerializeField] float nextToggle;
    [SerializeField] float toggleRate = 10f;
    [SerializeField] float toggleRange = 5f;
    [SerializeField] float pullingNextToggle = 0;
    [SerializeField] float randomRotationLast;
    [SerializeField] float pullingToggleRate = 5f;
    [SerializeField] float pullingToggleRange = 1f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float switchDelay = 1f;
    [SerializeField] bool miniGameActive = false;
    [SerializeField] bool pulling = false;
    [SerializeField] Renderer netRenderer;
    [SerializeField] Material fightingMat;
    [SerializeField] Material reelMat;
    [SerializeField] TextMeshProUGUI rodHealthText;
    
    bool dammaging = false;
    private bool isMoving = false;  // Flag to check if the fish is currently moving
    Vector3 tempTransform;
    public GameObject resetPointCube;
    public float rodHealth;
    float damagedDelay;

    [SerializeField] RodPositionState currentRodState;
    [SerializeField] RodPositionState exspectedRodState;

    bool damageInvoked = false;
    bool healInvoked = false;


    private void Update()
    {
        if (miniGameActive && caughtFish != null)
        {

            currentRodState = rodAnchorScript.rodPositionState;
            FishStates fishStates = fishNavScipt.GetStats();

            if (Time.time > nextToggle)
            {
                if (fishStates == FishStates.FishPullingFight)
                {
                    //print("FishNowTired");
                    ChangeFishDirection(0);

                    //print(" Start the movement coroutine");
                    StartCoroutine(MoveToTarget(tempTransform));

                    nextToggle = Time.time + Random.Range(toggleRate - toggleRange, toggleRate + toggleRange);
                }
                else if (fishStates == FishStates.FishTiredPull)
                {
                    if (!isMoving)
                    {
                        //print("FishNowFighting");
                        tempTransform = caughtFish.transform.position;
                        resetPointCube.transform.position = tempTransform;
                        //print("FishNowFighting");
                        ChangeState(FishStates.FishPullingFight);
                        pulling = true;
                        LureFollowFish.UpdateLureFollowFish(true);
                        netRenderer.material = fightingMat;

                        nextToggle = Time.time + Random.Range(toggleRate - toggleRange + 20, toggleRate + toggleRange + 20);
                    }
                }
                else
                {
                    //print("FishNowPull From Else"); 
                    ChangeState(FishStates.FishTiredPull);
                }
            }

            if (pulling)
            {
                if (Time.time > pullingNextToggle)
                {
                    //print("Toggling Pull Direction");
                    int randomRotation = 0;

                    while (randomRotationLast == randomRotation)
                    {
                        randomRotation = Random.Range(-1, 2);
                        //print("random Rotation " + randomRotation + " = random Rotation Last " + randomRotationLast);
                    }

                    randomRotationLast = randomRotation;
                    switchDelay = 1f;
                    ChangeFishDirection(randomRotation);
                    pullingNextToggle = Time.time + Random.Range(pullingToggleRate - pullingToggleRange, pullingToggleRate + pullingToggleRange);
                }

                switchDelay -= Time.deltaTime;
                damagedDelay -= Time.deltaTime;

               

                if (switchDelay < 0 && currentRodState != exspectedRodState && !damageInvoked)
                {
                    InvokeRepeating("DamageRod", 0f, 2f);
                    damageInvoked = true;  // Set the flag to true to avoid repeated invocation
                }
                else if (switchDelay >= 0 || currentRodState == exspectedRodState)
                {
                    CancelInvoke("DamageRod");
                    dammaging = false;
                    damageInvoked = false;  // Reset the flag when conditions are no longer met
                }
            }
            else
            {
                CancelInvoke("DamageRod");
                damageInvoked = false;
            }

            if (damagedDelay < 0 && !healInvoked && !dammaging)
            {
                InvokeRepeating("HealRod", 0f, 1f);
                healInvoked = true;  // Set the flag to true to avoid repeated invocation
            }
            else if (damagedDelay >= 0 || dammaging)
            {            
                CancelInvoke("HealRod");
                healInvoked = false;  // Reset the flag when conditions are no longer met
            }
        }
    }

    // Coroutine to move the fish to the target position
    private IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        if (!isMoving)
        {
            isMoving = true;
            ChangeState(FishStates.FishIdel);
            

            while (!FishReachedTarget())
            {
                
                // Lerp the position
                Vector3 newPosition = Vector3.Lerp(caughtFish.transform.position, targetPosition, moveSpeed * Time.deltaTime);
                caughtFish.transform.position = newPosition;
                resetPointCube.transform.position = targetPosition;
               // print("Loop");

                yield return null;
            }

            isMoving = false;
            ChangeState(FishStates.FishTiredPull);
            //print("Target reached");
            pulling = false;
            LureFollowFish.UpdateLureFollowFish(false);
            netRenderer.material = reelMat;
            exspectedRodState = RodPositionState.Middle;
        }
    }

    bool FishReachedTarget()
    {
        // Check if the fish has reached the target position
        //print("tempTransform = " + tempTransform + " fish transform = " + caughtFish.transform.position);
        resetPointCube.transform.position = tempTransform;
        float distanceToTarget = Vector3.Distance(caughtFish.transform.position, tempTransform);
        //print("Distance to target = " + distanceToTarget);
        return distanceToTarget < 0.5f; // Adjust the threshold as needed
    }

    private void DamageRod()
    {
        //damages the rod every second based on the fishes damage to rod
        rodHealth = rodHealth - fishStat.fish.damageToRod;
        rodHealthText.text = rodHealth + "/100";
        dammaging = true;
        damagedDelay = 1f;
        updateColour(rodHealth);
        if (rodHealth < 0)
        {
            print("Health hit 0 line snapped");
            rodHealthText.text = "SNAPPED!";
            CancelInvoke("DamageRod");
            damageInvoked = false;

            isMoving = false;
            pulling = false;

            ChangeState(FishStates.FishFlocking);
            LureFollowFish.UpdateLureFollowFish(false);
            
            
            netRenderer.material = reelMat;

            closestObjectsFinder.FullFishReset();


            //play snapping sound at lure location 
        }
    }

    private void HealRod()
    {
        if (rodHealth <100)
        {
            rodHealth = rodHealth + 1;
            rodHealthText.text = rodHealth + "/100";
            updateColour(rodHealth);
        }
        else
        {
            rodHealth = 100;
        }
        
        
       
    }

    void updateColour(float health)
    {
        // Set the color based on the health value
        if (health < 25)
        {
            rodHealthText.color = Color.red;
        }
        else if (health > 50)
        {
            rodHealthText.color = Color.green;
        }
        else
        {
            rodHealthText.color = Color.yellow;
        }
    }

    void ChangeState(FishStates fishStates)
    {

        if (caughtFish != null)
        {

            fishNavScipt.ToggleToCatch(fishStates);
        }
        else
        {
            print("Look Here we have a problem");
        }
    }

    void ChangeFishDirection(float randomRotation)
    {
        if (caughtFish != null)
        {
            fishNavScipt.UpdateFishDirection(randomRotation);

            if (randomRotation == 1)
            {
                exspectedRodState = RodPositionState.Left;
            }
            else if (randomRotation == 0)
            {
                
            }
            else if (randomRotation == -1)
            {
                exspectedRodState = RodPositionState.Right;
            }
            else
            {
                print("How did you get here?");
            }


        }
        else
        {
            print("Look Here we have a problem");
        }
    }

    public void ToggleTimer(bool timerState)
    {
        miniGameActive = timerState;
        caughtFish = closestObjectsFinder.caughtFish;
        fishNavScipt = caughtFish.GetComponent<FishNavigationScript>();
        fishStat = caughtFish.GetComponent<fishStats>();
        rodHealth = 100; // need to set up something later to get the rod health from somewhere else so we can have upgrades
        rodHealthText.text = rodHealth + "/100";
        updateColour(rodHealth);
    }

}
