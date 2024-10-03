using Obvious.Soap;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Europa
{
    public class FarmDataManager : GameBehaviour
    {
        [Header("Events")]
        [Tooltip("Morning Event, An Event that is triggerd in the morning starts")]
        [SerializeField] private ScriptableEventNoParam dawnEvent;
        [Tooltip("Darkning Event, An event that is called when the Eclips Starts")]
        [SerializeField] private ScriptableEventNoParam darkningEvent;
        [Tooltip("Morning Event, An even that is called when afternoon starts")]
        [SerializeField] private ScriptableEventNoParam duskEvent;
        [Tooltip("Morning Event, an event that is called when night starts")]
        [SerializeField] private ScriptableEventNoParam darkEvent;

        [Header("Crop List")]
        [Tooltip("Scriptiblke Object list that contains all of the crops")]
        [SerializeField] private ScriptableListCropData cropList;

        private WaitForSeconds  = new WaitForSeconds();

        private void OnEnable()
        {
            dawnEvent.OnRaised += DawnEvent;
            darkningEvent.OnRaised += DarkningEvent;
            duskEvent.OnRaised += DuskEvent;
            darkEvent.OnRaised += DarkEvent;

        }

        private void OnDisable()
        {
            dawnEvent.OnRaised -= DawnEvent;
            darkningEvent.OnRaised -= DarkningEvent;
            duskEvent.OnRaised -= DuskEvent;
            darkEvent.OnRaised -= DarkEvent;
        }

        private IEnumerator DarkEventCorutine()
        {
            foreach (FOEItem_Crop _crop in cropList)
            {
                cropList.UpdateGrothStage(_crop);
                yield return new WaitForSeconds(1);
            } 
        }
        private void DarkEvent()
        {
            
            
        }

        private void DuskEvent()
        {
           
        }

        private void DarkningEvent()
        {
            
        }

        private void DawnEvent()
        {
            
        }

       


    }
}
