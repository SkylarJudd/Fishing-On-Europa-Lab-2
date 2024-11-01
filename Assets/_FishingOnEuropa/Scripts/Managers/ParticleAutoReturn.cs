// Ignore Spelling: Dict FOEVFX VFX IFOEVFX

using UnityEngine;

namespace Europa
{
    public class ParticleAutoReturn : MonoBehaviour
    {
        private ParticleSystem particle;
        private System.Action<ParticleSystem> returnCallback;

        public void Init(ParticleSystem particle, System.Action<ParticleSystem> callback)
        {
            this.particle = particle;
            this.returnCallback = callback;

            // Retrieve the main module and modify its stop action.
            var mainModule = particle.main;  // Copy the struct.
            mainModule.stopAction = ParticleSystemStopAction.Callback;  // Modify the copy.

            // Ensure the particle system stops properly.
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void OnParticleSystemStopped()
        {
            returnCallback?.Invoke(particle);  // Call the return function.
            Destroy(this);  // Destroy this helper component to avoid duplication.   
        }
    }
}
