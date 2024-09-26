using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace Europa
{
    public enum StackType { UIStack, WorldStack, }
    public enum ItemType { Error, Hybrid, Food, Seed, Plushie }
    public enum FoodType { Error, Veg, Fruit, Meat, Seed, Leaves, Bugs, Flowers };
    public enum FoodList { Error, Bugs, WhiteMeat, RedMeat, Leaves, Flowers, Seeds, Carrot, Potato, Orange, Apple, Banana, WaterMelon }
    public enum ToyList { Error, }
    public enum ItemRarity { Common, Uncommon, Rare, Epic };
    public enum ItemLocation { Error, LeftHand, RightHand, World, Inventory, Tank1, Tank2, FarmStation1, FarmStation2, ZoneOne, ZoneTwo, ZoneThree, Cave, }


    public class FOEItem : GameBehaviour, IEropaItemable, ISellable
    {
        [Header("Scriptible Object Data")]
        public InventoryItemSO inventoryItemSO;
        public EuropaItemSO europaItemSO;
        public EuropaItemData europaItemData;
        public bool held;
        public GameObject nameTextGO;
        public TMP_Text nameText;

        ItemType itemType;


        public BubbleMovement bubbleMovement;

        public ulong ItemLastUnloadedTime;

        private void Start()
        {
            nameText.text = europaItemSO.itemName;
        }

        public virtual void OnDrop(Hand _Hand, Grabbable _Grabbable)
        {
            bubbleMovement.Release();
            held = false;
        }

        public virtual void OnPickUp(Hand _Hand, Grabbable _Grabbable)
        {
            bubbleMovement.PickUp();
            held = true;
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
}

