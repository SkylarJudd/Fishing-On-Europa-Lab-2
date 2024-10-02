using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public enum HybridVisualsState
    {
        Error, World, Bubble, Inventory,
    }

    public class FOEItem_Hybrid : FOEItem
    {
        ItemRarity rarity;
        public HybridSO hybridSO;
        public HybridNavigationData navigationData;
        public FOESaveItem_Hybrid FOEHybridSaveData;

        [SerializeField]
        private GameObject inWorldVisuals;
        [SerializeField]
        private GameObject inBubbleVisuals;
        [SerializeField]
        private GameObject inInventoryVisuals;
        [SerializeField]
        private MeshRenderer ballVisuals;
        [SerializeField]
        GameObject patTrigger;
        HybridInteractionTrigger hybridInteractionTrigger;

        FOESaveItem_Hybrid _hybridSave;


        private void Start()
        {
            hybridInteractionTrigger = GetComponentInChildren<HybridInteractionTrigger>();

            bubbleMovement.isFloating = false;
            hybridSO.patTrigger = patTrigger.GetComponent<HybridHeadPatTrigger>();
            hybridSO.hybridInteractionTrigger = hybridInteractionTrigger;
        }
        public void SetVisuals(HybridVisualsState _State)
        {
            HideAllObjects();
            switch (_State)
            {
                case HybridVisualsState.Error:
                    Debug.LogError("Hybrid Visual State has not been set");
                    break;
                case HybridVisualsState.World:
                    inWorldVisuals.SetActive(true);
                    patTrigger.SetActive(true);
                    break;
                case HybridVisualsState.Bubble:
                    ballVisuals.enabled = true;
                    inBubbleVisuals.SetActive(true);
                    patTrigger.SetActive(false);

                    break;
                case HybridVisualsState.Inventory:
                    ballVisuals.enabled = true;
                    inInventoryVisuals.SetActive(true);
                    patTrigger.SetActive(false);

                    break;
                default:
                    Debug.Log("State Not found, Please ensure that the state has been added to this switch statement");
                    break;
            }
        }

        private void HideAllObjects()
        {
            ballVisuals.enabled = false;
            inWorldVisuals.SetActive(false);
            inBubbleVisuals?.SetActive(false);
            inInventoryVisuals?.SetActive(false);
            patTrigger.SetActive(false);

        }

        public void InitHybrid(GameObject go, float waterHight, ItemLocation pondType, HybridState state, HybridVisualsState visualState)
        {
            europaItemData.itemGO = go;

            navigationData.waterHeight = waterHight;
            navigationData.hybridLocation = pondType;

            navigationData.minSpeed = hybridSO.fishSpeed;
            navigationData.maxSpeed = hybridSO.fishSpeed * 2;
            navigationData.hybridState = state;
            navigationData.velocity = Vector3.forward * hybridSO.fishSpeed;
            navigationData.firstNav = true;

            // Assign Rigidbody to hybrid, or add one if it is missing
            print(europaItemData.itemRB);
            if (europaItemData.itemRB == null)
            {
                Debug.LogWarning($"{europaItemData.itemGO.name} does not have a Rigidbody; one has been assigned"); 
                europaItemData.itemRB = europaItemData.itemGO.AddComponent<Rigidbody>();
            }

            SetVisuals(visualState);
        }

        /// <summary>
        /// Initalise data into Hybrid
        /// </summary>
        /// <param name="savedHybrid"></param>
        /// <param name="spawnPoint"></param>
        /// <param name="waterHight"></param>
        /// <param name="location"></param>
        /// <param name="state"></param>
        /// <param name="visualState"></param>


        public void InitSavedHybrid(GameObject go, FOESaveItem_Hybrid savedHybrid, Transform spawnPoint, float waterHight, ItemLocation location, HybridState state, HybridVisualsState visualState)

        {
            europaItemData.itemGO = go;

            _hybridSave = savedHybrid;

            _hybridSave.hybridTrust = savedHybrid.hybridTrust;

            europaItemData.itemName = savedHybrid.hybridName;
            europaItemData.itemTransform = spawnPoint;

            navigationData.shiny = savedHybrid.hybridShiny;
            navigationData.waterHeight = waterHight;
            navigationData.hybridLocation = location;
            navigationData.hybridState = state;
            navigationData.firstNav = true;

            navigationData.minSpeed = hybridSO.fishSpeed;
            navigationData.maxSpeed = hybridSO.fishSpeed * 2;
            navigationData.velocity = Vector3.forward * hybridSO.fishSpeed;

            nameText.text = europaItemData.itemName;

            SetVisuals(visualState);
        }

        public Transform ReturnPatTrigger()
        {
            return hybridSO.patTrigger.ReturnHand();
        }
    }
}

