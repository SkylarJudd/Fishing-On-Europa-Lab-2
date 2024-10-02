using Obvious.Soap;
using UnityEngine;

namespace Europa
{
    public class Lure : MonoBehaviour
    {
        [SerializeField] Vector3Reference fishingRodEndPointTransform;
        [SerializeField] Vector3Reference LureEndPointTransform;
        [SerializeField] FloatReference LureCurrentDistance;
        [SerializeField] FloatReference LureCurrentMaxDistance;
        [SerializeField] FloatReference LureMaxDistanceFromRod;
        [SerializeField] BoolReference Casted;

        [SerializeField] Rigidbody lureRigidbody;
        [SerializeField] float forceMultiplier = 10f;
        [SerializeField] float maxForce = 50f;

        private void Awake()
        {
            LureCurrentDistance.Variable.OnValueChanged += OnDistanceChanged;
        }

        private void OnDisable()
        {
            LureCurrentDistance.Variable.OnValueChanged += OnDistanceChanged;
        }

        private void OnDistanceChanged(float newValue)
        {
            // Check if the current distance exceeds the allowed max distance
            if (LureCurrentDistance.Value > LureCurrentMaxDistance.Value)
            {
                // Calculate how much the current distance exceeds the max allowed distance
                float excessDistance = LureCurrentDistance.Value - LureCurrentMaxDistance.Value;

                // Calculate direction from the lure to the fishing rod
                Vector3 directionToRod = (fishingRodEndPointTransform.Value - LureEndPointTransform.Value).normalized;

                // Calculate the force based on the excess distance and force multiplier
                Vector3 force = directionToRod * excessDistance * forceMultiplier;

                // Clamp the magnitude of the force to the specified maxForce
                force = Vector3.ClampMagnitude(force, maxForce);

                // Apply the clamped force to the Rigidbody
                lureRigidbody.AddForce(force, ForceMode.Force);

                // Optionally, print out for debugging
                Debug.Log($"Applying force towards rod: {force}, Force magnitude: {force.magnitude}");
            }
        }

       

        private void Update()
        {
            LureEndPointTransform.Value = transform.position;
        }
    }
}
