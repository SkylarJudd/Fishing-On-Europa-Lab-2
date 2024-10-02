using Autohand;
using UnityEngine;
using TMPro;

namespace Europa
{
    public enum StackType { UIStack, WorldStack, }
    public enum ItemType { Error, Hybrid, Food, Seed, Plushie }
    public enum FoodType { Error, Veg, Fruit, Meat, Seed, Leaves, Bugs, Flowers };
    public enum FoodList { Error, Bugs, WhiteMeat, RedMeat, Leaves, Flowers, Seeds, Carrot, Potato, Orange, Apple, Banana, WaterMelon }
    public enum ToyList { Error, }
    public enum ItemRarity { Common, Uncommon, Rare, Epic };
    public enum ItemLocation { Error, LeftHand, RightHand, World, Inventory, Tank1, Tank2, FarmStation1, FarmStation2, ZoneOne, ZoneTwo, ZoneThree, Cave, }
    public enum CropState { Seed, Sprout, Flowering, Fruited, Harvested }


    public class FOEItem : GameBehaviour, IEropaItemable, ISellable
    {
        [Header("Scriptible Object Data")]
        public InventoryItemSO inventoryItemSO;
        public EuropaItemSO europaItemSO;
        public EuropaItemData europaItemData;
        public bool held;
        public GameObject nameTextGO;
        public TMP_Text nameText;

        public GameObject[] itemVisuals;

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

        /// <summary>
        /// A function that is used to update the Visuals of the FOEItme
        /// </summary>
        /// <param name="Index"></param>
        public void SwapVisuals(int Index)
        {
            foreach (var item in itemVisuals)
            {
                item.SetActive(false);
            }

            if (Index >= itemVisuals.Length || Index < 0)
            {
                Debug.LogError($"{europaItemData.itemName}'s Visuals Index of {Index} does not contain a game object. Please check that the visuals are assigned correctly and you are calling the correct value.");
            }
            else
            {
                itemVisuals[Index].SetActive(true);
            }
        }

        /// <summary>
        /// A Function that is used to reset the Item when its added back to the object Pool
        /// </summary>
        public virtual void ResetItem()
        {
            foreach (var item in itemVisuals)
            {
                item.SetActive(false);
            }
        }


    }
}

