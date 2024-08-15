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
    [SerializeField] TimeSettings timeSettings;
    TimeService service;

    // Start is called before the first frame update
    void Start()
    {
        service = new TimeService(timeSettings);
        volume.profile.TryGet(out colorAdjustments);
    }

    private void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
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
        float rotation = service.CalculateSunAngle();
        sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);

    }

    private void UpdateTimeOfDay()
    {

        //multiply 3.125hr or 187.5 mins

        service.UpdateTime(Time.deltaTime);

       

        //get hours
        int earthhours = service.CurrentTime.Hour;

        //get current minutes plus hours converted to minutes
        int earthminutes = service.CurrentTime.Minute;

        //times by 187.5
        int europianTimeMin = Mathf.RoundToInt((earthminutes * 187.5f) % 60);
        int europianTimeHour = Mathf.RoundToInt((earthhours * 3.125f) % 75);

        string timeString = string.Format("{0:D2}:{1:D2}", europianTimeHour, europianTimeMin);

        //print(europianTimeHour + ":" + europianTimeMin);
        
        //hours = Mathf.RoundToInt(hours * 3.125f);



        //get current time
        if (timeText != null)
        {

            //timeText.text = service.CurrentTime.ToString("hh:mm"); //earth time
            timeText.text = (timeString);
        }
    }

}
