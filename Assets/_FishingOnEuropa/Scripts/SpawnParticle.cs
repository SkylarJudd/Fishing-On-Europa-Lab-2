using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Europa
{
    public class SpawnParticle : GameBehaviour
    {
        [SerializeField]
        FOEVFXClass vFXClass;
        [SerializeField]
        FOEVFX particle;
        [SerializeField]
        bool perent;
        [SerializeField]
        bool hasExitTime;
        [SerializeField]
        float exitTime = 1f;


        public void CallParticleSpawner()
        {
            if (hasExitTime)
                _VFXM.PlayVFX(vFXClass, particle, gameObject.transform, perent, exitTime);
            else
                _VFXM.PlayVFX(vFXClass, particle, gameObject.transform, perent);
        }
    }

    [CustomEditor(typeof(Europa.SpawnParticle))]
    public class SpawnParticleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Add a space before the button
            GUILayout.Space(10);

            // Create a button in the inspector
            if (GUILayout.Button("Spawn Particle"))
            {
                // Get a reference to the target script
                Europa.SpawnParticle spawner = (Europa.SpawnParticle)target;

                // Call the particle spawner method
                spawner.CallParticleSpawner();
            }
        }
    }
}
