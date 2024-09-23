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


        private void Start()
        {
            bubbleMovement.isFloating = false;
            hybridSO.patTrigger = patTrigger.GetComponent<HybridHeadPatTrigger>();
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
            europaItemSO.itemGO = go;

            navigationData.waterHeight = waterHight;
            navigationData.hybridLocation = pondType;

            navigationData.minSpeed = hybridSO.fishSpeed;
            navigationData.maxSpeed = hybridSO.fishSpeed * 2;
            navigationData.hybridState = state;
            navigationData.velocity = Vector3.forward * hybridSO.fishSpeed;
            navigationData.firstNav = true;

            // Assign Rigidbody to hybrid, or add one if it is missing

            if (europaItemSO.itemRB == null)
            {
                Debug.LogWarning($"{europaItemSO.itemGO.name} does not have a Rigidbody; one has been assigned");
                europaItemSO.itemRB = europaItemSO.itemGO.AddComponent<Rigidbody>();
            }

            SetVisuals(visualState);
        }

        public void InitSavedHybrid(FOESaveItem_Hybrid savedHybrid, Transform spawnPoint, float waterHight, ItemLocation location, HybridState state, HybridVisualsState visualState)
        {

            europaItemSO.itemGO = savedHybrid.itemGO;
            europaItemSO.itemName = savedHybrid.hybridName;
            europaItemSO.itemTransform = spawnPoint;

            navigationData.shiny = savedHybrid.hybridShiny;
            navigationData.waterHeight = waterHight;
            navigationData.hybridLocation = location;
            navigationData.hybridState = state;
            navigationData.firstNav = true;

            navigationData.minSpeed = hybridSO.fishSpeed;
            navigationData.maxSpeed = hybridSO.fishSpeed * 2;
            navigationData.velocity = Vector3.forward * hybridSO.fishSpeed;

            SetVisuals(visualState);
        }

        public Transform ReturnPatTrigger()
        {
            return hybridSO.patTrigger.ReturnHand();
        }
    }
}

