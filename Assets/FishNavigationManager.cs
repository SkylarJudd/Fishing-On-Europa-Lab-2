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

    [SerializeField] float waterHight = 0;

    private Coroutine updateHybridFlyingCoroutine;
    private Coroutine updateHitWaterCoroutine;

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
    }

    private void Start()
    {
        //waterHight = GetComponentInParent<GeyserMannager>().waterHight.transform.position.y;
        updateHybridFlyingCoroutine = StartCoroutine(UpdateHybridFlying());
        updateHitWaterCoroutine = StartCoroutine(UpdateHitWater());
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

    private IEnumerator UpdateHybridFlying()
    {
        while (true)
        {
            print("Looping1");
            if (hybridsFlying.Count > 0)
            {
                print("Hybrids flying count is grater then 0");
                for (int i = hybridsFlying.Count - 1; i >= 0; i--)
                {
                    hybridNavData _hybridd = hybridsFlying[i];

                    // Animate the Hybrids

                    // Check if Hybrid has left the water
                    if (_hybridd.HybridState != HybridState.HybridFlying)
                    {
                        _hybridd.hybridRigidbody.useGravity = true;
                        _hybridd.hybridRigidbody.drag = airDrag;
                        _hybridd.HybridState = HybridState.HybridFlying;
                    }

                    print($"{_hybridd} has a y value of {_hybridd.hybridGameObject.transform.position.y} and the water hight is {waterHight}.y ");
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

    private IEnumerator UpdateHitWater()
    {
        while (true)
        {
            //print("Looping2");
            if (hybridsHitWater.Count > 0)
            {
                print("Hybrids hit water count is grater then 0");
                foreach (hybridNavData _hybridd in hybridsHitWater)
                {
                    if (_hybridd.HybridState != HybridState.HybridHitWater)
                    {
                        _hybridd.hybridRigidbody.useGravity = false;
                        _hybridd.hybridRigidbody.drag = waterDrag;
                        _hybridd.HybridState = HybridState.HybridHitWater;
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    //void ApplyFlockRules(hybridNavData navData)
    //{
    //    //bc we said the array to static we are able to refrance it here and pull the fish from it
    //    List<GameObject> gos = fishSpawnerScript.GetFishToDepawnList();



    //    Vector3 vCentre = Vector3.zero;
    //    Vector3 vAvoid = Vector3.zero;
    //    float gSpeed = 0.1f;

    //    Vector3 goalPos = FishSpawner.goalPos;


    //    float dist;

    //    int groupSize = 0;
    //    gSpeed = 0;

    //    foreach (GameObject go in gos)
    //    {
    //        if (go != this.gameObject)
    //        {
    //            dist = Vector3.Distance(go.transform.position, this.transform.position);
    //            if (dist <= neighbourDistance)
    //            {
    //                vCentre += go.transform.position;
    //                groupSize++;

    //                if (dist < 3.5f)
    //                {
    //                    vAvoid = vAvoid + (this.transform.position - go.transform.position);
    //                }
    //                FishNavigationScript anotherFlock = go.GetComponent<FishNavigationScript>();
    //                gSpeed += anotherFlock.speed;
    //            }
    //        }
    //    }
    //    if (groupSize > 0)
    //    {
    //        //finds the avarge of the group center
    //        vCentre = vCentre / groupSize + (goalPos - this.transform.position); //+ new Vector3(Random.Range(1,-1), Random.Range(1, -1), Random.Range(1, -1)));
    //                                                                             // Calculate the average speed for the group
    //                                                                             //print("Gspeed before avarge" + gSpeed);
    //        gSpeed = gSpeed / groupSize;
    //        //print("group size = " + groupSize);
    //        // Update the fish's speed
    //        speed = gSpeed;
    //        // print("fish Is going " + speed);

    //        //updates the direction the fish needs to turn based on the avoid and the center values
    //        Vector3 direction = (vCentre + vAvoid) - transform.position;

    //        //slowly turns the fish 
    //        if (direction != Vector3.zero)
    //        {
    //            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    //        }
    //    }
    //}

}
