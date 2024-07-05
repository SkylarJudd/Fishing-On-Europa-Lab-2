using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StellarCycle : MonoBehaviour
{
    TimeManager tm;
    public float xRotation; //rotates vertically
    public float yRotation; //rotates horizontally
    public float zRotation; //rotates object like a barrel roll
    public float lerpDuration = 0.5f; // duration of the rotate/lerp
    [SerializeField] public bool isSun = false;
    [SerializeField] public bool isIO = false;
    [SerializeField] public bool isGanymede = false;
    [SerializeField] public bool isContinuous = false;
    [SerializeField] public bool isRotating = false;

    private void Start()
    {
        tm = FindObjectOfType<TimeManager>();
        if (isSun)
        {
            UpdateDayDuration(2);
            isRotating = true;
        }
        if (isIO)
        {
            UpdateDayDuration(3);
            isRotating = true;
        }
        if (isGanymede)
        {
            UpdateDayDuration(1);
            isRotating = true;
        }

    }

    private void Update()
    {
        if (isRotating)
        {
            isRotating = false; //immediately set to false or else the function will be called multiple times which messes up the rotation
            StartCoroutine(GetRotation());
        }
    }

    IEnumerator GetRotation()
    {
        float timeElapsed = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(xRotation, yRotation, zRotation); // multiply with quaternion eular angle to add angle amount
        while (timeElapsed < lerpDuration) //Maintains the rotation within the while loop
        {
            //slerps the rotation instead of an instant change
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation; // ensures the rotation finishes at exactly the right angle.
        if (isContinuous)
            RotateObject();
    }
    //A universal trigger for buttons to use remotely.
    public void RotateObject()
    {
        isRotating = true;
    }
    //For Day/Night Cycle
    void UpdateDayDuration(int _orbit)
    {
        lerpDuration = tm.dayDuration / _orbit;
    }
}
