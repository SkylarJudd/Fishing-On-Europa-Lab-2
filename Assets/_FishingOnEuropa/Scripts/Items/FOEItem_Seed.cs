using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class FOEItem_Seed : FOEItem_Food
    {
        public SeedsSO seedsSO;

        [SerializeField]
        [Tooltip("Threshold angle in degrees for the object to be considered upside down.")]
        private float rotationThreshold = 170f;
        [SerializeField]
        [Tooltip("Total number of seeds to drop.")]
        public int totalSeeds = 4;
        [SerializeField]
        [Tooltip("Delay between each seed drop.")]
        public float dropDelay = 0.4f;

        private bool isDroppingSeeds = false;
        private Coroutine dropSeedsCoroutine;
        private int seedsDropped = 0;

        private void Update()
        {
            checkRotation();
        }

        private void checkRotation()
        {
            if (!held)
                return;

            // Calculate the angle between the object's up vector and the world up vector
            float angle = Vector3.Angle(europaItemData.itemGO.transform.up, Vector3.up);

            if (angle >= rotationThreshold)
            {
                // If the angle exceeds the threshold and the coroutine hasn't been started, start it
                if (!isDroppingSeeds)
                {
                    //dropSeedsCoroutine = StartCoroutine(DropSeeds());
                    isDroppingSeeds = true;
                }
            }
            else
            {
                // If the object is no longer upside down and the coroutine is running, stop it
                if (isDroppingSeeds)
                {
                    //StopCoroutine(dropSeedsCoroutine);
                    isDroppingSeeds = false;
                    Debug.Log("Stopped dropping seeds because the bag is upright.");
                }
            }

        }

        IEnumerator DropSeeds()
        {
            Debug.Log("Dropping seeds...");

            while (seedsDropped < totalSeeds)
            {
                yield return new WaitForSeconds(dropDelay);

                // Simulate dropping a seed
                Debug.Log($"Seed {seedsDropped + 1} dropped!");

                seedsDropped++;

                // If the object turns upright while dropping seeds, the coroutine will stop
                if (Vector3.Angle(europaItemData.itemGO.transform.up, Vector3.up) < rotationThreshold)
                {
                    yield break;
                }
            }

            ResetSeed();
        }

        private void ResetSeed()
        {
            //send back to the object pool mannager and reset the values 
        }

       
        
    }
}

