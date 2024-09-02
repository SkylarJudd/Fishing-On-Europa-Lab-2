using System;
using System.Collections;
using System.Collections.Generic;
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


    private void Start()
    {
        //TESTING ONLY REMOVE LATER!!!!!!!!!
        lureLocation = _FMGM.bobberGameObject;

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

    public void addHybridToPondList(GameObject go, HybridState enterState , float _waterHight, PondType _pondType)
    {
        hybridNavData newEntry = new hybridNavData();
        newEntry.hybridGameObject = go;
        FOEItem_Hybrid hybridInfo = go.GetComponent<FOEItem_Hybrid>();
        if (hybridInfo == null)
        {
            Debug.LogError($"{go} dose not have Hybrid Info attached");
        }

        hybridInfo.SetVisuals(HybridVisualsState.World);

        newEntry.minSpeed = hybridInfo.hybridSO.fishSpeed;
        newEntry.maxSpeed = hybridInfo.hybridSO.fishSpeed * 2;
        newEntry.HybridState = HybridState.HybridFlying;
        newEntry.hybridRigidbody = go.GetComponent<Rigidbody>();
        newEntry.velocity = Vector3.forward * hybridInfo.hybridSO.fishSpeed;
        newEntry.rotationSpeed = hybridInfo.hybridSO.rotationSpeed;
        newEntry.firstNav = true;
        newEntry.foodEaten = hybridInfo.hybridSO.foodEaten;
        newEntry.waterHeight = _waterHight;
        newEntry.pondType = _pondType;

        if (newEntry.hybridRigidbody == null)
        {
            Debug.LogWarning($"{go} Dose not have a rigid body, so one has been assinged");
            newEntry.hybridGameObject.AddComponent<Rigidbody>();
        }

        hybridsInPond.Add(newEntry);

        switch (enterState)
        {
            case HybridState.HybridFlying:
                hybridsFlying.Add(newEntry);
                break;
            case HybridState.HybridFlocking:
                hybridsSwimming.Add(newEntry);
                break;
            default:
                hybridsSwimming.Add(newEntry);
                break;
        }
    }

    private IEnumerator UpdateHybridStates()
    {
        while (true)
        {
            if (hybridsInPond.Count > 0)
            {
                foreach (hybridNavData _hybrid in hybridsInPond)
                {
                    _hybrid.distanceToPlayer = Vector3.Distance(_PLAYER.player.transform.position, _hybrid.hybridGameObject.transform.position);
                    if (_hybrid.distanceToPlayer < playerReactionDistance)
                    {
                        if (hybridReactToPlayer.Contains(_hybrid) == false)
                        {
                            removeHybrid(_hybrid.hybridGameObject, false);
                            hybridReactToPlayer.Add(_hybrid);
                        }

                        float distanceToRightHand = 0;
                        float distanceToLeftHand = 0;

                        if (_PLAYER.hasItem == false)
                        {
                            distanceToRightHand = Vector3.Distance(_hybrid.hybridGameObject.transform.position, _PLAYER.rightHand.transform.position);
                            distanceToLeftHand = Vector3.Distance(_hybrid.hybridGameObject.transform.position, _PLAYER.leftHand.transform.position);
                        }


                        if (_PLAYER.hasItem == true && (_PLAYER.rightHandFood != null || _PLAYER.leftHandFood != null))
                        {
                            FOEItem_Food _rightFood = _PLAYER.rightHandFood;
                            FOEItem_Food _leftFood = _PLAYER.rightHandFood;

                            foreach (FoodType _food in _hybrid.foodEaten)
                            {
                                if (_food == _leftFood.foodType)
                                {
                                    _hybrid.itemTarget = _PLAYER.leftHandFood.gameObject.transform;
                                    _hybrid.HybridState = HybridState.HybridSwimToItem;
                                    if (_hybrid.distanceToPlayer < moveToPlayerStoppingDistance)
                                    {
                                        _hybrid.HybridState = HybridState.HybridLookAtHand;
                                    }
                                }
                                else if (_food == _rightFood.foodType)
                                {
                                    _hybrid.itemTarget = _PLAYER.rightHandFood.gameObject.transform;
                                    _hybrid.HybridState = HybridState.HybridSwimToItem;
                                    if (_hybrid.distanceToPlayer < moveToPlayerStoppingDistance)
                                    {
                                        _hybrid.HybridState = HybridState.HybridLookAtHand;
                                    }
                                }
                            }
                        }
                        else if (_PLAYER.hasItem == true && (_PLAYER.rightHandPlushie != null || _PLAYER.leftHandPlushie != null))
                        {
                            if (_hybrid.favToy == _PLAYER.leftHandPlushie.ToyItem)
                            {
                                _hybrid.itemTarget = _PLAYER.leftHandPlushie.gameObject.transform;
                                _hybrid.HybridState = HybridState.HybridSwimToItem;
                                if (_hybrid.distanceToPlayer < moveToPlayerStoppingDistance)
                                {
                                    _hybrid.HybridState = HybridState.HybridLookAtHand;
                                }
                            }
                            else if (_hybrid.favToy == _PLAYER.rightHandPlushie.ToyItem)
                            {
                                _hybrid.itemTarget = _PLAYER.rightHandPlushie.gameObject.transform;
                                _hybrid.HybridState = HybridState.HybridSwimToItem;
                                if (_hybrid.distanceToPlayer < moveToPlayerStoppingDistance)
                                {
                                    _hybrid.HybridState = HybridState.HybridLookAtHand;
                                }
                            }
                        }
                        else if (_PLAYER.hasItem == false && (distanceToRightHand < playerHandReactionDistance || distanceToLeftHand < playerHandReactionDistance))
                        {

                            // Compare distances and set the target to the closer hand
                            if (distanceToRightHand < distanceToLeftHand)
                            {
                                _hybrid.itemTarget = _PLAYER.rightHand;
                            }
                            else
                            {
                                _hybrid.itemTarget = _PLAYER.leftHand;
                            }

                            // Set the Hybrid's state to look at the selected hand
                            _hybrid.HybridState = HybridState.HybridLookAtHand;
                        }
                        else
                        {

                            _hybrid.HybridState = HybridState.HybridWatchPlayer;
                        }
                    }
                    else if (_hybrid.distanceToPlayer > playerReactionDistanceReset && _hybrid.HybridState == HybridState.HybridWatchPlayer)
                    {
                        _hybrid.HybridState = HybridState.HybridFlocking;

                        removeHybridReact.Add(_hybrid);
                        hybridsSwimming.Add(_hybrid);
                    }

                }
                // Remove hybrids from hybridsHitWater
                foreach (var _hybrid in removeHybridReact)
                {
                    hybridReactToPlayer.Remove(_hybrid);
                }
                removeHybridReact.Clear();
            }
            yield return new WaitForFixedUpdate();
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
                        _hybrid.hybridRigidbody.useGravity = true;
                        _hybrid.hybridRigidbody.drag = airDrag;
                        _hybrid.hybridRigidbody.angularDrag = airDrag;
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
    /// Loops though the list of hybrids that have hit the water and updates their rigid bodies and corrects their rotation
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateHitWater()
    {
        while (true)
        {
            if (hybridsHitWater.Count > 0)
            {


                for (int i = hybridsHitWater.Count - 1; i >= 0; i--)
                {
                    hybridNavData _hybrid = hybridsHitWater[i];

                    if (_hybrid.HybridState != HybridState.HybridHitWater)
                    {
                        _hybrid.hybridRigidbody.useGravity = false;
                        _hybrid.hybridRigidbody.drag = waterDrag;
                        _hybrid.hybridRigidbody.angularDrag = waterDrag;
                        _hybrid.HybridState = HybridState.HybridHitWater;
                    }

                    // Define the target rotation
                    Quaternion targetRotation = Quaternion.Euler(0, _hybrid.hybridGameObject.transform.rotation.eulerAngles.y, 0);

                    // Lerp towards the target rotation
                    _hybrid.hybridGameObject.transform.rotation = Quaternion.Lerp(_hybrid.hybridGameObject.transform.rotation, targetRotation, Time.deltaTime * correctRotationSpeed);

                    // Convert rotation to Euler angles
                    Vector3 currentRotation = _hybrid.hybridGameObject.transform.rotation.eulerAngles;

                    // Check if the rotation is within the desired range
                    if ((currentRotation.x < 0.5f || currentRotation.x > 359.5f) &&
                        (currentRotation.z < 0.5f || currentRotation.z > 359.5f))
                    {
                        //print($"{_hybridd} is within the range to make swim");
                        hybridsSwimming.Add(_hybrid);
                        removeHybridsHitWater.Add(_hybrid);
                        _hybrid.velocity = _hybrid.hybridGameObject.transform.position;
                    }
                }

                // Remove hybrids from hybridsHitWater
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
                    print("swim part 2" + lureLocation.transform.position);
                    Vector3 directionToTarget = lureLocation.transform.position - _hybrid.hybridGameObject.transform.position;
                    float yOffset = 0.5f;
                    directionToTarget = new Vector3(directionToTarget.x, directionToTarget.y - yOffset, directionToTarget.z);

                    // Rotate towards the target
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _hybrid.rotationSpeed);

                    // Move towards the target
                    Vector3 targetPosition = lureLocation.transform.position;

                    if (targetPosition.y > _hybrid.waterHeight)
                    {
                        targetPosition.y = _hybrid.waterHeight;
                    }
                    print(_hybrid.hybridGameObject.transform.position);
                    _hybrid.hybridGameObject.transform.position = Vector3.Lerp(_hybrid.hybridGameObject.transform.position, new Vector3(targetPosition.x, targetPosition.y - yOffset, targetPosition.z), Time.deltaTime * (_hybrid.maxSpeed * 2));

                    // Check if the object has reached the target position
                    float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
                    float threshold = 1f; // Adjust the threshold as needed

                    if (distanceToTarget < threshold && _hybrid.arrivedAtLure == false)
                    {
                        Debug.Log("Object has reached the target!");
                        _hybrid.HybridState = HybridState.HybridIdle;
                        _hybrid.arrivedAtLure = true;
                        hybridsIdle.Add(_hybrid);
                        removeHybridSwimToLure.Add(_hybrid);

                        //send an update to minigame Manager that the Hybrid has arrived
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

                }
                else if (caughtHybrid.HybridState == HybridState.HybridMiniGame_Tired)
                {

                    //print("Attached to line");
                    Vector3 directionToTarget = lureLocation.transform.position - transform.position;
                    float yOffset = 0.5f;
                    directionToTarget = new Vector3(directionToTarget.x, directionToTarget.y - yOffset, directionToTarget.z);
                    // Rotate towards the target

                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * caughtHybrid.rotationSpeed);

                    // Move towards the target
                    Vector3 targetPosition = lureLocation.transform.position;

                    if (targetPosition.y > caughtHybrid.waterHeight)
                    {
                        targetPosition.y = caughtHybrid.waterHeight;
                    }

                    transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y - yOffset, targetPosition.z), Time.deltaTime * 100);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator UpdateHybridReact()
    {
        while (true)
        {
            if (hybridReactToPlayer.Count > 0)
            {


                for (int i = hybridReactToPlayer.Count - 1; i >= 0; i--)
                {
                    hybridNavData _hybrid = hybridReactToPlayer[i];
                    if (_hybrid.HybridState == HybridState.HybridAvoidingWall)
                    {
                        _hybrid.HybridState = HybridState.HybridWatchPlayer;
                    }

                    switch (_hybrid.HybridState)
                    {
                        case HybridState.HybridWatchPlayer:
                            // Rotate the Hybrid toward the Player
                            Quaternion targetRotation = Quaternion.LookRotation((_PLAYER.playerHead.transform.position - _hybrid.hybridGameObject.transform.position).normalized);
                            _hybrid.hybridGameObject.transform.rotation = Quaternion.Lerp(_hybrid.hybridGameObject.transform.rotation, targetRotation, _hybrid.rotationSpeed * Time.deltaTime);
                            break;
                        case HybridState.HybridSwimToItem:

                            Quaternion targetRotation2 = Quaternion.LookRotation((_hybrid.itemTarget.transform.position - _hybrid.hybridGameObject.transform.position).normalized);
                            _hybrid.hybridGameObject.transform.rotation = Quaternion.Lerp(_hybrid.hybridGameObject.transform.rotation, targetRotation2, _hybrid.rotationSpeed * Time.deltaTime);

                            // Move towards the target
                            Vector3 targetPosition = _hybrid.itemTarget.transform.position;

                            if (targetPosition.y > _hybrid.waterHeight)
                            {
                                targetPosition.y = _hybrid.waterHeight;
                            }

                            transform.position = Vector3.Lerp(_hybrid.hybridGameObject.transform.position, targetPosition, Time.deltaTime * _hybrid.maxSpeed);
                            break;
                        case HybridState.HybridLookAtHand:
                            // Rotate the Hybrid toward the Player
                            Quaternion targetRotation3 = Quaternion.LookRotation((_hybrid.itemTarget.transform.position - _hybrid.hybridGameObject.transform.position).normalized);
                            _hybrid.hybridGameObject.transform.rotation = Quaternion.Lerp(_hybrid.hybridGameObject.transform.rotation, targetRotation3, _hybrid.rotationSpeed * Time.deltaTime);
                            break;
                    }


                }

                // Remove hybrids from hybridsHitWater
                foreach (var hybrid in removeHybridReact)
                {
                    hybridReactToPlayer.Remove(hybrid);
                }
                removeHybridReact.Clear();
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void removeHybrid(GameObject go, bool _RemoveFromPond)
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
        removeHybrid(_hybridInfo.gameObject, true);
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


