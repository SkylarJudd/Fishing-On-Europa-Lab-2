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

}

public class FishNavigationManager : MonoBehaviour
{
    public List<hybridNavData> hybridsInPond = new List<hybridNavData>();

    [SerializeField] List<hybridNavData> hybridsIdel = new List<hybridNavData>();
    [SerializeField] List<hybridNavData> hybridsFlying = new List<hybridNavData>();
    [SerializeField] List<hybridNavData> hybridsHitWater = new List<hybridNavData>();
    [SerializeField] List<hybridNavData> hybridsSwimming = new List<hybridNavData>();

    [SerializeField] float speed = 0.1f;
    [SerializeField] float neighbourDistance = 4.0f; //lower number = less likely to flock
    [SerializeField] float rotationSpeed = 45f;
    [SerializeField] float correctRotationSpeed = 0.1f;

    [SerializeField] float waterHeight = 0;

    [SerializeField] GameObject turnTarget;

    [SerializeField] BoidSettings boidSettings = new BoidSettings(1.5f, 3f, 2f, 1f, 1f, 1f, 1f, 0.5f, 5f, 10);


    private Coroutine updateHybridFlyingCoroutine;
    private Coroutine updateHitWaterCoroutine;
    private Coroutine updateSwimmingCoroutine;

    private float waterDrag = 5.0f;
    private float airDrag = 0.0f;

    [Serializable]
    public class hybridNavData
    {
        public GameObject hybridGameObject;
        public float baseSpeed;
        public float currentSpeed;
        public HybridState HybridState;
        public Rigidbody hybridRigidbody;
        public bool isTurning;

        public Vector3 velocity;
        public bool aboutToHitWall = false;

    }

    [Serializable]
    public struct BoidSettings
    {

        // boid behavior range
        public float separationRange;
        public float alignmentRange;
        public float cohesionRange;

        // boid behavior weights
        public float separationFactor;
        public float alignmentFactor;
        public float cohesionFactor;

        // misc settings
        public float boidScale;
        public float minSpeed;
        public float maxSpeed;
        public int rotationSpeed;

        // Constructor to set default values
        public BoidSettings(float separationRange, float alignmentRange, float cohesionRange,
                            float separationFactor, float alignmentFactor, float cohesionFactor,
                            float boidScale, float minSpeed, float maxSpeed, int rotationSpeed)
        {
            this.separationRange = separationRange;
            this.alignmentRange = alignmentRange;
            this.cohesionRange = cohesionRange;
            this.separationFactor = separationFactor;
            this.alignmentFactor = alignmentFactor;
            this.cohesionFactor = cohesionFactor;
            this.boidScale = boidScale;
            this.minSpeed = minSpeed;
            this.maxSpeed = maxSpeed;
            this.rotationSpeed = rotationSpeed;
        }
    }

    private void Start()
    {
        // If values are being set here, ensure they are correct
        boidSettings = new BoidSettings(1.5f, 3f, 2f, 1f, 1f, 1f, 1f, 0.5f, 5f, 10);

        //waterHight = GetComponentInParent<GeyserMannager>().waterHight.transform.position.y;
        updateHybridFlyingCoroutine = StartCoroutine(UpdateHybridFlying());
        updateHitWaterCoroutine = StartCoroutine(UpdateHitWater());
        updateSwimmingCoroutine = StartCoroutine(UpdateSwimming());



    }

    public void addHybridToPondList(GameObject go)
    {
        hybridNavData newEntry = new hybridNavData();
        newEntry.hybridGameObject = go;
        HybridInfo hybridInfo = go.GetComponent<HybridInfo>();
        if (hybridInfo == null)
        {
            Debug.LogError($"{go} dose not have Hybrid Info attached");
        }

        newEntry.baseSpeed = hybridInfo.hybridInfo.fishSpeed;
        newEntry.currentSpeed = 0f;
        newEntry.HybridState = HybridState.HybridFlying;
        newEntry.hybridRigidbody = go.GetComponent<Rigidbody>();
        newEntry.velocity = Vector3.forward * newEntry.baseSpeed; 

        if (newEntry.hybridRigidbody == null)
        {
            Debug.LogWarning($"{go} Dose not have a rigid body, so one has been assinged");
            newEntry.hybridGameObject.AddComponent<Rigidbody>();
        }

        hybridsInPond.Add(newEntry);

        hybridsFlying.Add(newEntry);

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
                        hybridsFlying.RemoveAt(i);
                    }
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
                // Create a temporary list to hold hybrids to be removed
                List<hybridNavData> hybridsToRemove = new List<hybridNavData>();

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
                        hybridsToRemove.Add(_hybridd);
                    }
                }

                // Remove hybrids from hybridsHitWater
                foreach (var hybrid in hybridsToRemove)
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

                    if (UnityEngine.Random.Range(0, 20) < 1 && hybridsSwimming.Count > 1)
                    {
                        Ray hybridRay = new Ray(_Hybrid.hybridGameObject.transform.position, _Hybrid.hybridGameObject.transform.forward);
                        float raycastDistance = 2f;
                        int layerMask = ~LayerMask.GetMask("Fish");

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

                    if (UnityEngine.Random.Range(0, 20) < 1 && hybridsSwimming.Count > 1)
                    {
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
                            if (dist < boidSettings.separationRange)
                            {
                                Vector3 otherBoidToCurrentBoid = currBoidPosition - otherBoidsPosition;
                                Vector3 dirToTravel = otherBoidToCurrentBoid.normalized;
                                separationVelocity += dirToTravel / dist;
                                numOfBoidsToAvoid++;
                            }

                            // Alignment Check
                            if (dist < boidSettings.alignmentRange)
                            {
                                alignmentVelocity += _otherBoid.velocity;
                                numOfBoidsToAlignWith++;
                            }

                            // Cohesion Check
                            if (dist < boidSettings.cohesionRange)
                            {
                                positionToMoveTowards += otherBoidsPosition;
                                numOfBoidsInFlock++;
                            }
                        }

                        if (numOfBoidsToAvoid != 0)
                        {
                            separationVelocity /= numOfBoidsToAvoid;
                            separationVelocity.Normalize();
                            separationVelocity *= boidSettings.separationFactor;
                        }

                        if (numOfBoidsToAlignWith != 0)
                        {
                            alignmentVelocity /= numOfBoidsToAlignWith;
                            alignmentVelocity.Normalize();
                            alignmentVelocity *= boidSettings.alignmentFactor;
                        }

                        if (numOfBoidsInFlock != 0)
                        {
                            positionToMoveTowards /= numOfBoidsInFlock;
                            Vector3 cohesionDirection = positionToMoveTowards - currBoidPosition;
                            cohesionDirection.Normalize();
                            cohesionVelocity = cohesionDirection * boidSettings.cohesionFactor;
                        }

                        _Hybrid.velocity += separationVelocity;
                        _Hybrid.velocity += alignmentVelocity;
                        _Hybrid.velocity += cohesionVelocity;

                        _Hybrid.velocity = Vector3.ClampMagnitude(_Hybrid.velocity, boidSettings.maxSpeed);

                        Vector3 direction = _Hybrid.velocity.normalized;
                        float speed = _Hybrid.velocity.magnitude;
                        speed = Mathf.Clamp(speed, boidSettings.minSpeed, boidSettings.maxSpeed);
                        _Hybrid.velocity = direction * speed;
                    }

                    if (outofWater)
                    {
                        _Hybrid.velocity.y = -Mathf.Abs(_Hybrid.velocity.y);
                    }

                    if (_Hybrid.aboutToHitWall == true && _Hybrid.HybridState != HybridState.HybridAvoidingWall)
                    {
                        Vector3 oppositeDirection = -_Hybrid.hybridGameObject.transform.forward;
                        _Hybrid.velocity = oppositeDirection * boidSettings.maxSpeed;
                        _Hybrid.HybridState = HybridState.HybridAvoidingWall;
                    }

                    // Move the Hybrid in the direction of Velocity
                    _Hybrid.hybridGameObject.transform.position += _Hybrid.velocity * Time.deltaTime;

                    // Rotate the Hybrid toward the direction it is moving
                    Quaternion targetRotation = Quaternion.LookRotation(_Hybrid.velocity);
                    Debug.Log($"Updating Rotation: {_Hybrid.hybridGameObject.name} Current Rotation: {_Hybrid.hybridGameObject.transform.rotation} Target Rotation: {targetRotation}");
                    _Hybrid.hybridGameObject.transform.rotation = Quaternion.Lerp(_Hybrid.hybridGameObject.transform.rotation, targetRotation, boidSettings.rotationSpeed * Time.deltaTime);

                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}


