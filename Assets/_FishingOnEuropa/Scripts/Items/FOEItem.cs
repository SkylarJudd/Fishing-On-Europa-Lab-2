using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOEItem : GameBehaviour, IEropaItemable , ISellable
{
    public InventoryItemSO inventoryItemSO;
    public EuropaItemSO europaItemSO;
     


    public void OnDrop()
    {
        throw new System.NotImplementedException();
    }

    public void OnPickUp()
    {
        throw new System.NotImplementedException();
    }

    public void OnPlaceInInventory()
    {
        throw new System.NotImplementedException();
    }

    public void OnRemoveFromInventory()
    {
        throw new System.NotImplementedException();
    }

    public void OnSell()
    {
        throw new System.NotImplementedException();
    }

   
}
