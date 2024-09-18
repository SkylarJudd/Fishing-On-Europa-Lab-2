using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Item Scribtable Objects", menuName = "Europa/Items/Inventory", order = 2)]
public class InventoryItemSO : ScriptableObject
{
    
    [Header("Inventory Data")]
    public int itemStackSize;
    public Sprite icon;
    public int itemSlot;

}
