using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;



public class InventoryMannager : Singleton<InventoryMannager>
{
    [Header("Inventory")]
    [SerializeField]
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    [SerializeField]
    private List<InventoryItem> hybridInventoryItems = new List<InventoryItem>();

    [Header("Inventory item")]
    [SerializeField]
    private int maxStackedItems;    //the max amount of items that can be added to the item inventory

    [Header("Hybrid item")]
    [SerializeField]
    private int maxHybrids;    //the max number of hybrids to be added to the inventory


    private void Start()
    {
        //get items from the save mannager

    }

    public bool AddItemToInventory(InventoryItem _item)
    {
        foreach (InventoryItem item in inventoryItems)
        {
            if (item.inventoryItemSO.itemID == _item.inventoryItemSO.itemID)
            {
                if (item.currnetStack >= item.inventoryItemSO.itemStackSize)
                {
                    inventoryItems.Add(_item);
                    item.currnetStack++;
                    //send _item to the disable object pool
                    return true;
                }
            }
        }

        return false;

    }

    public (bool, FOEItem) RemoveItemFromInventory(InventoryItem _item)
    {
        foreach (InventoryItem item in inventoryItems)
        {
            if (item.inventoryItemSO.itemID == _item.inventoryItemSO.itemStackSize)
            {
                if (item.currnetStack != 1)
                {

                    item.currnetStack--;
                    //send _item to the disable object pool
                    return (true, item.inventoryItemSO.worldObject);
                }
                else
                {
                    inventoryItems.Remove(item);
                    //get _item game object from the pool and give it to the player
                    return (true, item.inventoryItemSO.worldObject);

                }
            }
        }

        return (false, _item.inventoryItemSO.worldObject);
    }
}
