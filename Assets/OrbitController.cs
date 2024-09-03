using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoonData
{
    public GameObject moonGo;
    public float rotSpeed;
    public float orbitDays;  // How many in-game days it takes for a full orbit
    public float rotDistance;
    public bool applyScaleModifier;
    public float scale;
}

public class OrbitController : MonoBehaviour
{
    [SerializeField] private Transform gravityCenter;
    [SerializeField] private CurrentTimeSO currentTimeSO;

    [Header("Moons")]
    [SerializeField] private List<MoonData> MoonDataList = new List<MoonData>();

    [Header("Modifiers")]
    [SerializeField] private float lerpSpeed = 1;
    [SerializeField] private float rotationSpeedModifier = 1;
    [SerializeField] private float rotationDistanceModifier = 1;
    [SerializeField] private float planetScaleModifier = 1;


    private void Start()
    {
        // Set the initial scale for each moon based on its defined scale
        foreach (MoonData moonData in MoonDataList)
        {
            moonData.moonGo.transform.localScale = Vector3.one * moonData.scale;
        }
    }

    private void Update()
    {
        UpdateOrbits();
    }

    private void UpdateOrbits()
    {
        foreach (MoonData moonData in MoonDataList)
        {
            // Apply rotation speed modifier (local rotation for the moon)
            if (moonData.rotSpeed > 0)
            {
                Quaternion targetRotation = Quaternion.Euler(0, moonData.rotSpeed * rotationSpeedModifier * Time.deltaTime, 0);
                moonData.moonGo.transform.rotation = Quaternion.Slerp(moonData.moonGo.transform.rotation, moonData.moonGo.transform.rotation * targetRotation, Time.deltaTime * 2);
            }

            // Calculate the angle for the orbit based on in-game days
            float totalDaysPassed = currentTimeSO.totalHours / 75f; // Convert total hours to days
            float angle = totalDaysPassed * (360f / moonData.orbitDays); // Full orbit takes 'orbitDays'

            // Apply orbit distance modifier
            var positionCenterObject = gravityCenter.position;

            if( moonData.rotSpeed > 0 )
            {
                var targetX = positionCenterObject.x + Mathf.Cos(angle * Mathf.Deg2Rad) * moonData.rotDistance * rotationDistanceModifier;
                var targetZ = positionCenterObject.z + Mathf.Sin(angle * Mathf.Deg2Rad) * moonData.rotDistance * rotationDistanceModifier;

                // Lerp position for smooth orbiting
                Vector3 targetPosition = new Vector3(targetX, moonData.moonGo.transform.position.y, targetZ);
                moonData.moonGo.transform.position = Vector3.Lerp(moonData.moonGo.transform.position, targetPosition, Time.deltaTime * lerpSpeed);
            }
           

            if(moonData.applyScaleModifier)
            // Apply scale modifier based on the moon's original scale
            moonData.moonGo.transform.localScale = Vector3.one * moonData.scale * planetScaleModifier;
        }
    }
}
