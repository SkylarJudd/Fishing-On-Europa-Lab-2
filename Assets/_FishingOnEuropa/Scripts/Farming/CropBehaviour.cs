using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{
    [Header("Crop Plot")]
    [SerializeField]
    private CropFarmingMannager cropFarmingMannager;
    [SerializeField]
    private int farnIndex;


    private void OnCollisionEnter(Collision collision)
    {
        
        FOEItem_Seed _seed = collision.gameObject.GetComponent<FOEItem_Seed>();
        if (_seed != null)
        {
            cropFarmingMannager.PlantCrop(_seed, farnIndex);
        }

    }
}
