using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;
using NaughtyAttributes;

namespace Europa
{
    public class TempAddCrop : MonoBehaviour
    {
        ScriptableListCropData cropList;

        [SerializeField]
        FOEItem_Crop cropToAdd;

        private void OnGUI()
        {
            AddCrop();
        }

        [Button]
        private void AddCrop()
        {
            if (GUILayout.Button("Add Crop"))
            {
                cropList.Add(cropToAdd);
            }
        }
    }
}
