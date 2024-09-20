using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Europa
{
    public class FOEItem_Food : FOEItem, ISellable
    {
        public FoodType foodType;
        public FoodList foodItem;
        public int happinesAdd;

        [Header("Visuals")]
        [SerializeField] GameObject inWorldVisuals;
        [SerializeField] GameObject onPlantVisuals;
        [SerializeField] GameObject inUIVisuals;
        [SerializeField] MeshRenderer bubbleVisual;

        public override void OnDrop(Hand _Hand, Grabbable _Grabbable)
        {
            // Call the base class's OnDrop method
            base.OnDrop(_Hand, _Grabbable);

            //_PLAYER.OnFoodDrop(_Hand.left);
            Debug.LogError("This Is broken Find Another Way to send this though events");
        }

        public override void OnPickUp(Hand _Hand, Grabbable _Grabbable)
        {
            // Call the base class's OnPickUp method
            base.OnPickUp(_Hand, _Grabbable);

            FOEItem_Food foodItemComponent = _Grabbable.gameObject.GetComponent<FOEItem_Food>();
            if (foodItemComponent != null)
            {
                //_PLAYER.OnFoodPickUp(_Hand.left, foodItemComponent);
                Debug.LogError("This Is broken Find Another Way to send this though events");
            }
            else
            {
                Debug.LogWarning("The object does not have a FOEItem_Food component.");
            }
        }
    }
}

