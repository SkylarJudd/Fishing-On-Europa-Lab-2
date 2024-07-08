using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] float waterHight = 0;

    [SerializeField] GameObject turnTarget;

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
    }

    private void Start()
    {
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

        if (newEntry.hybridRigidbody == null)
        {
            Debug.LogWarning($"{go} Dose not have a rigid body, so one has been assinged");
            newEntry.hybridGameObject.AddComponent<Rigidbody>();
        }

        hybridsInPond.Add(newEntry);

        hybridsFlying.Add(newEntry);

    }

    private void Update()
    {

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
                    if (_hybridd.hybridGameObject.transform.position.y <= waterHight)
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

                    CheckIfOutOfWater(_Hybrid);
                    CheckIfWall(_Hybrid);

                    //add function to set z back to 0 over time

                    if (UnityEngine.Random.Range(0, 10) < 1)
                    {
                        //Update boids system 
                        ApplyFlockRules(_Hybrid);
                    }

                    if (_Hybrid.isTurning == false)
                    {
                        MoveHybrid(_Hybrid);
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
    /// <summary>
    /// Moves the Hybrid Forward 
    /// </summary>
    /// <param name="_Hybrid"></param>
    private void MoveHybrid(hybridNavData _Hybrid)
    {
        _Hybrid.hybridGameObject.transform.Translate(0, 0, Time.deltaTime * speed);
    }
    /// <summary>
    /// Turns the Inputted Hybrid to the inputted Direction by the inputted Rotation speed. 
    /// </summary>
    /// <param name="_Hybrid"></param>
    /// <param name="turnDirection"></param>
    /// <param name="rotationSpeed"></param>
    /// <returns></returns>
    private IEnumerator turnHybrid(hybridNavData _Hybrid, Vector3 turnDirection, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(turnDirection);
        _Hybrid.isTurning = true;
        while (true)
        {
            _Hybrid.hybridGameObject.transform.rotation = Quaternion.Slerp(_Hybrid.hybridGameObject.transform.rotation,targetRotation,rotationSpeed * Time.deltaTime);

            // Check if the rotation is close enough to the target rotation
            if (Quaternion.Angle(_Hybrid.hybridGameObject.transform.rotation, targetRotation) < 5f)
            {
                _Hybrid.isTurning = false; 
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
    /// <summary>
    /// Checks if the inputted Hybrid is above the water Hight
    /// </summary>
    /// <param name="_Hybrid"></param>
    private void CheckIfOutOfWater(hybridNavData _Hybrid)
    {
        if (_Hybrid.hybridGameObject.transform.position.y > waterHight)
        {

            Vector3 oppositeDirection = -_Hybrid.hybridGameObject.transform.forward;
            oppositeDirection = new Vector3(oppositeDirection.x + UnityEngine.Random.Range(-10f, 10f), oppositeDirection.y + UnityEngine.Random.Range(-10f, 10f), oppositeDirection.z + UnityEngine.Random.Range(-10f, 10f));


            if (_Hybrid.isTurning == false)
            {
                _Hybrid.hybridGameObject.transform.position = new Vector3(_Hybrid.hybridGameObject.transform.position.x, _Hybrid.hybridGameObject.transform.position.y - 0.2f, _Hybrid.hybridGameObject.transform.position.z);
                StartCoroutine(turnHybrid(_Hybrid, oppositeDirection, rotationSpeed));
            }
        }
    }
    /// <summary>
    /// checks to see if the hybrid is about to collide with a wall
    /// </summary>
    /// <param name="_Hybrid"></param>
    private void CheckIfWall(hybridNavData _Hybrid)
    {
        Ray hybridRay = new Ray(_Hybrid.hybridGameObject.transform.position, _Hybrid.hybridGameObject.transform.forward);
        float raycastDistance = 2f;

        Debug.DrawRay(hybridRay.origin, hybridRay.direction * raycastDistance, Color.red);

        int layerMask = ~LayerMask.GetMask("Fish");

        if (Physics.Raycast(hybridRay, out RaycastHit hit, raycastDistance, layerMask))
        {

            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                Vector3 oppositeDirection = -_Hybrid.hybridGameObject.transform.forward;
                oppositeDirection = new Vector3(oppositeDirection.x + UnityEngine.Random.Range(-10f, 10f), oppositeDirection.y + UnityEngine.Random.Range(-10f, 10f), oppositeDirection.z + UnityEngine.Random.Range(-10f, 10f));
                if (_Hybrid.isTurning == false)
                {
                    StartCoroutine(turnHybrid(_Hybrid, oppositeDirection, rotationSpeed));
                }
            }
        }
    }
    /// <summary>
    /// calculates the the inputted hybrids direction using the boids algorithm
    /// </summary>
    /// <param name="_hybridd"></param>
    void ApplyFlockRules(hybridNavData _hybridd)
    {

        List<hybridNavData> gos = hybridsInPond;

        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = FishSpawner.goalPos;


        float dist;

        int groupSize = 0;
        gSpeed = 0;

        foreach (hybridNavData go in gos)
        {
            if (go != _hybridd)
            {
                dist = Vector3.Distance(go.hybridGameObject.transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    vCentre += go.hybridGameObject.transform.position;
                    groupSize++;

                    if (dist < 3.5f)
                    {
                        vAvoid = vAvoid + (this.transform.position - go.hybridGameObject.transform.position);
                    }
                    gSpeed += _hybridd.baseSpeed;
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


}


