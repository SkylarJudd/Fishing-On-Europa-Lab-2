using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigitalClock : MonoBehaviour
{
    TimeManager tm;
    Text display;

    public bool _24hourClock = false;
    // Start is called before the first frame update
    void Start()
    {
        tm = FindObjectOfType<TimeManager>();
        display = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_24hourClock)
            display.text = tm.Clock24Hour();
        else
            display.text = tm.Clock12Hour();
    }
}
