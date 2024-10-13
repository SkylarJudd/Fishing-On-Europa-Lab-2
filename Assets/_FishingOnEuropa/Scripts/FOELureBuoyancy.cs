


using UnityEngine;
using Crest;
using Obvious.Soap;

namespace Europa
{

    public class FOELureBuoyancy : SimpleFloatingObject
    {
        [Header("LureEvents")]
        [Tooltip("Scriptible Event that is raised when the lure Hits the water")]
        [SerializeField] ScriptableEventNoParam OnLureHitWater;
        [Tooltip("Scriptible Event that is raised when the lure Hits the water")]
        [SerializeField] ScriptableEventNoParam OnLureExitWater;

        // Variable to track previous state of _inWater
        [SerializeField] private bool _previousInWaterState;

        /// <summary>
        /// This Function is called when the lure hits the water
        /// </summary>
        protected virtual void OnEnterWater()
        {

            Debug.Log("Object has entered the water!");
            OnLureHitWater.Raise();
        }

        protected virtual void OnExitWater()
        {

            Debug.Log("Object has exited the water!");
            OnLureExitWater.Raise();
        }


        // Override FixedUpdate to detect state change
        protected override void FixedUpdate()
        {
            // Call the base class's FixedUpdate to maintain the existing functionality
            base.FixedUpdate();

            // Check if the object has just entered the water
            if (_inWater && !_previousInWaterState)
            {
                OnEnterWater();
            }
            // Check if the object has just exited the water
            else if (!_inWater && _previousInWaterState)
            {
                OnExitWater();
            }

            // Update the previous state to the current state at the end of the update
            _previousInWaterState = _inWater;
        }
    }
}






