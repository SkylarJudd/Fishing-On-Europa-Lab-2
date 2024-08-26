using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
//using UnityEditor.SearchService;
using UnityEngine;

public class Audio
{
    [Header("Audio Settings")]
    public float masterVolume;
    public float musicVolume;
    public float sFXVolume;
    public float hybridVolume;
    public float voicesVolume;
}

[Serializable]
public class SaveData
{
    [Header("Saved Data")]
    public string saveName;
    public string saveDate;

    [Header("Farm Hybrid Save Data")]
    public List<int> HybridsInFarm = new List<int>();
    public List<bool> HybridsShiney = new List<bool>();
    public List<string> HybridsNames = new List<string>();
    public List<int> HybridTank = new List<int>();
    public List<Vector3> HybridLastPos = new List<Vector3>();

    [Header("PlayerSaveData")]
    public string currentScene;
    public Vector3 playerLocation;

    [Header("Player Settings")]
    public int turnType;
    public int turnAngle;
    public float turnSpeed;

    [Header("Audio Settings")]
    public float masterVolume;
    public float musicVolume;
    public float sFXVolume;
    public float hybridVolume;
    public float voicesVolume;
  
    [Header("Graphics Settings")]
    public int gameQuality;

    [Header("PostProsessing Settings")]
    public bool postProssessing;
    public float bloomAmout;

    [Header("Inventory")]
    public List<int> HybridsInInventory = new List<int>();
    public List<int> ItemInInventory = new List<int>();

    [Header("Farming")]
    public List<int> itemsInFarm = new List<int>();
    public List<Vector3> plantLocations = new List<Vector3>();
    public List<int> growthStage = new List<int>();
    public List<int> farmLocatedIn = new List<int>();

    [Header("Toys")]
    public List<int> toysInWorld = new List<int>();
    public List<Vector3> toysPosition = new List<Vector3>();
    public List<Vector3> toysRotation = new List<Vector3>();

    [Header("Time")]
    public int Days;
    public int Hours;
    public int Minuites;
}

public class TempSaveMannager : Singleton<TempSaveMannager>
{
    [Header("Saves")]
    public List<SaveData> saveDatas = new List<SaveData>();
    public SaveData currentSave;
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
        //base.Awake();
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
        currentSave = new SaveData();
        Debug.Log("New game save created");

        InitializeNewSave();
        Save();
        saveDatas.Add(LoadDataObject<SaveData>(currentSaveIndex));
        return true;
    }

    /// <summary>
    /// Sets the starting Values for a save
    /// </summary>
    private void InitializeNewSave()
    {
        print("InitializingSave");
        currentSave.saveName = fileName + currentSaveIndex;
        currentSave.saveDate = MakeTimestampNow();

        currentSave.HybridsInFarm = new List<int>();
        currentSave.HybridsShiney = new List<bool>();
        currentSave.HybridsNames = new List<string>();
        currentSave.HybridTank = new List<int>();
        currentSave.HybridLastPos = new List<Vector3>();

        currentSave.currentScene = Scenes._TUTORIAL.ToString();
        currentSave.playerLocation = Vector3.zero;

        currentSave.turnType = 1;
        currentSave.turnAngle = 30;
        currentSave.turnSpeed = 180;

        currentSave.masterVolume = 50;
        currentSave.musicVolume = 50;
        currentSave.hybridVolume = 50;
        currentSave.sFXVolume = 50;
        currentSave.voicesVolume = 50;

        currentSave.gameQuality = 1;

        currentSave.postProssessing = true;
        currentSave.bloomAmout = 0.1f;

        currentSave.HybridsInInventory = new List<int>();
        currentSave.ItemInInventory = new List<int>();

        currentSave.itemsInFarm = new List<int>();
        currentSave.plantLocations = new List<Vector3>();
        currentSave.growthStage = new List<int>();
        currentSave.farmLocatedIn = new List<int>();

    currentSave.toysInWorld = new List<int>();
        currentSave.toysPosition = new List<Vector3>();
        currentSave.toysRotation = new List<Vector3>();

        currentSave.Days = 0;
        currentSave.Hours = 0;
        currentSave.Minuites = 0;
    }
    /// <summary>
    /// Loads the save data from the specified save index
    /// </summary>
    /// <param name="_saveIndex">The index of the save file to load</param>
    public void Load(int saveIndex)
    {
        currentSave = LoadDataObject<SaveData>(saveIndex);
        currentSaveIndex = saveIndex;
        // Call event to load all objects that need to be loaded
    }
    /// <summary>
    /// Will save the data curretly stored in the current save to its corraponding file. 
    /// </summary>
    [ContextMenu("Save")]
    public void Save()
    {
        // Call event to save all objects that need to be saved
        SaveDataObject(currentSave, currentSaveIndex);
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
    protected T LoadDataObject<T>(int saveIndex) where T : SaveData
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

                    // Create cryptostream around the filestream
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
    protected void SaveDataObject<T>(T data, int saveIndex) where T : SaveData
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
                saveDatas.Add(LoadDataObject<SaveData>(i));
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
}
