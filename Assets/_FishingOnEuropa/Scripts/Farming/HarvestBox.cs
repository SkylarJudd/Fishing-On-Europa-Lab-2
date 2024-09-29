using System;
using UnityEngine;

namespace Europa
{
    [Serializable]
    public class StoredItems
    {
        public GameObject storedItem;
        public int ItemCount;
    }

    public class HarvestBox : MonoBehaviour
    {

        [SerializeField]
        private StoredItems[] storedTiems;

        public void Start()
        {
            InitializesItems();
        }

        /// <summary>
        /// initializes the Items setting all their values to 0 and making sure they are turned off
        /// </summary>
        private void InitializesItems()
        {
            foreach (StoredItems _Item in storedTiems)
            {
                _Item.storedItem.SetActive(false);
                _Item.ItemCount = 0;
            }
        }

        /// <summary>
        /// Increases the count of individual fruit when called.
        /// </summary>
        /// <param name="_name">seed scriptable object name of frut to add</param>
        public void UpdateHarvestValue(FoodList _name, int _Value)
        {
            switch (_name)
            {
                case FoodList.Apple:
                    UpdateFoodBox(0, _Value);
                    break;
                case FoodList.Banana:
                    UpdateFoodBox(1, _Value);
                    break;
                case FoodList.Carrot:
                    UpdateFoodBox(2, _Value);
                    break;
                case FoodList.Orange:
                    UpdateFoodBox(3, _Value);
                    break;
                case FoodList.Potato:
                    UpdateFoodBox(4, _Value);
                    break;
                case FoodList.WaterMelon:
                    UpdateFoodBox(5, _Value);
                    break;
            }
        }

        private void UpdateFoodBox(int _Index, int _Value)
        {
            storedTiems[_Index].ItemCount += _Value;
            storedTiems[_Index].storedItem.SetActive(storedTiems[_Index].ItemCount > 0);
        }
    }

}
