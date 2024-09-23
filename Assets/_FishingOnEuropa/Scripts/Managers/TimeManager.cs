using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Obvious.Soap;


namespace Europa
{
    public class TimeManager : Singleton<TimeManager>
    {
        [SerializeReference] Light sun;
        [SerializeReference] Light moon_temp; //will need to change as you can see two moons from Europa (Io and Ganymede)
        [SerializeField] AnimationCurve lightIntensityCurve;
        [SerializeField] float maxSunIntensity = 1;
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

        [SerializeField] ScriptableEventNoParam newDayEvent;

        private void Awake()
        {
            //Save data here



            CurrentTimeSO lastSavedCurrentTime = new();
            lastSavedCurrentTime = GetDataFromSaveManager(currentTimeSO);
            LoadTimeFromSave(lastSavedCurrentTime);


            currentTimeSO.hour = timeSettings.startHour;

            service = new TimeService(timeSettings, currentTimeSO);
            //start time

            //start time by incrementing minute
            InvokeRepeating("IncrementMinute", 0, timeSettings.lengthOfMinute);
        }
        #region Enable/Disable
        private void OnEnable()
        {
            GameEvents.OnUpdateTime += GameEvents_OnUpdateTime;


        }

        private void OnDisable()
        {
            GameEvents.OnUpdateTime -= GameEvents_OnUpdateTime;

        }
        #endregion

        #region Add Event Listeners
        private void GameEvents_OnUpdateTime(int arg1, int arg2, int arg3)
        {
            UpdateTimeFromInt(arg1, arg2, arg3);
        }


        #endregion


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

            if (currentTimeSO.hour == timeSettings.sunriseHour) GameEvents.SunriseEvent();
            if (currentTimeSO.hour == timeSettings.sunsetHour) GameEvents.SunsetEvent();


            //day has passed
            if (currentTimeSO.hour == 75)
            {
                //invoke new day event
                newDayEvent.Raise();

                currentTimeSO.day++;
                currentTimeSO.hour = 0;

            }
        }
        #endregion

        private void Update()
        {
            UpdateTimeOfDay();
            RotateSun(); //can change to sub to on hour change clock
            UpdateLightSettings();
            UpdateSkyBlend();

        }

        /// <summary>
        /// Update sky box material based on current time
        /// </summary>
        void UpdateSkyBlend()
        {
            //how far across the sky the sun has traveled
            float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.up);

            float blend = Mathf.Lerp(0, 1, lightIntensityCurve.Evaluate(dotProduct));

            skyboxMaterial.SetFloat("_Blend", blend);
        }

        /// <summary>
        /// Update directional lights based on current time
        /// </summary>
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

        /// <summary>
        /// Rotate sun based on current time
        /// </summary>
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

        /// <summary>
        /// Update UI with current time of day
        /// </summary>
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

        /// <summary>
        /// Update CurrentTimeSO with last saved time
        /// </summary>
        /// <param name="_savedTime"></param>
        private void LoadTimeFromSave(CurrentTimeSO _savedTime)
        {
            currentTimeSO.day = _savedTime.day;
            currentTimeSO.hour = _savedTime.hour;
            currentTimeSO.minute = _savedTime.minute;

            currentTimeSO.totalMinutes = _savedTime.minute + (_savedTime.hour * 60) + (_savedTime.day * 1440);
            currentTimeSO.totalHours = _savedTime.hour + (_savedTime.day * 24);
        }

        /// <summary>
        /// Update time from UpdateTime event using integers
        /// </summary>
        /// <param name="_hour"></param>
        /// <param name="_min"></param>
        /// <param name="_day"></param>
        void UpdateTimeFromInt(int _hour, int _min, int _day)
        {
            currentTimeSO.day = _day;
            currentTimeSO.hour = _hour;
            currentTimeSO.minute = _min;

            currentTimeSO.totalMinutes = _min + (_hour * 60) + (_day * 1440);
            currentTimeSO.totalHours = _hour + (_day * 24);
        }

        /// <summary>
        /// Retrive last saved time from save file
        /// </summary>
        /// <param name="_currentTimeData"></param>
        /// <param name="_index"></param>
        /// <returns></returns>
        private CurrentTimeSO GetDataFromSaveManager(CurrentTimeSO _currentTimeData)
        {
            _currentTimeData.day = _TSM.currentSave.time.days.Value;
            _currentTimeData.hour = _TSM.currentSave.time.hours.Value;
            _currentTimeData.minute = _TSM.currentSave.time.minutes.Value;

            return _currentTimeData;
        }

        /// <summary>
        /// Save current time to save file
        /// </summary>
        /// <param name="_currentTime"></param>
        private void AddTimeToSave(CurrentTimeSO _currentTime)
        {
            _TSM.currentSave.time.days.Value = _currentTime.day;
            _TSM.currentSave.time.hours.Value = _currentTime.hour;
            _TSM.currentSave.time.minutes.Value = _currentTime.minute;
        }
    }
}

