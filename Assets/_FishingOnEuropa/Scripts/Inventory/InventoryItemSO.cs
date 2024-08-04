using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Item Scribtable Objects", menuName = "Europa/Inventory", order = 2)]
public class InventoryItemSO : ScriptableObject
{
    [Header("Inventory Item")]
    public FOEItem worldObject;
    public Sprite icon;
    public string itemName;
    public string itemDescription;

    [Header("SaveData")]
    public int itemID;

    [Header("Inventory Data")]
    public int itemStackSize;
    public Rarity rarity = Rarity.Common;


}
