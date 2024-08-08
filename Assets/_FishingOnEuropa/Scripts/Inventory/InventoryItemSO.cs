using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Item Scribtable Objects", menuName = "Europa/Inventory", order = 2)]
public class InventoryItemSO : ScriptableObject
{
    
    [Header("Inventory Data")]
    public int itemStackSize;
    public Rarity rarity = Rarity.Common;
    public Sprite icon;

}
