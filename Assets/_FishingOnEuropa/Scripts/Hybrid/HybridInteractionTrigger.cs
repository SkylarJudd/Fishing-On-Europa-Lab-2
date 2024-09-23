using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class HybridInteractionTrigger : GameBehaviour
    {
        FOEItem_Hybrid hybrid;

        public bool playerInRange;

        List<FOEItem_Food> foodInRange;

        private void Start()
        {
            hybrid = GetComponentInParent<FOEItem_Hybrid>();
        }

        private void OnTriggerEnter(Collider other)
        {
            //Player in range
            if(other.CompareTag("Player"))
            {
                playerInRange = true;
            }

            //Food in range
            FOEItem_Food foodComponent = other.GetComponent<FOEItem_Food>();
            if (foodComponent != null)
            {
                if (!foodInRange.Contains(foodComponent)) foodInRange.Add(foodComponent);
            }

            //Plushie
            FOEItem_Plushie plushieComponent = other.GetComponent<FOEItem_Plushie>();
            if(plushieComponent != null)
            {
                if(plushieComponent.ToyItem == hybrid.hybridSO.favToy)
                {
                    _TM.UpdateHybridPlushieTrust(hybrid);

                    //audio here
                }
            }


        }

        private void OnTriggerExit(Collider other)
        {
            //Player in range
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
            }

            FOEItem_Food foodComponent = other.GetComponent<FOEItem_Food>();
            if (foodComponent != null)
            {
                if (foodInRange.Contains(foodComponent)) foodInRange.Remove(foodComponent);
            }
        }

    }
}
