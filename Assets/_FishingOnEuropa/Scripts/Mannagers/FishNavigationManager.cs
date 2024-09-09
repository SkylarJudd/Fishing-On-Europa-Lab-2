using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HybridState
{
    HybridIdle,
    HybridFlying,
    HybridHitWater,
    HybridFlocking,
    HybridAvoidingWall,

    HybridMiniGame_SwimToLure,
    HybridWaitForMiniGame,
    HybridMiniGame_Pulling,
    HybridMiniGame_Tired,
    HybridMiniGame_Caught,

    HybridOnLand_Sitting,
    HybridOnLand_Walking,
    HybridSwimToItem,
    HybridLookAtHand,
    HybridWatchPlayer,
    HybridSwimToPlayer,
}

public class FishNavigationManager : Singleton<FishNavigationManager>
{
    [Header("All Hybrids In Pond")]
    [Tooltip("a list that contains all the hybrids that has been spawnned into this pond.")]
    public List<hybridNavData> hybridsInPond = new List<hybridNavData>();

    List<hybridNavData> removeHybridsInPond = new List<hybridNavData>();

    #region Hybrid Nav Lists
    [Header("Hybrids Doing Diffrent Tasks")] // Lists for each of the hybrids doing diffrent things so we dont have to loop over them all. 
    
    [Tooltip("a list that contains all the hybrids that are Idel.")]
    [SerializeField] List<hybridNavData> hybridsIdle = new List<hybridNavData>();
    
    [Tooltip("a list that contains all the hybrids that are Flying.")]
    [SerializeField] List<hybridNavData> hybridsFlying = new List<hybridNavData>();
    
    [Tooltip("a list that contains all the hybrids that are Hitting the water.")]
    [SerializeField] List<hybridNavData> hybridsHitWater = new List<hybridNavData>();
    
    [Tooltip("a list that contains all the hybrids that are Swimming.")]
    [SerializeField] List<hybridNavData> hybridsSwimming = new List<hybridNavData>();
    
    [Tooltip("a list that contains all the hybrids that are Swimming To the Lure or an item in the players hand.")]
    [SerializeField] List<hybridNavData> hybridSwimToPoint = new List<hybridNavData>();
    
    [Tooltip("a list that contains all the hybrids that are reacting to the player.")]
    [SerializeField] List<hybridNavData> hybridReactToPlayer = new List<hybridNavData>();

    [Tooltip("Holds the info about he Hybrid that has Been Caught")]
    public hybridNavData caughtHybrid;
    #endregion

    #region RemoveHybridLists
    List<hybridNavData> removeHybridsIdle = new List<hybridNavData>();
    List<hybridNavData> removeHybridsFlying = new List<hybridNavData>();
    List<hybridNavData> removeHybridsHitWater = new List<hybridNavData>();
    List<hybridNavData> removeHybridsSwimming = new List<hybridNavData>();
    List<hybridNavData> removeHybridSwimToLure = new List<hybridNavData>();
    List<hybridNavData> removeHybridReact = new List<hybridNavData>();
    #endregion

    #region Boids Settings
    [Header("Boids Nav Settings")]
    [SerializeField] int applyBoidsChance;
    [SerializeField] int rayCastCheckChance;
    [SerializeField] bool debug = true;

    // boid behavior range
    [SerializeField][Range(0.0f, 3.0f)] float separationRange;
    [SerializeField][Range(0.0f, 3.0f)] float alignmentRange;
    [SerializeField][Range(0.0f, 3.0f)] float cohesionRange;

    // boid behavior weights
    [SerializeField][Range(0.0f, 3.0f)] float separationFactor;
    [SerializeField][Range(0.0f, 3.0f)] float alignmentFactor;
    [SerializeField][Range(0.0f, 3.0f)] float cohesionFactor;
    #endregion

    [SerializeField] float correctRotationSpeed = 0.1f; //being used by UpdateHitWater()

    #region Hybrid Nav Coroutines
    private Coroutine updateHybridFlyingCoroutine;
    private Coroutine updateHitWaterCoroutine;
    private Coroutine updateSwimmingCoroutine;
    private Coroutine updateSwimToLure;
    private Coroutine UpdateHybridStatesCoroutine;
    private Coroutine UpdateHybridReactCoroutine;
    #endregion

    private GameObject lureLocation;

    private float waterDrag = 5.0f;
    private float airDrag = 0.0f;

    [SerializeField] LayerMask fishCollisionLayerMask;

    [Serializable]
    public class hybridNavData
    {
        [Header("Hybrid States")]


        [Header("Hybrid Info")]
        public GameObject hybridGameObject;
        public Rigidbody hybridRigidbody;
        public PondType pondType;
        public FoodType[] foodEaten;
        public ToyList favToy;

        public bool isTamed = true; //TESTING PURPOSES


        [Header("Hybrid Nav")]

        public HybridState HybridState;
        public Vector3 velocity;

        public float minSpeed;
        public float maxSpeed;
        public float rotationSpeed;
        public float waterHeight;

        public bool isTurning;
        public bool aboutToHitWall = false;
        public bool firstNav;

        [Header("Hybrid Player Interact")]
        public float distanceToPlayer;
        public float distanceToLeftHand;
        public float distanceToRightHand;

        [Header("Hybrid MiniGame")]
        public bool arrivedAtLure = false;


        public Transform itemTarget; // this is used for the item target you can set the lure, 
        
    }

    [Header("Player Interaction Settings")]
    [SerializeField] float playerReactionDistance;
    [SerializeField] float playerReactionDistanceReset;

    [SerializeField] float playerHandReactionDistance;
    [SerializeField] float playerHandReactionDistanceReset;

    [SerializeField] float moveToPlayerStoppingDistance;

    enum PlayerItemState { FoodItem, PlushieItem, NoItem}
    PlayerItemState playerItemState;



    private void Start()
    {
        //TESTING ONLY REMOVE LATER!!!!!!!!!
        lureLocation = _FMGM.bobberTipGO;

        StartCoroutines();
    }

    private void StartCoroutines()
    {
        updateHybridFlyingCoroutine = StartCoroutine(UpdateHybridFlying());
        updateHitWaterCoroutine = StartCoroutine(UpdateHitWater());
        updateSwimmingCoroutine = StartCoroutine(UpdateSwimming());
        updateSwimToLure = StartCoroutine(UpdateHybridToLure());
        UpdateHybridStatesCoroutine = StartCoroutine(UpdateHybridStates());
        UpdateHybridReactCoroutine = StartCoroutine(UpdateHybridReact());
    }

    /// <summary>
    /// Adds a new hybrid GameObject to the pond list, setting its initial state and other parameters.
    /// </summary>
    /// <param name="go">The GameObject representing the hybrid.</param>
    /// <param name="enterState">Initial state of the hybrid when added to the pond.</param>
    /// <param name="_waterHight">The water height for the hybrid to maintain.</param>
    /// <param name="_pondType">The type of pond the hybrid belongs to.</param>
    public void addHybridToPondList(GameObject go, HybridState enterState, float _waterHight, PondType _pondType)
    {
        // Create a new hybrid navigation data entry
        hybridNavData newEntry = new hybridNavData
        {
            hybridGameObject = go,
            waterHeight = _waterHight,
            pondType = _pondType
        };

        // Retrieve hybrid specific information from the GameObject
        FOEItem_Hybrid hybridInfo = go.GetComponent<FOEItem_Hybrid>();
        if (hybridInfo == null)
        {
            // Log error if the hybrid info is not attached to the GameObject
            Debug.LogError($"{go.name} does not have Hybrid Info attached");
            return;
        }

        // Set initial visual state for the hybrid in the world
        hybridInfo.SetVisuals(HybridVisualsState.World);

        // Initialize movement and state parameters from hybrid info
        newEntry.minSpeed = hybridInfo.hybridSO.fishSpeed;
        newEntry.maxSpeed = hybridInfo.hybridSO.fishSpeed * 2;
        newEntry.HybridState = enterState;
        newEntry.velocity = Vector3.forward * hybridInfo.hybridSO.fishSpeed;
        newEntry.rotationSpeed = hybridInfo.hybridSO.rotationSpeed;
        newEntry.firstNav = true;  // Indicates this is the first navigation update
        newEntry.foodEaten = hybridInfo.hybridSO.foodEaten;

        // Assign Rigidbody to hybrid, or add one if it is missing
        newEntry.hybridRigidbody = go.GetComponent<Rigidbody>();
        if (newEntry.hybridRigidbody == null)
        {
            Debug.LogWarning($"{go.name} does not have a Rigidbody; one has been assigned");
            newEntry.hybridRigidbody = newEntry.hybridGameObject.AddComponent<Rigidbody>();
        }

        // Add the new hybrid to the main pond list
        hybridsInPond.Add(newEntry);

        // Add the new hybrid to the appropriate state-specific list based on the entry state
        switch (enterState)
        {
            case HybridState.HybridFlying:
                hybridsFlying.Add(newEntry);
                break;
            case HybridState.HybridFlocking:
                hybridsSwimming.Add(newEntry);
                print("Adding Hybrid to Swim List");
                break;
            default:
                hybridsSwimming.Add(newEntry);  // Default to swimming if state is not specified
                print("Adding Hybrid to Swim List");
                break;
        }
    }

    #region OldUpdateHybridState
    //private IEnumerator UpdateHybridStates()
    //{
    //    while (true)
    //    {
    //        if (hybridsInPond.Count > 0)
    //        {
    //            foreach (hybridNavData _hybrid in hybridsInPond)
    //            {
    //                _hybrid.distanceToPlayer = Vector3.Distance(_PLAYER.player.transform.position, _hybrid.hybridGameObject.transform.position);
    //                if (_hybrid.distanceToPlayer < playerReactionDistance)
    //                {
    //                    if (hybridReactToPlayer.Contains(_hybrid) == false)
    //                    {
    //                        removeHybrid(_hybrid.hybridGameObject, false);
    //                        hybridReactToPlayer.Add(_hybrid);
    //                    }

    //                    float distanceToRightHand = 0;
    //                    float distanceToLeftHand = 0;

    //                    if (_PLAYER.hasItem == false)
    //                    {
    //                        distanceToRightHand = Vector3.Distance(_hybrid.hybridGameObject.transform.position, _PLAYER.rightHand.transform.position);
    //                        distanceToLeftHand = Vector3.Distance(_hybrid.hybridGameObject.transform.position, _PLAYER.leftHand.transform.position);
    //                    }


    //                    if (_PLAYER.hasItem == true && (_PLAYER.rightHandFood != null || _PLAYER.leftHandFood != null))
    //                    {
    //                        FOEItem_Food _rightFood = _PLAYER.rightHandFood;
    //                        FOEItem_Food _leftFood = _PLAYER.rightHandFood;

    //                        foreach (FoodType _food in _hybrid.foodEaten)
    //                        {
    //                            if (_food == _leftFood.foodType)
    //                            {
    //                                _hybrid.itemTarget = _PLAYER.leftHandFood.gameObject.transform;
    //                                _hybrid.HybridState = HybridState.HybridSwimToItem;
    //                                if (_hybrid.distanceToPlayer < moveToPlayerStoppingDistance)
    //                                {
    //                                    //swim to hand then look at hand
    //                                    _hybrid.HybridState = HybridState.HybridLookAtHand;
    //                                }
    //                            }
    //                            else if (_food == _rightFood.foodType)
    //                            {
    //                                _hybrid.itemTarget = _PLAYER.rightHandFood.gameObject.transform;
    //                                _hybrid.HybridState = HybridState.HybridSwimToItem;
    //                                if (_hybrid.distanceToPlayer < moveToPlayerStoppingDistance)
    //                                {
    //                                    _hybrid.HybridState = HybridState.HybridLookAtHand;
    //                                }
    //                            }
    //                        }
    //                    }
    //                    else if (_PLAYER.hasItem == true && (_PLAYER.rightHandPlushie != null || _PLAYER.leftHandPlushie != null))
    //                    {
    //                        if (_hybrid.favToy == _PLAYER.leftHandPlushie.ToyItem)
    //                        {
    //                            _hybrid.itemTarget = _PLAYER.leftHandPlushie.gameObject.transform;
    //                            _hybrid.HybridState = HybridState.HybridSwimToItem;
    //                            if (_hybrid.distanceToPlayer < moveToPlayerStoppingDistance)
    //                            {
    //                                // swim to until close enough then look at it
    //                                _hybrid.HybridState = HybridState.HybridLookAtHand;
    //                            }
    //                        }
    //                        else if (_hybrid.favToy == _PLAYER.rightHandPlushie.ToyItem)
    //                        {
    //                            _hybrid.itemTarget = _PLAYER.rightHandPlushie.gameObject.transform;
    //                            _hybrid.HybridState = HybridState.HybridSwimToItem;
    //                            if (_hybrid.distanceToPlayer < moveToPlayerStoppingDistance)
    //                            {
    //                                _hybrid.HybridState = HybridState.HybridLookAtHand;
    //                            }
    //                        }
    //                    }
    //                    else if (_PLAYER.hasItem == false && (distanceToRightHand < playerHandReactionDistance || distanceToLeftHand < playerHandReactionDistance))
    //                    {

    //                        // Compare distances and set the target to the closer hand
    //                        if (distanceToRightHand < distanceToLeftHand)
    //                        {
    //                            _hybrid.itemTarget = _PLAYER.rightHand;
    //                        }
    //                        else
    //                        {
    //                            _hybrid.itemTarget = _PLAYER.leftHand;
    //                        }

    //                        // Set the Hybrid's state to look at the selected hand
    //                        _hybrid.HybridState = HybridState.HybridLookAtHand;
    //                    }
    //                    else
    //                    {

    //                        _hybrid.HybridState = HybridState.HybridWatchPlayer;
    //                    }
    //                }
    //                else if (_hybrid.distanceToPlayer > playerReactionDistanceReset && _hybrid.HybridState == HybridState.HybridWatchPlayer)
    //                {
    //                    _hybrid.HybridState = HybridState.HybridFlocking;

    //                    removeHybridReact.Add(_hybrid);
    //                    hybridsSwimming.Add(_hybrid);
    //                }

    //            }
    //            // Remove hybrids from hybridsHitWater
    //            foreach (var _hybrid in removeHybridReact)
    //            {
    //                hybridReactToPlayer.Remove(_hybrid);
    //            }
    //            removeHybridReact.Clear();
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
    #endregion

    /// <summary>
    /// Continuously updates the state of each hybrid in the pond, reacting to the player's proximity and actions.
    /// This coroutine runs in a loop and processes interactions between the player and hybrids based on distance and held items.
    /// </summary>
    private IEnumerator UpdateHybridStates()
    {
        // Continuously update hybrid states runs in a while loop so its not creating garbage creating new Corutines each time
        while (true)
        {
            // check to see if there are any hybrids in the pond and only if there are hybrids in the pond will continue
            if (hybridsInPond.Count > 0)
            {
                // Iterate through each hybrid in the pond
                foreach (hybridNavData _hybrid in hybridsInPond)
                {

                    //if (!_hybrid.isTamed)
                    //{
                    //    break;
                        
                    //}

                    // Update distance the hybrid is to the player
                    _hybrid.distanceToPlayer = Vector3.Distance(_PLAYER.player.transform.position, _hybrid.hybridGameObject.transform.position);


                    // Check if the hybrid is within the player's reaction distance
                    if (_hybrid.distanceToPlayer < playerReactionDistance)
                    {
                        // Add the hybrid to the react list if it's not already included
                        if (!hybridReactToPlayer.Contains(_hybrid))
                        {
                            RemoveHybrid(_hybrid.hybridGameObject, false);
                            hybridReactToPlayer.Add(_hybrid);

                            //Note For Lilli ( Make Hybrid Swim to player, set their state to something like swim to player then in UpdateHybridReact have a loop that will make them swim towards the player until
                            //they reach a wall, you can do this with a ray cast simmilar to the one below that was used in the flocking corutine. 

                            #region Notes For Lilli
                            //if (UnityEngine.Random.Range(0, rayCastCheckChance) < 1 && hybridsSwimming.Count > 1)
                            //{
                            //    Ray hybridRay = new Ray(_hybrid.hybridGameObject.transform.position, _hybrid.hybridGameObject.transform.forward);
                            //    float raycastDistance = 0.5f;
                            //    int layerMask = ~LayerMask.GetMask("Fish");

                            //    if (debug)
                            //        Debug.DrawRay(hybridRay.origin, hybridRay.direction * raycastDistance, Color.red);

                            //    if (Physics.Raycast(hybridRay, out RaycastHit hit, raycastDistance, layerMask))
                            //    {
                            //        if (hit.collider.gameObject.CompareTag("Wall"))
                            //        {
                            //            _hybrid.aboutToHitWall = true;
                            //        }
                            //        else
                            //        {
                            //            _hybrid.aboutToHitWall = false;
                            //            _hybrid.HybridState = HybridState.HybridFlocking;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        _hybrid.aboutToHitWall = false;
                            //        _hybrid.HybridState = HybridState.HybridFlocking;
                            //    }
                            //}
                            #endregion
                        }

                        // Check if player is holding an item
                        if (_PLAYER.hasItem)
                        {
                            ProcessPlayerWithItem(_hybrid);
                        }
                        else
                        {
                            ProcessPlayerWithoutItem(_hybrid);
                        }
                    }
                    else
                    {
                        // Clean up hybrids that are no longer reacting
                        foreach (var _hybridInReact in removeHybridReact)
                        {
                            hybridReactToPlayer.Remove(_hybridInReact);
                        }
                        removeHybridReact.Clear();

                        if (_hybrid.HybridState == HybridState.HybridFlocking)
                        {
                            
                        }
                        if(hybridReactToPlayer.Contains(_hybrid))  
                        {
                            // Reset hybrid state to flocking when the player moves away beyond reset distance
                            _hybrid.HybridState = HybridState.HybridFlocking;
                            removeHybridReact.Add(_hybrid);
                            hybridsSwimming.Add(_hybrid);
                        }
                    }


                }

            }

            // Wait until the next physics update
            yield return new WaitForFixedUpdate();
        }
    }


    /// <summary>
    /// Handles hybrid interactions when the player is holding an item, checking for specific reactions based on the type of item held.
    /// </summary>
    private void ProcessPlayerWithItem(hybridNavData _hybrid)
    {
        
        FOEItem_Food _rightFood = _PLAYER.rightHandFood;
        FOEItem_Food _leftFood = _PLAYER.leftHandFood;

        // Check for food interactions
        if (_PLAYER.rightHandFood != null || _PLAYER.leftHandFood != null)
        {
            CheckFoodInteraction(_hybrid, _rightFood, _leftFood);
        }
        // Check for plushie interactions
        else if (_PLAYER.rightHandPlushie != null || _PLAYER.leftHandPlushie != null)
        {
            CheckPlushieInteraction(_hybrid);
        }
    }

    /// <summary>
    /// Processes hybrid interactions when the player is not holding any item, determining reactions based on proximity to the player's hands.
    /// </summary>
    private void ProcessPlayerWithoutItem(hybridNavData _hybrid)
    {
        // Calculate distances to player's hands
        float distanceToRightHand = Vector3.Distance(_hybrid.hybridGameObject.transform.position, _PLAYER.rightHand.transform.position);
        float distanceToLeftHand = Vector3.Distance(_hybrid.hybridGameObject.transform.position, _PLAYER.leftHand.transform.position);

        // Determine which hand is closer, or default to watching the player
        if (distanceToRightHand < playerHandReactionDistance || distanceToLeftHand < playerHandReactionDistance)
        {
            _hybrid.itemTarget = (distanceToRightHand < distanceToLeftHand) ? _PLAYER.rightHand : _PLAYER.leftHand;
            _hybrid.HybridState = HybridState.HybridLookAtHand;
        }
        else
        {
            _hybrid.HybridState = HybridState.HybridWatchPlayer;
        }
    }

    /// <summary>
    /// Checks for food interactions between the player's held items and the hybrid's preferences, setting the target and state accordingly.
    /// </summary>
    private void CheckFoodInteraction(hybridNavData _hybrid, FOEItem_Food _rightFood, FOEItem_Food _leftFood)
    {
        // Interact with the type of food the hybrid eats
        foreach (FoodType _food in _hybrid.foodEaten)
        {
            if (_food == _leftFood.foodType)
            {
                SetHybridTargetState(_hybrid, _PLAYER.leftHandFood.gameObject.transform);
            }
            else if (_food == _rightFood.foodType)
            {
                SetHybridTargetState(_hybrid, _PLAYER.rightHandFood.gameObject.transform);
            }
        }
    }

    /// <summary>
    /// Checks plushie interactions based on the hybrid's favorite toy and the player's held plushies.
    /// </summary>
    private void CheckPlushieInteraction(hybridNavData _hybrid)
    {
        // Interact with the favorite toy
        if (_hybrid.favToy == _PLAYER.leftHandPlushie.ToyItem)
        {
            SetHybridTargetState(_hybrid, _PLAYER.leftHandPlushie.gameObject.transform);
        }
        else if (_hybrid.favToy == _PLAYER.rightHandPlushie.ToyItem)
        {
            SetHybridTargetState(_hybrid, _PLAYER.rightHandPlushie.gameObject.transform);
        }
    }

    /// <summary>
    /// Sets the hybrid's target and updates its state based on its proximity to the target and current behavior parameters.
    /// </summary>
    private void SetHybridTargetState(hybridNavData _hybrid, Transform targetTransform)
    {
        // Set target and update state based on proximity to the player
        _hybrid.itemTarget = targetTransform;

        _hybrid.HybridState = HybridState.HybridSwimToItem;
        if (_hybrid.distanceToPlayer < moveToPlayerStoppingDistance)
        {
            _hybrid.HybridState = HybridState.HybridLookAtHand;
        }
    }

    private IEnumerator UpdateIdle()
    {
        while (true)
        {
            if (hybridsIdle.Count > 0)
            {
                foreach (hybridNavData _hybrid in hybridsIdle)
                {
                    //Play Idle animation
                }
                // Remove hybrids from hybridsHitWater
                foreach (var _hybrid in removeHybridsIdle)
                {
                    hybridsIdle.Remove(_hybrid);
                }
                removeHybridsIdle.Clear();
            }
            yield return new WaitForFixedUpdate();
        }
    }


    /// <summary>
    /// loops though the list of Flying Hybrids and updates their rigid body values and check when they hit the water. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateHybridFlying()
    {
        while (true)
        {

            if (hybridsFlying.Count > 0)
            {
                //print("Hybrids flying count is grater then 0");
                for (int i = hybridsFlying.Count - 1; i >= 0; i--)
                {
                    hybridNavData _hybrid = hybridsFlying[i];

                    // Animate the Hybrids

                    // Check if Hybrid has left the water
                    if (_hybrid.HybridState != HybridState.HybridFlying)
                    {
                        UpdateRB(_hybrid.hybridRigidbody, true, airDrag, airDrag);
                        _hybrid.HybridState = HybridState.HybridFlying;
                    }

                    //print($"{_hybridd} has a y value of {_hybridd.hybridGameObject.transform.position.y} and the water hight is {waterHight}.y ");
                    // Check if the Hybrids have hit the water
                    if (_hybrid.hybridGameObject.transform.position.y <= _hybrid.waterHeight)
                    {
                        hybridsHitWater.Add(_hybrid);
                        //hybridsFlying.RemoveAt(i);
                        removeHybridsFlying.Add(_hybrid);
                    }
                }

                // Remove hybrids from hybridsHitWater
                foreach (var hybrid in removeHybridsFlying)
                {
                    hybridsFlying.Remove(hybrid);
                }
                removeHybridsFlying.Clear();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Loops through the list of hybrids that have hit the water and updates their rigid bodies and corrects their rotation
    /// to ensure they align properly with the water surface. Hybrids are moved to the swimming state once their rotation is corrected.
    /// </summary>
    private IEnumerator UpdateHitWater()
    {
        while (true)
        {
            if (hybridsHitWater.Count > 0)
            {
                for (int i = hybridsHitWater.Count - 1; i >= 0; i--)
                {
                    hybridNavData _hybrid = hybridsHitWater[i];

                    // Initialize hybrid's physics when they first hit the water
                    if (_hybrid.HybridState != HybridState.HybridHitWater)
                    {
                        // Disable gravity and adjust drag to simulate water resistance
                        UpdateRB(_hybrid.hybridRigidbody, false, waterDrag, waterDrag);
                        _hybrid.HybridState = HybridState.HybridHitWater;
                    }

                    // Define and interpolate towards the target rotation to align with the water surface
                    Quaternion targetRotation = Quaternion.Euler(0, _hybrid.hybridGameObject.transform.rotation.eulerAngles.y, 0);
                    _hybrid.hybridGameObject.transform.rotation = Quaternion.Lerp(_hybrid.hybridGameObject.transform.rotation, targetRotation, Time.deltaTime * correctRotationSpeed);

                    // Check if the hybrid is aligned within acceptable thresholds to transition to swimming
                    Vector3 currentRotation = _hybrid.hybridGameObject.transform.rotation.eulerAngles;
                    if (IsRotationWithinRange(currentRotation))
                    {
                        // Mark for transition to swimming state
                        hybridsSwimming.Add(_hybrid);
                        removeHybridsHitWater.Add(_hybrid);
                        _hybrid.velocity = _hybrid.hybridGameObject.transform.position;
                    }
                }

                // Remove processed hybrids from the hit water list
                foreach (var hybrid in removeHybridsHitWater)
                {
                    hybridsHitWater.Remove(hybrid);
                }
                removeHybridsHitWater.Clear();
            }
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Checks if the given rotation is within the acceptable range to be considered aligned with the water.
    /// </summary>
    /// <param name="rotation">The current rotation as Euler angles.</param>
    /// <returns>True if within range, otherwise false.</returns>
    private bool IsRotationWithinRange(Vector3 rotation)
    {
        return (rotation.x < 0.5f || rotation.x > 359.5f) && (rotation.z < 0.5f || rotation.z > 359.5f);
    }

    /// <summary>
    /// A helper fuction to updates the properties of a inputed Rigid Body. 
    /// </summary>
    /// <param name="_rb">The Rigid Body you want to update the values of</param>
    /// <param name="_gravity">To use Gravity or not</param>
    /// <param name="_drag">The drag of the Rigid body</param>
    /// <param name="_angularDrag">The agular drag of the Rigid body</param>
    private void UpdateRB(Rigidbody _rb, bool _gravity, float _drag, float _angularDrag)
    {
        _rb.useGravity = _gravity;
        _rb.drag = _drag;
        _rb.angularDrag = _angularDrag;
    }

    /// <summary>
    /// loops though the list of hybrids that are swimming and calls the fuctions that are needed for each hybrid to move. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateSwimming()
    {
        while (true)
        {
            if (hybridsSwimming.Count > 0)
            {
                foreach (hybridNavData _hybrid in hybridsSwimming)
                {
                    bool outofWater = false;

                    if (_hybrid.hybridGameObject.transform.position.y > _hybrid.waterHeight)
                    {
                        outofWater = true;
                    }

                    if (UnityEngine.Random.Range(0, rayCastCheckChance) < 1 && hybridsSwimming.Count > 1)
                    {
                        Ray hybridRay = new Ray(_hybrid.hybridGameObject.transform.position, _hybrid.hybridGameObject.transform.forward);
                        float raycastDistance = 0.5f;

                        if (debug)
                            Debug.DrawRay(hybridRay.origin, hybridRay.direction * raycastDistance, Color.red);

                        if (Physics.Raycast(hybridRay, out RaycastHit hit, raycastDistance, fishCollisionLayerMask))
                        {
                            if (hit.collider.gameObject.CompareTag("Wall"))
                            {
                                _hybrid.aboutToHitWall = true;
                            }
                            else
                            {
                                _hybrid.aboutToHitWall = false;
                                _hybrid.HybridState = HybridState.HybridFlocking;
                            }
                        }
                        else
                        {
                            _hybrid.aboutToHitWall = false;
                            _hybrid.HybridState = HybridState.HybridFlocking;
                        }
                    }

                    if (UnityEngine.Random.Range(0, applyBoidsChance) < 1 && hybridsSwimming.Count > 1 || _hybrid.firstNav == true)
                    {
                        _hybrid.firstNav = false;
                        Vector3 separationVelocity = Vector3.zero;
                        Vector3 alignmentVelocity = Vector3.zero;
                        Vector3 cohesionVelocity = Vector3.zero;

                        int numOfBoidsToAvoid = 0;
                        int numOfBoidsToAlignWith = 0;
                        int numOfBoidsInFlock = 0;
                        Vector3 currBoidPosition = _hybrid.hybridGameObject.transform.position;
                        Vector3 positionToMoveTowards = Vector3.zero;

                        foreach (hybridNavData _otherBoid in hybridsSwimming)
                        {
                            if (ReferenceEquals(_otherBoid, _hybrid))
                            {
                                continue;
                            }

                            Vector3 otherBoidsPosition = _otherBoid.hybridGameObject.transform.position;
                            float dist = Vector3.Distance(currBoidPosition, otherBoidsPosition);

                            // Separation Check
                            if (dist < separationRange)
                            {
                                Vector3 otherBoidToCurrentBoid = currBoidPosition - otherBoidsPosition;
                                Vector3 dirToTravel = otherBoidToCurrentBoid.normalized;
                                separationVelocity += dirToTravel / dist;
                                numOfBoidsToAvoid++;
                            }

                            // Alignment Check
                            if (dist < alignmentRange)
                            {
                                alignmentVelocity += _otherBoid.velocity;
                                numOfBoidsToAlignWith++;
                            }

                            // Cohesion Check
                            if (dist < cohesionRange)
                            {
                                positionToMoveTowards += otherBoidsPosition;
                                numOfBoidsInFlock++;
                            }
                        }

                        if (numOfBoidsToAvoid != 0)
                        {
                            separationVelocity /= numOfBoidsToAvoid;
                            separationVelocity.Normalize();
                            separationVelocity *= separationFactor;
                        }

                        if (numOfBoidsToAlignWith != 0)
                        {
                            alignmentVelocity /= numOfBoidsToAlignWith;
                            alignmentVelocity.Normalize();
                            alignmentVelocity *= alignmentFactor;
                        }

                        if (numOfBoidsInFlock != 0)
                        {
                            positionToMoveTowards /= numOfBoidsInFlock;
                            Vector3 cohesionDirection = positionToMoveTowards - currBoidPosition;
                            cohesionDirection.Normalize();
                            cohesionVelocity = cohesionDirection * cohesionFactor;
                        }

                        _hybrid.velocity += separationVelocity;
                        _hybrid.velocity += alignmentVelocity;
                        _hybrid.velocity += cohesionVelocity;

                        _hybrid.velocity = Vector3.ClampMagnitude(_hybrid.velocity, _hybrid.maxSpeed);

                        Vector3 direction = _hybrid.velocity.normalized;
                        float speed = _hybrid.velocity.magnitude;
                        speed = Mathf.Clamp(speed, _hybrid.minSpeed, _hybrid.maxSpeed);
                        _hybrid.velocity = direction * speed;
                    }

                    if (outofWater)
                    {
                        _hybrid.velocity.y = -Mathf.Abs(_hybrid.velocity.y);
                    }

                    if (_hybrid.aboutToHitWall == true && _hybrid.HybridState != HybridState.HybridAvoidingWall)
                    {
                        Vector3 oppositeDirection = -_hybrid.hybridGameObject.transform.forward;
                        _hybrid.velocity = oppositeDirection * _hybrid.maxSpeed;
                        _hybrid.HybridState = HybridState.HybridAvoidingWall;
                    }

                    // Move the Hybrid in the direction of Velocity
                    _hybrid.hybridGameObject.transform.position += _hybrid.velocity * Time.deltaTime;

                    // Rotate the Hybrid toward the direction it is moving
                    Quaternion targetRotation = Quaternion.LookRotation(_hybrid.velocity);
                    //Debug.Log($"Updating Rotation: {_Hybrid.hybridGameObject.name} Current Rotation: {_Hybrid.hybridGameObject.transform.rotation} Target Rotation: {targetRotation}");
                    _hybrid.hybridGameObject.transform.rotation = Quaternion.Lerp(_hybrid.hybridGameObject.transform.rotation, targetRotation, _hybrid.rotationSpeed * Time.deltaTime);

                }

                // Remove hybrids from hybridsHitWater
                foreach (var hybrid in removeHybridsSwimming)
                {
                    hybridsSwimming.Remove(hybrid);
                }
                removeHybridsSwimming.Clear();
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator UpdateHybridToLure()
    {
        while (true)
        {
            if (hybridSwimToPoint.Count > 0)
            {
                foreach (hybridNavData _hybrid in hybridSwimToPoint)
                {
                    Vector3 directionToTarget = lureLocation.transform.position - _hybrid.hybridGameObject.transform.position;
                    float yOffset = 0.1f;
                    directionToTarget = new Vector3(directionToTarget.x, directionToTarget.y - yOffset, directionToTarget.z);

                    // Rotate towards the target
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    _hybrid.hybridGameObject.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _hybrid.rotationSpeed);

                    // Move towards the target
                    Vector3 targetPosition = lureLocation.transform.position;

                    if (targetPosition.y > _hybrid.waterHeight)
                    {
                        targetPosition.y = _hybrid.waterHeight;
                    }
                    _hybrid.hybridGameObject.transform.position = Vector3.Lerp(_hybrid.hybridGameObject.transform.position, new Vector3(targetPosition.x, targetPosition.y - yOffset, targetPosition.z), Time.deltaTime * (_hybrid.maxSpeed * 2));

                    // Check if the object has reached the target position
                    float distanceToTarget = Vector3.Distance(_hybrid.hybridGameObject.transform.position, targetPosition);
                    float threshold = 0.2f; // Adjust the threshold as needed

                    print(distanceToTarget);

                    if (distanceToTarget < threshold && _hybrid.arrivedAtLure == false)
                    {
                        Debug.Log("Object has reached the target!");
                        _hybrid.HybridState = HybridState.HybridIdle;
                        _hybrid.arrivedAtLure = true;
                        hybridsIdle.Add(_hybrid);
                        removeHybridSwimToLure.Add(_hybrid);

                        //send an update to minigame Manager that the Hybrid has arrived
                        _FMGM.bobberState = FishingMiniGameManager.BobberState.AttachedFish;

                        //TEMP
                        _FMGM.BeginFightingPeriod();

                        caughtHybrid = GetHybridFromGO(_FMGM.targetHybrid);

                        //start minigame ienumerator
                        StartCoroutine(UpdateMiniGame());
                    }

                }
                foreach (var hybrid in removeHybridSwimToLure)
                {
                    hybridSwimToPoint.Remove(hybrid);
                }
                removeHybridSwimToLure.Clear();
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator UpdateMiniGame()
    {
        while (true)
        {
            if (caughtHybrid != null)
            {
                if (caughtHybrid.HybridState == HybridState.HybridMiniGame_Pulling)
                {
                    print("Fighintg");
                    
                    caughtHybrid.hybridGameObject.transform.LookAt(lureLocation.transform.position);

                    // Move towards the target
                    //Vector3 targetPosition = _FMGM.bobberFishSpot.transform.position;

                    //if (targetPosition.y > caughtHybrid.waterHeight)
                    //{
                    //    targetPosition.y = caughtHybrid.waterHeight;
                    //}
                    //caughtHybrid.hybridGameObject.transform.position = Vector3.Lerp(caughtHybrid.hybridGameObject.transform.position, new Vector3(targetPosition.x, targetPosition.y-yOffset, targetPosition.z), Time.deltaTime * (caughtHybrid.maxSpeed * 2));

                    if (_FMGM.currentPullDirection == FishingMiniGameManager.PullDirections.NotSet) _FMGM.currentPullDirection = _FMGM.GetDirection();


                    Vector3 rotationAxis = new();
                    Vector3 direction = new();
                    
                    switch (_FMGM.currentPullDirection)
                    {
                        case FishingMiniGameManager.PullDirections.Left:

                            //set bobber rotation
                            rotationAxis = Vector3.down;
                            direction = Vector3.right;
                            //reset temp

                            break;
                        case FishingMiniGameManager.PullDirections.Right:
                            rotationAxis = Vector3.up;
                            direction = Vector3.left;


                            break;
                        case FishingMiniGameManager.PullDirections.Middle:
                            rotationAxis = Vector3.zero;
                            direction = Vector3.zero;


                            break;
                    }

                    print(rotationAxis);

                    float deltaAngle = _FMGM.fishRotationSpeed * Time.deltaTime;
                    var targetDir = _PLAYER.transform.position - caughtHybrid.hybridGameObject.transform.position;

                    float currentAngle = Vector3.Angle(targetDir, _PLAYER.transform.forward);
                    print("angle " + currentAngle);

                    if (currentAngle < _FMGM.maxAngle && !Physics.CheckSphere(caughtHybrid.hybridGameObject.transform.position, 1, fishCollisionLayerMask))
                    {
                        caughtHybrid.hybridGameObject.transform.RotateAround(_PLAYER.transform.position, rotationAxis, deltaAngle);


                    }
                    //right rotation

                    //left rotation




                }
                else if (caughtHybrid.HybridState == HybridState.HybridMiniGame_Tired)
                {

                    print("Attached to line");
                    Vector3 directionToTarget = lureLocation.transform.position - transform.position;
                    float yOffset = 0.1f;
                    directionToTarget = new Vector3(directionToTarget.x, directionToTarget.y - yOffset, directionToTarget.z);
                    // Rotate towards the target
    
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    caughtHybrid.hybridGameObject.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * caughtHybrid.rotationSpeed);

                    // Move towards the target
                    Vector3 targetPosition = lureLocation.transform.position;

                    if (targetPosition.y > caughtHybrid.waterHeight)
                    {
                        targetPosition.y = caughtHybrid.waterHeight;
                    }

                    caughtHybrid.hybridGameObject.transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y - yOffset, targetPosition.z), Time.deltaTime * 100);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Continuously updates the behavior of hybrids that are reacting to the player, handling state transitions and movements based on the hybrid's current state.
    /// </summary>
    private IEnumerator UpdateHybridReact()
    {
        while (true)
        {
            // Only process if there are hybrids currently reacting to the player
            if (hybridReactToPlayer.Count > 0)
            {
                // Iterate backwards through the list to safely remove elements if needed
                for (int i = hybridReactToPlayer.Count - 1; i >= 0; i--)
                {
                    hybridNavData _hybrid = hybridReactToPlayer[i];

                    // Reset hybrids avoiding wall back to watching player ( This is a bug, Needs to be fixed, The Hybrid should not have a state of avoiding walls while in here I have no idea why it does.) 
                    if (_hybrid.HybridState == HybridState.HybridAvoidingWall)
                    {
                        _hybrid.HybridState = HybridState.HybridWatchPlayer;
                    }

                    // Handle behavior based on current state
                    switch (_hybrid.HybridState)
                    {
                        case HybridState.HybridWatchPlayer:
                            RotateHybridTowards(_hybrid, _PLAYER.playerHead.transform.position);
                            break;
                        case HybridState.HybridSwimToItem:
                            RotateAndMoveHybridTowards(_hybrid, _hybrid.itemTarget.transform.position);
                            break;
                        case HybridState.HybridLookAtHand:
                            RotateHybridTowards(_hybrid, _hybrid.itemTarget.transform.position);
                            break;
                        case HybridState.HybridSwimToPlayer:

                            TravelHybridTowardsPlayer(_hybrid);

                            break;
                    }
                }

                // Clear the list of hybrids to remove after processing
                foreach (var hybrid in removeHybridReact)
                {
                    hybridReactToPlayer.Remove(hybrid);
                }
                removeHybridReact.Clear();
            }

            // Wait until the next physics update
            yield return new WaitForFixedUpdate();
        }
    }

    void TravelHybridTowardsPlayer(hybridNavData _hybrid)
    {
        #region Rotate to look at player
        Vector3 targetPos = _PLAYER.transform.position;
        Vector3 direction = (targetPos - _hybrid.hybridGameObject.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _hybrid.hybridGameObject.transform.rotation = Quaternion.Lerp(_hybrid.hybridGameObject.transform.rotation, targetRotation, _hybrid.rotationSpeed * Time.deltaTime);
        #endregion

        #region Wall Collision Detections

        Ray hybridRay = new Ray(_hybrid.hybridGameObject.transform.position, _hybrid.hybridGameObject.transform.forward);
        float raycastDistance = 0.5f;
        int layerMask = ~LayerMask.GetMask("Fish");

        if (debug)
            Debug.DrawRay(hybridRay.origin, hybridRay.direction * raycastDistance, Color.red);

        if (Physics.Raycast(hybridRay, out RaycastHit hit, raycastDistance, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                _hybrid.aboutToHitWall = true;
            }
            else
            {
                _hybrid.aboutToHitWall = false;
            }
        }
        else
        {
            _hybrid.aboutToHitWall = false;
        }
        #endregion

        //travel if no collision detected
        if (!_hybrid.aboutToHitWall)
        {
            // Move towards the target
            _hybrid.hybridGameObject.transform.position = Vector3.Lerp(_hybrid.hybridGameObject.transform.position, _PLAYER.transform.position, Time.deltaTime * _hybrid.maxSpeed);
        }
    }

    //public void RemoveHybrid(GameObject go, bool _RemoveFromPond)

    /// <summary>
    /// Rotates the hybrid towards a target position.
    /// </summary>
    /// <param name="_hybrid">Hybrid to rotate.</param>
    /// <param name="_targetPos">Position to face.</param>
    private void RotateHybridTowards(hybridNavData _hybrid, Vector3 _targetPos)
    {
        Vector3 direction = (_targetPos - _hybrid.hybridGameObject.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _hybrid.hybridGameObject.transform.rotation = Quaternion.Lerp(_hybrid.hybridGameObject.transform.rotation, targetRotation, _hybrid.rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Rotates and moves the hybrid towards a target position.
    /// </summary>
    /// <param name="_hybrid">Hybrid to move.</param>
    /// <param name="_targetPos">Position to move to and face.</param>
    private void RotateAndMoveHybridTowards(hybridNavData _hybrid, Vector3 _targetPos)
    {
        RotateHybridTowards(_hybrid, _targetPos);

        // Clamp target position to water height
        if (_targetPos.y > _hybrid.waterHeight)
        {
            _targetPos.y = _hybrid.waterHeight;
        }

        // Move towards the target
        _hybrid.hybridGameObject.transform.position = Vector3.Lerp(_hybrid.hybridGameObject.transform.position, _targetPos, Time.deltaTime * _hybrid.maxSpeed);
    }

    public void RemoveHybrid(GameObject go, bool _RemoveFromPond)

    {
        foreach (hybridNavData _hybrid in hybridsInPond)
        {
            if (_hybrid.hybridGameObject == go)
            {
                switch (_hybrid.HybridState)
                {
                    case HybridState.HybridIdle:
                        removeHybridsIdle.Add(_hybrid);
                        break;
                    case HybridState.HybridFlying:
                        removeHybridsFlying.Add(_hybrid);
                        break;
                    case HybridState.HybridHitWater:
                        removeHybridsHitWater.Add(_hybrid);
                        break;
                    case HybridState.HybridFlocking or HybridState.HybridAvoidingWall:
                        removeHybridsSwimming.Add(_hybrid);
                        break;
                    case HybridState.HybridMiniGame_SwimToLure:
                        removeHybridSwimToLure.Add(_hybrid);
                        break;
                }
                if (_RemoveFromPond)
                    removeHybridsInPond.Add(_hybrid);
            }
        }
    }

    public void AddHybridTolist(GameObject go)
    {
        var _hybrid = GetHybridFromGO(go);  
        if (_hybrid.hybridGameObject == go)
        {
            print(_hybrid.HybridState);
            switch (_hybrid.HybridState)
            {
                case HybridState.HybridIdle:
                    hybridsIdle.Add(_hybrid);
                    break;
                case HybridState.HybridFlying:
                    hybridsFlying.Add(_hybrid);
                    break;
                case HybridState.HybridHitWater:
                    hybridsHitWater.Add(_hybrid);
                    break;
                case HybridState.HybridFlocking or HybridState.HybridAvoidingWall:
                    hybridsSwimming.Add(_hybrid);
                    break;
                case HybridState.HybridMiniGame_SwimToLure:
                    print("SWIM");
                    hybridSwimToPoint.Add(_hybrid);
                    break;
            }
        }
    }

    public void OnPickUp(FOEItem_Hybrid _hybridInfo)
    {
        _hybridInfo.SetVisuals(HybridVisualsState.Bubble);
        RemoveHybrid(_hybridInfo.gameObject, true);
    }

    public void OnDrop(FOEItem_Hybrid _hybridInfo)
    {
        _hybridInfo.SetVisuals(HybridVisualsState.World);

        //add a fuction to get the closest pond and its water hight
        addHybridToPondList(_hybridInfo.europaItemSO.worldObject.gameObject, HybridState.HybridFlying, 0, PondType.Error);
    }

    public void RemoveAllHybrids()
    {

    }

    /// <summary>
    /// update the hybrid state Using a gameObject
    /// </summary>
    /// <param name="_hybridGO"></param>
    /// <param name="_HybridState"></param>
    public void UpdateHybridState(GameObject _hybridGO, HybridState _HybridState)
    {
        hybridNavData _hybrid = GetHybridFromGO(_hybridGO);
        _hybrid.HybridState = _HybridState;
        print(_hybrid.HybridState);
    }

    /// <summary>
    /// Update the Hybrid State Using the HybridNavData
    /// </summary>
    /// <param name="_hybridGO"></param>
    /// <param name="_HybridState"></param>
    public void UpdateHybridState(hybridNavData _hybridGO, HybridState _HybridState)
    {
        _hybridGO.HybridState = _HybridState;
    }
    /// <summary>
    /// Check current HybridState of gameobject
    /// </summary>
    /// <param name="_hybridGO"></param>
    /// <param name="_HybridState"></param>
    public bool CheckHybridState(GameObject _hybridGO, HybridState _HybridState)
    {
        if (GetHybridFromGO(_hybridGO).HybridState == _HybridState) return true;
        else return false;
    }

    //dont call, called updateHybridState
    /// <summary>
    /// HelperFuction to find a hybrid NavData Using a GameObject
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    private hybridNavData GetHybridFromGO(GameObject go)
    {
        foreach (hybridNavData _hybrid in hybridsInPond)
        {
            if (_hybrid.hybridGameObject == go)
            {
                return _hybrid;
            }
        }
        return null;
    }

}



