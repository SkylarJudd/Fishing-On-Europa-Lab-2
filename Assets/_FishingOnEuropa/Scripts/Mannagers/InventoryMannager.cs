using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using static UnityEditor.Progress;

[Serializable]
public class StoredFOEItems
{
    public int stackCount;
    public int itemID;
    public List<FOEItem> inventoryItems = new List<FOEItem>();
}


public class InventoryMannager : Singleton<InventoryMannager>
{
    [Header("Inventory")]
    [SerializeField]
    private List<StoredFOEItems> inventoryItems = new List<StoredFOEItems>();
    [SerializeField]
    private List<FOEItem> hybridInventoryItems = new List<FOEItem>();

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

    public void AddItemToInventory(FOEItem _item)
    {
        if(AddItemToInventoryStack(_item) == false)
        {
            StoredFOEItems setup = new StoredFOEItems();

            setup.stackCount = 0;
            setup.itemID = _item.europaItemSO.itemID;
            setup.inventoryItems.Add(_item);

            inventoryItems.Add(setup);
        }
    }


    private bool AddItemToInventoryStack(FOEItem _item)
    {
        foreach (StoredFOEItems storedItems in inventoryItems)
        {
            if (storedItems.itemID == _item.europaItemSO.itemID)
            {
                if (storedItems.stackCount <= _item.inventoryItemSO.itemStackSize)
                {
                    storedItems.inventoryItems.Add(_item);
                    storedItems.stackCount++;
                    //send _item to the disable object pool
                    return true;
                }
            }
        }

        return false;

    }


    public (bool, FOEItem) RemoveItemFromInventoryStack(FOEItem _item)
    {
        foreach (StoredFOEItems storedItems in inventoryItems)
        {
            if (storedItems.itemID == _item.europaItemSO.itemID)
            {
                if (storedItems.stackCount != 1)
                {

                    FOEItem itemToReturn = storedItems.inventoryItems[storedItems.stackCount - 1];
                    storedItems.stackCount--;

                    storedItems.inventoryItems.Remove(itemToReturn);

                    //send _item to the disable object pool
                    return (true, itemToReturn);
                }
                else
                {
                    FOEItem itemToReturn = storedItems.inventoryItems[storedItems.stackCount - 1];
                    inventoryItems.Remove(storedItems);
                    //get _item game object from the pool and give it to the player
                    return (true, itemToReturn);

                }
            }
        }

        return (false, null);
    }
}
