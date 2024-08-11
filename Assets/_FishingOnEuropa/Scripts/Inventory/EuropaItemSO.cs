using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoHand;
using Autohand;

[CreateAssetMenu(fileName = "Inventory Item Scribtable Objects", menuName = "Europa/Item", order = 2)]
public class EuropaItemSO : ScriptableObject
{
    [Header("Europa Item")]
    public FOEItem worldObject;
    public string itemName;
    public string itemDescription;
    public Rarity rarity = Rarity.Common;

    [Header("Componets")]
    public Rigidbody ItemRB;
    public Grabbable ItemGrabbable;


    [Header("SaveData")]
    public int itemID;
    public Transform itemTransform;





}
