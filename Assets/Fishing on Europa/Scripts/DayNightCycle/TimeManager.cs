using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeReference] Light sun;
    [SerializeReference] Light moon_temp; //will need to change as you can see two moons from Europa (Io and Ganymede)

    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TimeSettings timeSettings;
    TimeService service;

    // Start is called before the first frame update
    void Start()
    {
        service = new TimeService(timeSettings);
        
    }

    private void Update()
    {
        UpdateTimeOfDay();
        RotateSun();

    }

    void RotateSun()
    {
        float rotation = service.CalculateSunAngle();
        sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);

    }

    private void UpdateTimeOfDay()
    {
        service.UpdateTime(Time.deltaTime);

        //get current time
        if(timeText != null)
        {
            timeText.text = service.CurrentTime.ToString("hh:mm");
        }
    }
}
