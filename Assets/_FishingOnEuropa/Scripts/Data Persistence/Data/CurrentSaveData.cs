using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obvious.Soap;
using UnityEditor;
using Europa;




[Serializable]
public class SaveHybrid : SaveItem
{
    public string hybridName;
    public bool hybridShiny;
    public int hybridHappiness;
}

[Serializable]
public class SaveItem
{
    public int itemID;
    public Vector3 itemPosition;
    public Vector3 itemRotation;
    public int itemSaveLocation;
    public int itemInventorySlot;
}

[Serializable]
public class SaveCrop : SaveItem
{
    public int growthStage;
}

[Serializable]
public class CurrentPlayer
{
    public IntReference currentScene = new IntReference();
    public Vector3Reference location = new Vector3Reference();
    public Vector3Reference rotation = new Vector3Reference();
}
[Serializable]
public class SavePlayer
{
    public int currentScene;
    public Vector3 location;
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

[Serializable]
public class CurrentTime
{
    public IntReference days = new IntReference();
    public IntReference hours = new IntReference();
    public IntReference minutes = new IntReference();
}

[Serializable]
public class SaveTime
{
    public int days;
    public int hours;
    public int minutes;
}
/// <summary>
/// Soap
/// </summary>
[Serializable]
public class CurrentSaveData
{
    [Header("Saved Data")]
    public StringReference saveName = new StringReference();
    public StringReference saveDate = new StringReference();

    [Header("Hybrids")]
    public ScriptableListFOESaveItem_Hybrid hybridFarmList;
    public ScriptableListFOESaveItem_Hybrid hybridDomeList;
    public ScriptableListFOESaveItem_Hybrid hybridsInventoryList;


    [Header("PlayerSaveData")]
    public CurrentPlayer player = new CurrentPlayer();

    [Header("Settings")]
    public CurrentSettings settings = new CurrentSettings();

    [Header("Items")]
   
    public ScriptableListFOESaveItem itemsInFarmList;
    public ScriptableListFOESaveItem itemsInDomeList;
    public ScriptableListFOESaveItem itemsInInventoryList;


    [Header("Farming")]
    public ScriptableListFOESaveItem_Crop cropsPlanted;

    [Header("Time")]
    public CurrentTime time = new CurrentTime();
}
/// <summary>
/// Base 
/// </summary>
[Serializable]
public class SavedSaveData
{
    [Header("Saved Data")]
    public string saveName;
    public string saveDate;

    [Header("Hybrids")]
    public List<SaveHybrid> hybridFarmList = new List<SaveHybrid>();
    public List<SaveHybrid> hybridDomeList = new List<SaveHybrid>();
    public List<SaveHybrid> hybridsInventoryList = new List<SaveHybrid>();


    [Header("PlayerSaveData")]
    public SavePlayer player = new SavePlayer();

    [Header("Settings")]
    public SaveSettings settings = new SaveSettings();

    [Header("Items")]
    public List<SaveItem> itemsInFarmList = new List<SaveItem>();
    public List<SaveItem> itemsInDomeList = new List<SaveItem>();
    public List<SaveItem> itemsInInventoryList = new List<SaveItem>();


    [Header("Farming")]
    public List<SaveCrop> cropsPlanted = new List<SaveCrop>();

    [Header("Time")]
    public SaveTime time = new SaveTime();


}
