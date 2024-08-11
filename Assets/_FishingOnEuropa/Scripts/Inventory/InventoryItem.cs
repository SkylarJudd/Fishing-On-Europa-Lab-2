using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryItem : GameBehaviour
{
    [Header("Item Data")]
    public InventoryItemSO inventoryItemSO;

    [Header("Object Data")]
    public int currnetStack;
    public TMP_Text itemUiAmount;
    
}
