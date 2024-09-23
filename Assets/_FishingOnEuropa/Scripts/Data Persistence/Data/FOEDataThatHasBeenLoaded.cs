using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;
using UnityEditor;
using Europa;

namespace Europa
{
    /// <summary>
    /// This is the soap class of all the save Data and the Scriptable Lists this is used to save all the loaded data from within the SaveManager
    /// </summary>
    [Serializable]
    public class FOEDataThatHasBeenLoaded
    {
        [Header("Saved Data")]
        [Tooltip("A soap String reference that holds the Name of the save")]
        public StringReference saveName = new StringReference();
        [Tooltip("A soap String reference that holds the date and time of the save")]
        public StringReference saveDate = new StringReference();

        [Header("Hybrids")]
        [Tooltip("A scriptible object list that holds FOESaveItems_Hybrid's that contain info about the current Hybrids in the farm.")]
        public ScriptableListFOESaveItem_Hybrid hybridFarmList;
        [Tooltip("A scriptible object list that holds FOESaveItems_Hybrid's that contain info about the current Hybrids in the Dome.")]
        public ScriptableListFOESaveItem_Hybrid hybridDomeList;
        [Tooltip("A scriptible object list that holds FOESaveItems_Hybrid's that contain info about the current Hybrids in the Players Inventory.")]
        public ScriptableListFOESaveItem_Hybrid hybridsInventoryList;


        [Header("PlayerSaveData")]
        [Tooltip("A reference to a class that stores references to the Soap objects that contain the players Data")]
        public CurrentPlayer player = new CurrentPlayer();

        [Header("Settings")]
        [Tooltip("A reference to a class that stores references to the Soap objects that contain the players settings")]
        public CurrentSettings settings = new CurrentSettings();

        [Header("Items")]
        [Tooltip("A scriptible object list that holds FOESaveItems's that contain info about the current Items in the farm.")]
        public ScriptableListFOESaveItem itemsInFarmList;
        [Tooltip("A scriptible object list that holds FOESaveItems's that contain info about the current Items in the dome.")]
        public ScriptableListFOESaveItem itemsInDomeList;
        [Tooltip("A scriptible object list that holds FOESaveItems's that contain info about the current Items in the players Inventory.")]
        public ScriptableListFOESaveItem itemsInInventoryList;


        [Header("Farming")]
        [Tooltip("A scriptible object list that holds FOESaveItems_Crop's that contain info about the current crops in the farm.")]
        public ScriptableListFOESaveItem_Crop cropsPlanted;

        [Header("Time")]
        [Tooltip("A reference to a class that stores references to the Soap objects that contain the in game time")]
        public CurrentTime time = new CurrentTime();
    }

    /// <summary>
    /// This Class Contains all the Data that will be saved to each of the save files, its set up as a mirror to the above soap version of this class and the data is converted inside the SaveMannager
    /// </summary>
    [Serializable]
    public class FOEDataFromSave
    {
        [Header("Saved Data")]
        [Tooltip("A String variable that holds the Name of the save")]
        public string saveName;
        [Tooltip("A String variable that holds the real world date and time of the save")]
        public string saveDate;

        [Header("Hybrids")]
        [Tooltip("A List that holds the save data for all the hybrids that will be loaded into the farm")]
        public List<SaveHybrid> hybridFarmList = new List<SaveHybrid>();
        [Tooltip("A List that holds the save data for all the hybrids that will be loaded into the Dome")]
        public List<SaveHybrid> hybridDomeList = new List<SaveHybrid>();
        [Tooltip("A List that holds the save data for all the hybrids that will be loaded into the players Inventory")]
        public List<SaveHybrid> hybridsInventoryList = new List<SaveHybrid>();


        [Header("PlayerSaveData")]
        [Tooltip("A reference to a class that Holds info on the players data to be saved")]
        public SavePlayer player = new SavePlayer();

        [Header("Settings")]
        [Tooltip("A reference to a class that Holds info on the players current settings")]
        public SaveSettings settings = new SaveSettings();

        [Header("Items")]
        [Tooltip("A List that holds the save data for all the Items that will be loaded into the farm")]
        public List<SaveItem> itemsInFarmList = new List<SaveItem>();
        [Tooltip("A List that holds the save data for all the Items that will be loaded into the Dome")]
        public List<SaveItem> itemsInDomeList = new List<SaveItem>();
        [Tooltip("A List that holds the save data for all the Items that will be loaded into the Inventory")]
        public List<SaveItem> itemsInInventoryList = new List<SaveItem>();


        [Header("Farming")]
        [Tooltip("A List that holds the save data for all the crops that will be loaded into the farm")]
        public List<SaveCrop> cropsPlanted = new List<SaveCrop>();

        [Header("Time")]
        [Tooltip("A reference to a class that stores the saved in game time")]
        public SaveTime time = new SaveTime();


    }
    /// <summary>
    /// A class that Extends Save Item to save all the impotent info about the hybrids
    /// </summary>
    [Serializable]
    public class SaveHybrid : SaveItem
    {
        [Tooltip("The Hybrids Name")]
        public string hybridName;
        [Tooltip("If The Hybrid Is Shiny")]
        public bool hybridShiny;
        [Tooltip("The Hybrids Happiness")]
        public int hybridHappiness;
    }

    /// <summary>
    /// A class that contains all the into that is needed to save an Item
    /// </summary>
    [Serializable]
    public class SaveItem
    {
        [Tooltip("The Items Save ID")]
        public int itemID;
        [Tooltip("The Items Position")]
        public Vector3 itemPosition;
        [Tooltip("The Items Rotation")]
        public Vector3 itemRotation;
        [Tooltip("The ItemsLocation Converted to an int")]  // Uses ItemLocation Enum
        public int itemSaveLocation;
        [Tooltip("The slot in the inventory that the item is currently in")] // Value will not be used if not in the inventory
        public int itemInventorySlot;
    }

    /// <summary>
    /// A Class that Extends SaveItem to save all the impotent information about crops
    /// </summary>
    [Serializable]
    public class SaveCrop : SaveItem
    {
        [Tooltip("The CropState Converted to Int")]  // uses CropState Enum
        public int growthStage;
    }

    /// <summary>
    /// A reference to the players current Scene and location and rotation.
    /// </summary>
    [Serializable]
    public class CurrentPlayer
    {
        [Tooltip("The Current Scene as a scriptible Enum")]
        public IntReference currentScene = new IntReference();       //The Current Scene as a scriptible Enum need to set this up still, at the moment its just an int
        [Tooltip("The Vector 3 reference to the players location")]
        public Vector3Reference location = new Vector3Reference();
        [Tooltip("The Vector 3 reference to the players rotation")]
        public Vector3Reference rotation = new Vector3Reference();
    }
    /// <summary>
    /// The stored saved player Scene , rotation and location.
    /// </summary>
    [Serializable]
    public class SavePlayer
    {
        [Tooltip("The saved CurrentScene as a int")]
        public int currentScene;
        [Tooltip("The saved location of the player as a vector3")]
        public Vector3 location;
        [Tooltip("The saved rotation of the player as a vector3")]
        public Vector3 rotation;
    }

    [Serializable]
    public class CurrentSettings
    {
        [Header("Player Settings")]
        public CurrentMoveSettings moveSettings = new CurrentMoveSettings();
        [Header("Audio Settings")]
        public CurrentAudioSettings audioSettings = new CurrentAudioSettings();
        [Header("Graphics Settings")]
        public CurrentGraphicSettings graphicSettings = new CurrentGraphicSettings();
        [Header("PostProsessing Settings")]
        public CurrentLanguageSettings languageSettings = new CurrentLanguageSettings();
    }

    [Serializable]
    public class SaveSettings
    {
        [Header("Player Settings")]
        public SaveMoveSettings moveSettings = new SaveMoveSettings();
        [Header("Audio Settings")]
        public SaveAudioSettings audioSettings = new SaveAudioSettings();
        [Header("Graphics Settings")]
        public SavedGraphicSettings graphicSettings = new SavedGraphicSettings();
        [Header("PostProsessing Settings")]
        public SavedLanguageSettings languageSettings = new SavedLanguageSettings();
    }
    [Serializable]
    public class CurrentMoveSettings
    {
        public IntReference turnType = new IntReference();
        public IntReference turnAngle = new IntReference();
        public FloatReference turnSpeed = new FloatReference();
    }
    [Serializable]
    public class SaveMoveSettings
    {
        public int turnType;
        public int turnAngle;
        public float turnSpeed;
    }
    [Serializable]
    public class CurrentAudioSettings
    {
        public FloatReference masterVolume = new FloatReference();
        public FloatReference musicVolume = new FloatReference();
        public FloatReference sFXVolume = new FloatReference();
        public FloatReference hybridVolume = new FloatReference();
        public FloatReference voicesVolume = new FloatReference();
    }

    [Serializable]
    public class SaveAudioSettings
    {
        public float masterVolume;
        public float musicVolume;
        public float sFXVolume;
        public float hybridVolume;
        public float voicesVolume;
    }
    [Serializable]
    public class CurrentGraphicSettings
    {
        public IntReference gameQuality = new IntReference();
        public BoolReference postProcessing = new BoolReference();
        public FloatReference bloomAmount = new FloatReference();
    }
    public class SavedGraphicSettings
    {
        public int gameQuality;
        public bool postProcessing;
        public float bloomAmount;
    }
    [Serializable]
    public class CurrentLanguageSettings
    {
        public IntReference language = new IntReference();
        public FloatReference fontSize = new FloatReference();
    }

    [Serializable]
    public class SavedLanguageSettings
    {
        public int language;
        public float fontSize;
    }
    /// <summary>
    /// This Class stores the TimeData For FOEDataThatHasBeenLoaded 
    /// </summary>
    [Serializable]
    public class CurrentTime
    {
        [Tooltip("The Current Day, how many days the player has been on Europa")]
        public IntReference days = new IntReference();
        [Tooltip("The Current Hour, how many hours the player has been on Europa")]
        public IntReference hours = new IntReference();
        [Tooltip("The Current minute, how many minutes the player has been on Europa")]
        public IntReference minutes = new IntReference();
    }
    /// <summary>
    /// This Class stores the time data for FOEDataFromSave 
    /// </summary>
    [Serializable]
    public class SaveTime
    {
        [Tooltip("The Current Saved Day, how many days the player has been on Europa")]
        public int days;
        [Tooltip("The Current Saved Hour, how many hours the player has been on Europa")]
        public int hours;
        [Tooltip("The Current Saved Minute, how many Minutes the player has been on Europa")]
        public int minutes;
    }
}



