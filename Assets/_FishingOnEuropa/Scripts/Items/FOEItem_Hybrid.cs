using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HybridVisualsState
{
    Error, World, Bubble, Inventory, 
}

public class FOEItem_Hybrid : FOEItem
{
    ItemRarity rarity;
    public HybridSO hybridSO;

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
        hybridSO.patTrigger = inWorldVisuals.transform.Find("PatHeadTrigger").GetComponent<HybridHeadPatTrigger>();
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
                break;
            case HybridVisualsState.Bubble:
                ballVisuals.enabled = true;
                inBubbleVisuals.SetActive(true);    
                break;
            case HybridVisualsState.Inventory:
                ballVisuals.enabled = true;
                inInventoryVisuals.SetActive(true);
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
    }
}
