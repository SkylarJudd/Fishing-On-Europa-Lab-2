using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    [SerializeReference] Light sun;
    [SerializeReference] Light moon_temp; //will need to change as you can see two moons from Europa (Io and Ganymede)
    [SerializeField] AnimationCurve lightIntensityCurve;
    [SerializeField] float maxSunIntensity =1;
    [SerializeField] float maxMoonIntensity = 0.5f;


    [SerializeField] Color dayAmbientLight;
    [SerializeField] Color nightAmbientLight;
    [SerializeField] Volume volume;
    [SerializeField] Material skyboxMaterial;

    ColorAdjustments colorAdjustments;

    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TimeSettings timeSettings;
    [SerializeField] CurrentTimeSO currentTimeSO;
    TimeService service;


    private void Awake()
    {
        currentTimeSO.hour = timeSettings.startHour;

        service = new TimeService(timeSettings, currentTimeSO);
        //start time

        //start time by incrementing minute
        InvokeRepeating("IncrementMinute", 0, timeSettings.lengthOfMinute);
    }

    void Start()
    {
        currentTimeSO.totalHours += timeSettings.startHour;
        currentTimeSO.totalMinutes += timeSettings.startHour * 60;
        volume.profile.TryGet(out colorAdjustments);
    }

    #region Calculate Time
    //increment minute
    void IncrementMinute()
    {
        currentTimeSO.minute++;
        currentTimeSO.totalMinutes++;

        if (currentTimeSO.minute == 60)
        {
            currentTimeSO.minute = 0;
            IncrementHour();

        }
    }

    void IncrementHour()
    {
        currentTimeSO.hour++;
        currentTimeSO.totalHours++;

        //day has passed
        if (currentTimeSO.hour == 75)
        {
            currentTimeSO.day++;
            currentTimeSO.hour = 0;

        }
    }
    #endregion

    private void Update()
    {
        UpdateTimeOfDay();
        RotateSun(); //can chane to sub to on hour change clock
        UpdateLightSettings();
        UpdateSkyBlend();

    }
    
    void UpdateSkyBlend()
    {
        //how far accross the sky the sun has travelled
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.up);

        float blend = Mathf.Lerp(0,1, lightIntensityCurve.Evaluate(dotProduct));

        skyboxMaterial.SetFloat("_Blend", blend);
    }

    void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);

        //change light intesnity based on where sun is facing
        sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensityCurve.Evaluate(dotProduct));
        moon_temp.intensity = Mathf.Lerp(0, maxMoonIntensity, lightIntensityCurve.Evaluate(dotProduct));

        if (colorAdjustments == null) return;

        //lerp colour values
        colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensityCurve.Evaluate(dotProduct));

    }


    void RotateSun()
    {
        // Calculate y-axis rotation based on the current hour
        float rotation = currentTimeSO.hour * 4.8f;

        // Reset rotation if it exceeds 360 degrees
        if (rotation >= 360) rotation -= 360;

        // Calculate the x-axis rotation based on the y-axis rotation
        // Use Mathf.Sin to smoothly transition between -10 and +10 as y moves from 0 to 360
        float xRotation = Mathf.Sin(rotation * Mathf.Deg2Rad) * 10;

        // Set the target rotation with the calculated x and y rotation
        Quaternion targetRotation = Quaternion.Euler(xRotation, rotation, 0);

        // Smoothly transition to the target rotation
        sun.transform.rotation = Quaternion.Lerp(sun.transform.rotation, targetRotation, 1 * Time.deltaTime);
    }

    private void UpdateTimeOfDay()
    {

        string timeString = string.Format("{0:D2}:{1:D2}", currentTimeSO.hour, currentTimeSO.minute);


        //get current time
        if (timeText != null)
        {
            timeText.text = (timeString);
            dayText.text = "Day " + currentTimeSO.day;
        }
    }
}
