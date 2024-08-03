using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using static FishSpawnerGeyser;

public enum HybridState
{
    HybridIdel,
    HybridFlying,
    HybridHitWater,
    HybridFlocking,
    HybridAvoidingWall,
    HybridSwimToLure,
    HybridWaitForMiniGame,
    HybridMiniGame_Pulling,
    HybridMiniGame_Tired,
    HybridOnLand_Sitting,
    HybridOnLand_Walking,

}

public class FishNavigationManager : MonoBehaviour
{
    [Header("All Hybrids In Pond")] // a list that contains all the hybrids that has been spawnned into this pond. 
    public List<hybridNavData> hybridsInPond = new List<hybridNavData>();
    List<hybridNavData> removeHybridsInPond = new List<hybridNavData>();
    [SerializeField] float waterHeight = 0;

    [Header("Hybrids Doing Diffrent Tasks")] // Lists for each of the hybrids doing diffrent things so we dont have to loop over them all. 
    [SerializeField] List<hybridNavData> hybridsIdel = new List<hybridNavData>();
    [SerializeField] List<hybridNavData> hybridsFlying = new List<hybridNavData>();
    [SerializeField] List<hybridNavData> hybridsHitWater = new List<hybridNavData>();
    [SerializeField] List<hybridNavData> hybridsSwimming = new List<hybridNavData>();
    [SerializeField] List<hybridNavData> hybridSwimToLure = new List<hybridNavData>();
    [SerializeField] hybridNavData caughtHybrid;

    List<hybridNavData> removeHybridsIdel = new List<hybridNavData>();
    List<hybridNavData> removeHybridsFlying = new List<hybridNavData>();
    List<hybridNavData> removeHybridsHitWater = new List<hybridNavData>();
    List<hybridNavData> removeHybridsSwimming = new List<hybridNavData>();
    List<hybridNavData> removeHybridSwimToLure = new List<hybridNavData>();

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

    [SerializeField] float correctRotationSpeed = 0.1f; //being used by UpdateHitWater()

    private Coroutine updateHybridFlyingCoroutine;
    private Coroutine updateHitWaterCoroutine;
    private Coroutine updateSwimmingCoroutine;
    private Coroutine updateSwimToLure;

    private GameObject lureLocation;

    private float waterDrag = 5.0f;
    private float airDrag = 0.0f;

    [Serializable]
    public class hybridNavData
    {
        public GameObject hybridGameObject;
        public float minSpeed;
        public float maxSpeed;
        public float rotationSpeed;
        public HybridState HybridState;
        public Rigidbody hybridRigidbody;
        public bool isTurning;

        public Vector3 velocity;
        public bool aboutToHitWall = false;

        public bool arrivedAtLure = false;
        public bool firstNav;

    }

    private void Start()
    {
        //waterHight = GetComponentInParent<GeyserMannager>().waterHight.transform.position.y;
        updateHybridFlyingCoroutine = StartCoroutine(UpdateHybridFlying());
        updateHitWaterCoroutine = StartCoroutine(UpdateHitWater());
        updateSwimmingCoroutine = StartCoroutine(UpdateSwimming());
        updateSwimToLure = StartCoroutine(UpdateHybridToLure());
    }

    public void addHybridToPondList(GameObject go, HybridState enterState)
    {
        hybridNavData newEntry = new hybridNavData();
        newEntry.hybridGameObject = go;
        HybridInfo hybridInfo = go.GetComponent<HybridInfo>();
        if (hybridInfo == null)
        {
            Debug.LogError($"{go} dose not have Hybrid Info attached");
        }

        newEntry.minSpeed = hybridInfo.hybridInfo.fishSpeed;
        newEntry.maxSpeed = hybridInfo.hybridInfo.fishSpeed * 2;
        newEntry.HybridState = HybridState.HybridFlying;
        newEntry.hybridRigidbody = go.GetComponent<Rigidbody>();
        newEntry.velocity = Vector3.forward * hybridInfo.hybridInfo.fishSpeed;
        newEntry.rotationSpeed = hybridInfo.hybridInfo.rotationSpeed;
        newEntry.firstNav = true;

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

    private IEnumerator UpdateIdel()
    {
        while (true)
        {
            if (hybridsIdel.Count > 0)
            {
                foreach (hybridNavData _Hybrid in hybridsIdel)
                {

                }
                // Remove hybrids from hybridsHitWater
                foreach (var hybrid in removeHybridsIdel)
                {
                    hybridsIdel.Remove(hybrid);
                }
                   
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
                    hybridNavData _hybridd = hybridsFlying[i];

                    // Animate the Hybrids

                    // Check if Hybrid has left the water
                    if (_hybridd.HybridState != HybridState.HybridFlying)
                    {
                        _hybridd.hybridRigidbody.useGravity = true;
                        _hybridd.hybridRigidbody.drag = airDrag;
                        _hybridd.hybridRigidbody.angularDrag = airDrag;
                        _hybridd.HybridState = HybridState.HybridFlying;
                    }

                    //print($"{_hybridd} has a y value of {_hybridd.hybridGameObject.transform.position.y} and the water hight is {waterHight}.y ");
                    // Check if the Hybrids have hit the water
                    if (_hybridd.hybridGameObject.transform.position.y <= waterHeight)
                    {
                        hybridsHitWater.Add(_hybridd);
                        //hybridsFlying.RemoveAt(i);
                        removeHybridsFlying.Add(_hybridd);
                    }
                }

                // Remove hybrids from hybridsHitWater
                foreach (var hybrid in removeHybridsFlying)
                {
                    hybridsFlying.Remove(hybrid);
                }
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
                    hybridNavData _hybridd = hybridsHitWater[i];

                    if (_hybridd.HybridState != HybridState.HybridHitWater)
                    {
                        _hybridd.hybridRigidbody.useGravity = false;
                        _hybridd.hybridRigidbody.drag = waterDrag;
                        _hybridd.hybridRigidbody.angularDrag = waterDrag;
                        _hybridd.HybridState = HybridState.HybridHitWater;
                    }

                    // Define the target rotation
                    Quaternion targetRotation = Quaternion.Euler(0, _hybridd.hybridGameObject.transform.rotation.eulerAngles.y, 0);

                    // Lerp towards the target rotation
                    _hybridd.hybridGameObject.transform.rotation = Quaternion.Lerp(_hybridd.hybridGameObject.transform.rotation, targetRotation, Time.deltaTime * correctRotationSpeed);

                    // Convert rotation to Euler angles
                    Vector3 currentRotation = _hybridd.hybridGameObject.transform.rotation.eulerAngles;

                    // Check if the rotation is within the desired range
                    if ((currentRotation.x < 0.5f || currentRotation.x > 359.5f) &&
                        (currentRotation.z < 0.5f || currentRotation.z > 359.5f))
                    {
                        //print($"{_hybridd} is within the range to make swim");
                        hybridsSwimming.Add(_hybridd);
                        removeHybridsHitWater.Add(_hybridd);
                        _hybridd.velocity = _hybridd.hybridGameObject.transform.position;
                    }
                }

                // Remove hybrids from hybridsHitWater
                foreach (var hybrid in removeHybridsHitWater)
                {
                    hybridsHitWater.Remove(hybrid);
                }
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
                foreach (hybridNavData _Hybrid in hybridsSwimming)
                {
                    bool outofWater = false;

                    if (_Hybrid.hybridGameObject.transform.position.y > waterHeight)
                    {
                        outofWater = true;
                    }

                    if (UnityEngine.Random.Range(0, rayCastCheckChance) < 1 && hybridsSwimming.Count > 1)
                    {
                        Ray hybridRay = new Ray(_Hybrid.hybridGameObject.transform.position, _Hybrid.hybridGameObject.transform.forward);
                        float raycastDistance = 0.5f;
                        int layerMask = ~LayerMask.GetMask("Fish");

                        if (debug)
                            Debug.DrawRay(hybridRay.origin, hybridRay.direction * raycastDistance, Color.red);

                        if (Physics.Raycast(hybridRay, out RaycastHit hit, raycastDistance, layerMask))
                        {
                            if (hit.collider.gameObject.CompareTag("Wall"))
                            {
                                _Hybrid.aboutToHitWall = true;
                            }
                            else
                            {
                                _Hybrid.aboutToHitWall = false;
                                _Hybrid.HybridState = HybridState.HybridFlocking;
                            }
                        }
                        else
                        {
                            _Hybrid.aboutToHitWall = false;
                            _Hybrid.HybridState = HybridState.HybridFlocking;
                        }
                    }

                    if (UnityEngine.Random.Range(0, applyBoidsChance) < 1 && hybridsSwimming.Count > 1 || _Hybrid.firstNav == true)
                    {
                        _Hybrid.firstNav = false;
                        Vector3 separationVelocity = Vector3.zero;
                        Vector3 alignmentVelocity = Vector3.zero;
                        Vector3 cohesionVelocity = Vector3.zero;

                        int numOfBoidsToAvoid = 0;
                        int numOfBoidsToAlignWith = 0;
                        int numOfBoidsInFlock = 0;
                        Vector3 currBoidPosition = _Hybrid.hybridGameObject.transform.position;
                        Vector3 positionToMoveTowards = Vector3.zero;

                        foreach (hybridNavData _otherBoid in hybridsSwimming)
                        {
                            if (ReferenceEquals(_otherBoid, _Hybrid))
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

                        _Hybrid.velocity += separationVelocity;
                        _Hybrid.velocity += alignmentVelocity;
                        _Hybrid.velocity += cohesionVelocity;

                        _Hybrid.velocity = Vector3.ClampMagnitude(_Hybrid.velocity, _Hybrid.maxSpeed);

                        Vector3 direction = _Hybrid.velocity.normalized;
                        float speed = _Hybrid.velocity.magnitude;
                        speed = Mathf.Clamp(speed, _Hybrid.minSpeed, _Hybrid.maxSpeed);
                        _Hybrid.velocity = direction * speed;
                    }

                    if (outofWater)
                    {
                        _Hybrid.velocity.y = -Mathf.Abs(_Hybrid.velocity.y);
                    }

                    if (_Hybrid.aboutToHitWall == true && _Hybrid.HybridState != HybridState.HybridAvoidingWall)
                    {
                        Vector3 oppositeDirection = -_Hybrid.hybridGameObject.transform.forward;
                        _Hybrid.velocity = oppositeDirection * _Hybrid.maxSpeed;
                        _Hybrid.HybridState = HybridState.HybridAvoidingWall;
                    }

                    // Move the Hybrid in the direction of Velocity
                    _Hybrid.hybridGameObject.transform.position += _Hybrid.velocity * Time.deltaTime;

                    // Rotate the Hybrid toward the direction it is moving
                    Quaternion targetRotation = Quaternion.LookRotation(_Hybrid.velocity);
                    Debug.Log($"Updating Rotation: {_Hybrid.hybridGameObject.name} Current Rotation: {_Hybrid.hybridGameObject.transform.rotation} Target Rotation: {targetRotation}");
                    _Hybrid.hybridGameObject.transform.rotation = Quaternion.Lerp(_Hybrid.hybridGameObject.transform.rotation, targetRotation, _Hybrid.rotationSpeed * Time.deltaTime);

                }

                // Remove hybrids from hybridsHitWater
                foreach (var hybrid in removeHybridsSwimming)
                {
                    hybridsSwimming.Remove(hybrid);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator UpdateHybridToLure()
    {
        while (true)
        {
            if (hybridSwimToLure.Count > 0)
            {
                foreach (hybridNavData _hybrid in hybridSwimToLure)
                {
                    Vector3 directionToTarget = lureLocation.transform.position - _hybrid.hybridGameObject.transform.position;
                    float yOffset = 0.5f;
                    directionToTarget = new Vector3(directionToTarget.x, directionToTarget.y - yOffset, directionToTarget.z);

                    // Rotate towards the target
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _hybrid.rotationSpeed);

                    // Move towards the target
                    Vector3 targetPosition = lureLocation.transform.position;

                    if (targetPosition.y > waterHeight)
                    {
                        targetPosition.y = waterHeight;
                    }

                    transform.position = Vector3.Lerp(_hybrid.hybridGameObject.transform.position, new Vector3(targetPosition.x, targetPosition.y - yOffset, targetPosition.z), Time.deltaTime * (_hybrid.maxSpeed * 2));

                    // Check if the object has reached the target position
                    float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
                    float threshold = 1f; // Adjust the threshold as needed

                    if (distanceToTarget < threshold && _hybrid.arrivedAtLure == false)
                    {
                        //Debug.Log("Object has reached the target!");
                        _hybrid.HybridState = HybridState.HybridIdel;
                        _hybrid.arrivedAtLure = true;
                        hybridsIdel.Add(_hybrid);
                        removeHybridSwimToLure.Add(_hybrid);

                        //send an update to minigame Mannager that the Hybrid has arrived
                    }

                }
                foreach (var hybrid in removeHybridSwimToLure)
                {
                    hybridSwimToLure.Remove(hybrid);
                }
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

                    if (targetPosition.y > waterHeight)
                    {
                        targetPosition.y = waterHeight;
                    }

                    transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y - yOffset, targetPosition.z), Time.deltaTime * 100);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void removeHybrid(GameObject go)
    {
        foreach (hybridNavData _hybrid in hybridsInPond)
        {
            if (_hybrid.hybridGameObject == go)
            {
                switch (_hybrid.HybridState)
                {
                    case HybridState.HybridIdel:
                        removeHybridsIdel.Add(_hybrid);
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
                    case HybridState.HybridSwimToLure:
                        removeHybridSwimToLure.Add(_hybrid);
                        break;
                }
                hybridsInPond.Remove(_hybrid);
            }
        }
    }



}


