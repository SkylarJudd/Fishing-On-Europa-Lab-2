using NaughtyAttributes;
using Obvious.Soap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;



public class SaveMannager : Singleton<SaveMannager>
{
    [Header("Saves")]
    public List<SavedSaveData> saveDatas = new List<SavedSaveData>();
    public CurrentSaveData currentSave;
    public int currentSaveIndex;

    [Header("SaveSettings")]
    [SerializeField, Tooltip("The max number of saves the player can have at any time, keep in single digits")]
    private int maxSaves = 5;
    [SerializeField, Tooltip("Default name for worlds")]
    private string fileName = "EuropaWorld";
    [SerializeField, Tooltip("The subdirectory for the save file")]
    private string extension = ".FOE";
    [SerializeField, Tooltip("The subdirectory for the save file")]
    private string subDir = "Save";
    [SerializeField, Tooltip("Do we want to use encryption")]
    private bool useEncryption;
    [SerializeField, Tooltip("The array of bytes we will use for our encryption key")]
    private byte[] cryptoKey = { 0xF7, 0x24, 0x94, 0x08, 0x71, 0xE9, 0x64, 0x51, 0xC3, 0x5B, 0x84, 0x60, 0xCC, 0x55, 0x12, 0x76 };
    [SerializeField, Tooltip("Date format")]
    public static string dateFormat = "yyyy-MM-dd HH:mm:ss zzz";
    /// <summary>
    /// Gets the path of where the application is installed
    /// </summary>
    /// <returns>The path of the games instal location</returns>
    private string GetPath(int index) => Path.Combine(Application.persistentDataPath, subDir, fileName + index + extension);
    /// <summary>
    /// Makes a Time Stamp for the current Time
    /// </summary>
    /// <returns></returns>
    private string MakeTimestampNow() => DateTime.Now.ToString(dateFormat);

    public void StartGame()
    {
        FindAllSaves();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnApplicationFocus(bool appInFocus)
    {
        if (!appInFocus)
            Save();
    }

    private void Update()
    {
        print("Hello");
    }

    /// <summary>
    /// Creates a new save file if under the maximum number of saves
    /// </summary>
    /// <returns>True if a new save is created, otherwise false</returns>
    [ContextMenu("NewSave")]
    public bool NewSave()
    {
        if (saveDatas.Count >= maxSaves)
        {
            return false;
        }

        currentSaveIndex = saveDatas.Count;
        SavedSaveData _newSave = new SavedSaveData();
        Debug.Log("New game save created");

        _newSave = InitializeNewSave(_newSave);
        currentSave = ConvertToCurretSave(_newSave);
        Save();
        saveDatas.Add(LoadDataObject<SavedSaveData>(currentSaveIndex));
        return true;
    }

    private void SetCurrentSave()
    {

    }

    /// <summary>
    /// Sets the starting Values for a save
    /// </summary>
    private SavedSaveData InitializeNewSave(SavedSaveData _NewSave)
    {
        print("InitializingSave");
        _NewSave.saveName = fileName + currentSaveIndex;
        _NewSave.saveDate = MakeTimestampNow();

        _NewSave.hybridFarmList = new List<SaveHybrid>();
        _NewSave.hybridDomeList = new List<SaveHybrid>();
        _NewSave.hybridsInventoryList = new List<SaveHybrid>();

        if (_NewSave.player == null)
        {
            Debug.LogError("_NewSave.player is null");
        }
        Debug.Log(Scenes._TUTORIAL); // Check if it's logging the correct value
        _NewSave.player.currentScene = (int)Scenes._TUTORIAL;
        _NewSave.player.location = Vector3.zero;

        _NewSave.settings.moveSettings.turnType = 1;
        _NewSave.settings.moveSettings.turnAngle = 30;
        _NewSave.settings.moveSettings.turnSpeed = 180;

        _NewSave.settings.audioSettings.masterVolume = 50;
        _NewSave.settings.audioSettings.musicVolume = 50;
        _NewSave.settings.audioSettings.hybridVolume = 50;
        _NewSave.settings.audioSettings.sFXVolume = 50;
        _NewSave.settings.audioSettings.voicesVolume = 50;

        _NewSave.settings.graphicSettings.gameQuality = 1;

        _NewSave.settings.graphicSettings.postProcessing = true;
        _NewSave.settings.graphicSettings.bloomAmount = 0.1f;

        _NewSave.itemsFarmList = new List<SaveItem>();
        _NewSave.itemsDomeList = new List<SaveItem>();
        _NewSave.itemsInventoryList = new List<SaveItem>();

        _NewSave.cropPlanted = new List<SaveCrop>();

        _NewSave.time.days = 0;
        _NewSave.time.hours = 0;
        _NewSave.time.minutes = 0;

        return _NewSave;
    }
    /// <summary>
    /// Loads the save data from the specified save index
    /// </summary>
    /// <param name="_saveIndex">The index of the save file to load</param>
    public void Load(int saveIndex)
    {

        SavedSaveData _Save = LoadDataObject<SavedSaveData>(saveIndex);
        currentSave = ConvertToCurretSave(_Save);

        currentSaveIndex = saveIndex;
        // Call event to load all objects that need to be loaded
    }
    /// <summary>
    /// Will save the data currently stored in the current save to its corresponding file. 
    /// </summary>
    [ContextMenu("Save")]
    public void Save()
    {
        // Call event to save all objects that need to be saved
        SavedSaveData _Save = ConvertToSavedSave(currentSave);
        SaveDataObject(_Save, currentSaveIndex);
    }
    /// <summary>
    /// Deletes the save file at the specified index
    /// </summary>
    /// <param name="_saveIndex">The index of the save file to delete</param>
    public void Delete(int saveIndex)
    {
        DeleteDataObject(saveIndex);
    }
    /// <summary>
    /// Loads our data as a GameDataObject type
    /// </summary>
    /// <typeparam name="T">The type of data to return</typeparam>
    /// <returns></returns>
    protected T LoadDataObject<T>(int saveIndex) where T : SavedSaveData
    {
        if (File.Exists(GetPath(saveIndex)))
        {
            print("FileFound");
            using (FileStream stream = new FileStream(GetPath(saveIndex), FileMode.Open))
            {
                StreamReader reader;
                if (useEncryption)
                {
                    // Create a new AES instance
                    Aes aes = Aes.Create();
                    aes.Mode = CipherMode.CBC;

                    // Create an array of correct size based on AES IV
                    byte[] outputIV = new byte[aes.IV.Length];

                    // Read the IV from the file
                    stream.Read(outputIV, 0, outputIV.Length);

                    // Create cryptostream around the file stream
                    using (CryptoStream cStream = new CryptoStream(stream, aes.CreateDecryptor(cryptoKey, outputIV), CryptoStreamMode.Read))
                    {
                        reader = new StreamReader(cStream);
                        string jSave = reader.ReadToEnd();
                        return JsonUtility.FromJson<T>(jSave);
                    }
                }
                else
                {
                    using (reader = new StreamReader(stream))
                    {
                        string jSave = reader.ReadToEnd();
                        return JsonUtility.FromJson<T>(jSave);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Save file not found in " + GetPath(saveIndex));
            return null;
        }
    }
    /// <summary>
    /// Saves our data object to disk
    /// </summary>
    /// <typeparam name="T">The data type</typeparam>
    /// <param name="data">The data object to save</param>
    protected void SaveDataObject<T>(T data, int saveIndex) where T : SavedSaveData
    {
        string path = GetPath(saveIndex);
        Debug.Log("Saving data to: " + path);

        Directory.CreateDirectory(Path.GetDirectoryName(GetPath(saveIndex)));

        string jSave = JsonUtility.ToJson(data);

        using (FileStream stream = new FileStream(GetPath(saveIndex), FileMode.Create))
        {
            if (useEncryption)
            {
                Aes aes = Aes.Create();
                aes.Mode = CipherMode.CBC;

                byte[] inputIV = aes.IV;
                stream.Write(inputIV, 0, inputIV.Length);

                using (CryptoStream cStream = new CryptoStream(stream, aes.CreateEncryptor(cryptoKey, aes.IV), CryptoStreamMode.Write))
                using (StreamWriter writer = new StreamWriter(cStream))
                {
                    writer.Write(jSave);
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(jSave);
                }
            }
        }
    }
    /// <summary>
    /// Deletes the data object at the specified index
    /// </summary>
    /// <param name="saveIndex">The index of the save file to delete</param>
    protected void DeleteDataObject(int saveIndex)
    {
        if (File.Exists(GetPath(saveIndex)))
        {
            Debug.Log("Deleting file " + GetPath(saveIndex));
            File.Delete(GetPath(saveIndex));
        }
        else
        {
            Debug.Log("No file found in " + GetPath(saveIndex));
        }
    }
    /// <summary>
    /// Finds all save data in our Directory
    /// </summary>
    private void FindAllSaves()
    {
        Debug.Log("Locating saves");
        for (int i = 0; i < maxSaves; i++)
        {
            if (File.Exists(GetPath(i)))
            {
                Debug.Log($"Save found: {GetPath(i)}");
                saveDatas.Add(LoadDataObject<SavedSaveData>(i));
            }
        }

        if (saveDatas.Count == 0)
        {
            NewSave();
        }
        else
        {
            Load(0);
        }
    }

    public void ResetSave()
    {
        saveDatas.Clear();
        currentSave = null;
        currentSaveIndex = -1;
    }

    private CurrentSaveData ConvertToCurretSave(SavedSaveData savedData)
    {
        CurrentSaveData _currentData = new CurrentSaveData();

            // Save Basic Information
            _currentData.saveName.Value = savedData.saveName;
            _currentData.saveDate.Value = savedData.saveDate;

            // Convert Player Data
            _currentData.player.currentScene.Value = savedData.player.currentScene;
            _currentData.player.location.Value = savedData.player.location;
            _currentData.player.rotation.Value = savedData.player.rotation;

            // Convert Settings
            _currentData.settings.moveSettings.turnType.Value = savedData.settings.moveSettings.turnType;
            _currentData.settings.moveSettings.turnAngle.Value = savedData.settings.moveSettings.turnAngle;
            _currentData.settings.moveSettings.turnSpeed.Value = savedData.settings.moveSettings.turnSpeed;

            _currentData.settings.audioSettings.masterVolume.Value = savedData.settings.audioSettings.masterVolume;
            _currentData.settings.audioSettings.musicVolume.Value = savedData.settings.audioSettings.musicVolume;
            _currentData.settings.audioSettings.sFXVolume.Value = savedData.settings.audioSettings.sFXVolume;
            _currentData.settings.audioSettings.hybridVolume.Value = savedData.settings.audioSettings.hybridVolume;
            _currentData.settings.audioSettings.voicesVolume.Value = savedData.settings.audioSettings.voicesVolume;

            _currentData.settings.graphicSettings.gameQuality.Value = savedData.settings.graphicSettings.gameQuality;
            _currentData.settings.graphicSettings.postProcessing.Value = savedData.settings.graphicSettings.postProcessing;
            _currentData.settings.graphicSettings.bloomAmount.Value = savedData.settings.graphicSettings.bloomAmount;

            _currentData.settings.languageSettings.language.Value = savedData.settings.languageSettings.language;
            _currentData.settings.languageSettings.fontSize.Value = savedData.settings.languageSettings.fontSize;

            // Convert Hybrids
            _currentData.hybridFarmList.Clear();
            foreach (var savedHybrid in savedData.hybridFarmList)
            {
                CurrentHybrid currentHybrid = new CurrentHybrid
                {
                    hybridItem = new CurrentItem
                    {
                        itemID = new IntReference(),
                        itemPosition = new Vector3Reference(),
                        itemRotation = new Vector3Reference(),
                        itemSaveLocation = new IntReference(),
                        itemInventorySlot = new IntReference(),
                        itemState = new IntReference()
                    },
                    hybridName = new StringReference(),
                    hybridShiny = new BoolReference()
                };

                currentHybrid.hybridItem.itemID.Value = savedHybrid.hybridItem.itemID;
                currentHybrid.hybridItem.itemPosition.Value = savedHybrid.hybridItem.itemPosition;
                currentHybrid.hybridItem.itemRotation.Value = savedHybrid.hybridItem.itemRotation;
                currentHybrid.hybridItem.itemSaveLocation.Value = savedHybrid.hybridItem.itemSaveLocation;
                currentHybrid.hybridItem.itemInventorySlot.Value = savedHybrid.hybridItem.itemInventorySlot;
                currentHybrid.hybridItem.itemState.Value = savedHybrid.hybridItem.itemState;
                currentHybrid.hybridName.Value = savedHybrid.hybridName;
                currentHybrid.hybridShiny.Value = savedHybrid.hybridShiny;



                _currentData.hybridFarmList.Add(currentHybrid);
            }

            // Convert Items
            _currentData.itemsFarmList.Clear();
            foreach (var savedItem in savedData.itemsFarmList)
            {
                CurrentItem currentItem = new CurrentItem
                {
                    itemID = new IntReference(),
                    itemPosition = new Vector3Reference(),
                    itemRotation = new Vector3Reference(),
                    itemSaveLocation = new IntReference(),
                    itemInventorySlot = new IntReference(),
                    itemState = new IntReference()
                };

                currentItem.itemID.Value = savedItem.itemID;
                currentItem.itemPosition.Value = savedItem.itemPosition;
                currentItem.itemRotation.Value = savedItem.itemRotation;
                currentItem.itemSaveLocation.Value = savedItem.itemSaveLocation;
                currentItem.itemInventorySlot.Value = savedItem.itemInventorySlot;
                currentItem.itemState.Value = savedItem.itemState;



                _currentData.itemsFarmList.Add(currentItem);
            }

            _currentData.itemsDomeList.Clear();
            foreach (var savedItem in savedData.itemsDomeList)
            {
                CurrentItem currentItem = new CurrentItem
                {
                    itemID = new IntReference(),
                    itemPosition = new Vector3Reference(),
                    itemRotation = new Vector3Reference(),
                    itemSaveLocation = new IntReference(),
                    itemInventorySlot = new IntReference(),
                    itemState = new IntReference()
                };

                currentItem.itemID.Value = savedItem.itemID;
                currentItem.itemPosition.Value = savedItem.itemPosition;
                currentItem.itemRotation.Value = savedItem.itemRotation;
                currentItem.itemSaveLocation.Value = savedItem.itemSaveLocation;
                currentItem.itemInventorySlot.Value = savedItem.itemInventorySlot;
                currentItem.itemState.Value = savedItem.itemState;




                _currentData.itemsDomeList.Add(currentItem);
            }

            // Convert Crops
            _currentData.cropPlanted.Clear();
            foreach (var savedCrop in savedData.cropPlanted)
            {
                CurrentCrop currentCrop = new CurrentCrop
                {
                    cropItem = new CurrentItem
                    {
                        itemID = new IntReference(),
                        itemPosition = new Vector3Reference(),
                        itemRotation = new Vector3Reference(),
                        itemSaveLocation = new IntReference(),
                        itemInventorySlot = new IntReference(),
                        itemState = new IntReference()
                    },
                    growthStage = new IntReference()
                };

                currentCrop.cropItem.itemID.Value = savedCrop.cropItem.itemID;
                currentCrop.cropItem.itemPosition.Value = savedCrop.cropItem.itemPosition;
                currentCrop.cropItem.itemRotation.Value = savedCrop.cropItem.itemRotation;
                currentCrop.cropItem.itemSaveLocation.Value = savedCrop.cropItem.itemSaveLocation;
                currentCrop.cropItem.itemInventorySlot.Value = savedCrop.cropItem.itemInventorySlot;
                currentCrop.cropItem.itemState.Value = savedCrop.cropItem.itemState;
                currentCrop.growthStage.Value = savedCrop.growthStage;
                   


                _currentData.cropPlanted.Add(currentCrop);
            }

            // Convert Time
            _currentData.time.days.Value = savedData.time.days;
            _currentData.time.hours.Value = savedData.time.hours;
            _currentData.time.minutes.Value = savedData.time.minutes;

            return _currentData;

    }

    private SavedSaveData ConvertToSavedSave(CurrentSaveData _currentData)
    {
        SavedSaveData _savedData = new SavedSaveData();


        // Save Basic Information
        _savedData.saveName = _currentData.saveName.Value;
        _savedData.saveDate = _currentData.saveDate.Value;

        // Convert Player Data
        _savedData.player.currentScene = _currentData.player.currentScene.Value;
        _savedData.player.location = _currentData.player.location.Value;
        _savedData.player.rotation = _currentData.player.rotation.Value;

        // Convert Settings
        _savedData.settings.moveSettings.turnType = _currentData.settings.moveSettings.turnType.Value;
        _savedData.settings.moveSettings.turnAngle = _currentData.settings.moveSettings.turnAngle.Value;
        _savedData.settings.moveSettings.turnSpeed = _currentData.settings.moveSettings.turnSpeed.Value;

        _savedData.settings.audioSettings.masterVolume = _currentData.settings.audioSettings.masterVolume.Value;
        _savedData.settings.audioSettings.musicVolume = _currentData.settings.audioSettings.musicVolume.Value;
        _savedData.settings.audioSettings.sFXVolume = _currentData.settings.audioSettings.sFXVolume.Value;
        _savedData.settings.audioSettings.hybridVolume = _currentData.settings.audioSettings.hybridVolume.Value;
        _savedData.settings.audioSettings.voicesVolume = _currentData.settings.audioSettings.voicesVolume.Value;

        _savedData.settings.graphicSettings.gameQuality = _currentData.settings.graphicSettings.gameQuality.Value;
        _savedData.settings.graphicSettings.postProcessing = _currentData.settings.graphicSettings.postProcessing.Value;
        _savedData.settings.graphicSettings.bloomAmount = _currentData.settings.graphicSettings.bloomAmount.Value;

        _savedData.settings.languageSettings.language = _currentData.settings.languageSettings.language.Value;
        _savedData.settings.languageSettings.fontSize = _currentData.settings.languageSettings.fontSize.Value;

        // Convert Hybrids
        _savedData.hybridFarmList.Clear();
        foreach (var currentHybrid in _currentData.hybridFarmList)
        {
            SaveHybrid savedHybrid = new SaveHybrid
            {
                hybridItem = new SaveItem
                {
                    itemID = currentHybrid.hybridItem.itemID.Value,
                    itemPosition = currentHybrid.hybridItem.itemPosition.Value,
                    itemRotation = currentHybrid.hybridItem.itemRotation.Value,
                    itemSaveLocation = currentHybrid.hybridItem.itemSaveLocation.Value,
                    itemInventorySlot = currentHybrid.hybridItem.itemInventorySlot.Value,
                    itemState = currentHybrid.hybridItem.itemState.Value
                },
                hybridName = currentHybrid.hybridName.Value,
                hybridShiny = currentHybrid.hybridShiny.Value
            };
            _savedData.hybridFarmList.Add(savedHybrid);
        }

        // Convert Items
        _savedData.itemsFarmList.Clear();
        foreach (var currentItem in _currentData.itemsFarmList)
        {
            SaveItem savedItem = new SaveItem
            {
                itemID = currentItem.itemID.Value,
                itemPosition = currentItem.itemPosition.Value,
                itemRotation = currentItem.itemRotation.Value,
                itemSaveLocation = currentItem.itemSaveLocation.Value,
                itemInventorySlot = currentItem.itemInventorySlot.Value,
                itemState = currentItem.itemState.Value
            };
            _savedData.itemsFarmList.Add(savedItem);
        }

        _savedData.itemsDomeList.Clear();
        foreach (var currentItem in _currentData.itemsDomeList)
        {
            SaveItem savedItem = new SaveItem
            {
                itemID = currentItem.itemID.Value,
                itemPosition = currentItem.itemPosition.Value,
                itemRotation = currentItem.itemRotation.Value,
                itemSaveLocation = currentItem.itemSaveLocation.Value,
                itemInventorySlot = currentItem.itemInventorySlot.Value,
                itemState = currentItem.itemState.Value
            };
            _savedData.itemsDomeList.Add(savedItem);
        }

        // Convert Crops
        _savedData.cropPlanted.Clear();
        foreach (var currentCrop in _currentData.cropPlanted)
        {
            SaveCrop savedCrop = new SaveCrop
            {
                cropItem = new SaveItem
                {
                    itemID = currentCrop.cropItem.itemID.Value,
                    itemPosition = currentCrop.cropItem.itemPosition.Value,
                    itemRotation = currentCrop.cropItem.itemRotation.Value,
                    itemSaveLocation = currentCrop.cropItem.itemSaveLocation.Value,
                    itemInventorySlot = currentCrop.cropItem.itemInventorySlot.Value,
                    itemState = currentCrop.cropItem.itemState.Value
                },
                growthStage = currentCrop.growthStage.Value
            };
            _savedData.cropPlanted.Add(savedCrop);
        }

        // Convert Time
        _savedData.time.days = _currentData.time.days.Value;
        _savedData.time.hours = _currentData.time.hours.Value;
        _savedData.time.minutes = _currentData.time.minutes.Value;

        return _savedData;
    }
}




