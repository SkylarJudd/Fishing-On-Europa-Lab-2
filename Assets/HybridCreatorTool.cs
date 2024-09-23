using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Europa
{
    public class HybridCreatorTool : GameBehaviour
    {
        [SerializeField] private FishToSpawnSO fishToSpawn;

        [SerializeField] private ScriptableListFOESaveItem_Hybrid hybridInFarmList;
        [SerializeField] private ScriptableListFOESaveItem_Hybrid hybridInDomeList;
        [SerializeField] private ScriptableListFOESaveItem_Hybrid hybridInInvetoryList;

        [SerializeField] private int hybridID;
        [SerializeField] private string hybridName;
        [SerializeField] private bool hybridShiney;
        [SerializeField] private ItemLocation hybridLocation = ItemLocation.Tank1;
        [SerializeField] private int InventorySlot;
        [SerializeField] private int hybridTrust;

        [SerializeField] private Transform[] spawnPoints;

        public void AddToFarmList() => AddHybridToList(hybridInFarmList);
        public void AddToDomeList() => AddHybridToList(hybridInDomeList);
        public void AddToInventoryList() => AddHybridToList(hybridInInvetoryList);

        private void AddHybridToList(ScriptableListFOESaveItem_Hybrid _list)
        {
            FOESaveItem_Hybrid _newHybrid = new FOESaveItem_Hybrid();

            _newHybrid.itemID = hybridID;
            _newHybrid.itemLocation = hybridLocation;
            _newHybrid.itemInventorySlot = InventorySlot;
            _newHybrid.hybridName = hybridName;
            _newHybrid.hybridShiny = hybridShiny;

            _newHybrid.hybridTrust = hybridTrust;



            _list.Add(_newHybrid);
        }

        // Only include this part in the Unity Editor
#if UNITY_EDITOR
        [CustomEditor(typeof(HybridCreatorTool))]
        public class HybridCreatorToolEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector(); // Draws the default inspector with your serialized fields.

                HybridCreatorTool hybridCreatorTool = (HybridCreatorTool)target;

                // Add buttons for the add functions
                if (GUILayout.Button("Add to Farm List"))
                {
                    hybridCreatorTool.AddToFarmList();
                }

                if (GUILayout.Button("Add to Dome List"))
                {
                    hybridCreatorTool.AddToDomeList();
                }

                if (GUILayout.Button("Add to Inventory List"))
                {
                    hybridCreatorTool.AddToInventoryList();
                }
            }
        }
#endif
    }
}