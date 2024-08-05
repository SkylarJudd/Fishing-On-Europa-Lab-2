using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor.SearchService;
using UnityEngine;

[Serializable]
public class SaveData
{

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
    public bool turntype;
    public int turnAngle;
    public float masterVolume;
    public float musicVolume;
    public float ceatureVolume;
    public float SFXVolume;

    [Header("Inventory")]
    public List<int> HybridsInInventory = new List<int>();
    public List<int> ItemInInventory = new List<int>();

    [Header("Farming")]
    public List<int> itemsInFarm = new List<int>();
    public List<Vector3> itemLocations = new List<Vector3>();
    public List<int> growthStage = new List<int>();

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
    [SerializeField, Tooltip("The max Number of saves the player can have at any time, keep in single Digits")]
    private int maxSaves;
    [SerializeField, Tooltip("defult Name For Worlds")]
    private string fileName = "World1";
    [SerializeField, Tooltip("The subdirectory for the save file")]
    private string Exstention = ".FOE";
    [SerializeField, Tooltip("The subdirectory for the save file")]
    private string subDir = "Save";
    [SerializeField, Tooltip("Do we want to use Encryption")]
    private bool useEncryption = true;
    [SerializeField, Tooltip("The array of bytes we will use for our encryption key")]
    private byte[] cryptoKey = { 0xF7, 0x24, 0x94, 0x08, 0x71, 0xE9, 0x64, 0x51, 0xC3, 0x5B, 0x84, 0x60, 0xCC, 0x55, 0x12, 0x76 };
    [SerializeField, Tooltip("Date Format")]
    public static string dateFormat = "yyyy-MM-dd HH:mm:ss zzz";

    /// <summary>
    /// Gets the path of where the application is installed
    /// </summary>
    /// <returns>The path of the games instal location</returns>
    private string GetPath(int _Index) => Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/" + subDir + "/" + fileName + Exstention + _Index;

    /// <summary>
    /// Makes a Time Stamp for the current Time
    /// </summary>
    /// <returns></returns>
    private string MakeTimestampNow() => DateTime.Now.ToString(dateFormat);


    void OnApplicationQuit()
    {
        Save();
    }

    void OnApplicationFocus(bool appInFocus)
    {
        if (!appInFocus)
            Save();
    }

    public bool NewSave()
    {
        if (saveDatas.Count >= maxSaves)
        {
            return false;
        }

        currentSaveIndex = saveDatas.Count + 1;
        //Load game data
        currentSave = LoadDataObject<SaveData>(currentSaveIndex);

        // Initialize new game data
        currentSave = new SaveData();
        Debug.Log("New Game Save Created");

        InitializeNewSave();

        return true;

    }
    /// <summary>
    /// Sets the starting Values for a save
    /// </summary>
    private void InitializeNewSave()
    {
        currentSave.HybridsInFarm = new List<int>();
        currentSave.HybridsShiney = new List<bool>();
        currentSave.HybridsNames = new List<string>();
        currentSave.HybridTank = new List<int>();
        currentSave.HybridLastPos = new List<Vector3>();

        currentSave.currentScene = Scenes._TUTORIAL.ToString();
        currentSave.playerLocation = Vector3.zero;

        currentSave.turntype = true;
        currentSave.turnAngle = 15;
        currentSave.masterVolume = 50;
        currentSave.musicVolume = 50;
        currentSave.ceatureVolume = 50;
        currentSave.SFXVolume = 50;

        currentSave.HybridsInInventory = new List<int>();
        currentSave.ItemInInventory = new List<int>();

        currentSave.itemsInFarm = new List<int>();
        currentSave.itemLocations = new List<Vector3>();
        currentSave.growthStage = new List<int>();

        currentSave.toysInWorld = new List<int>();
        currentSave.toysPosition = new List<Vector3>();
        currentSave.toysRotation = new List<Vector3>();

        currentSave.Days = 0;
        currentSave.Hours = 0;
        currentSave.Minuites = 0;

    }

    public void Load(int _saveIndex)
    {
        currentSave = LoadDataObject<SaveData>(_saveIndex);
        currentSaveIndex = _saveIndex;
        //call event to load all objects that need to be leaded
    }

    public void Save()
    {
        //call Event to save all objects that need to be saved
        SaveDataObject(currentSave, currentSaveIndex);
    }

    public void Delete(int _saveIndex)
    {
        DeleteDataObject(_saveIndex);
    }

    /// <summary>
    /// Loads our data as a GameDataObject type
    /// </summary>
    /// <typeparam name="T">The type of data to return</typeparam>
    /// <returns></returns>
    protected T LoadDataObject<T>(int _saveIndex) where T : SaveData
    {
        // Ensure that the file exists
        if (File.Exists(GetPath(_saveIndex)))
        {
            //Creates the File Stream for opening files
            FileStream stream = new FileStream(GetPath(_saveIndex), FileMode.Open);

            //Creates a stream reader and reads the stream
            StreamReader reader = new StreamReader(stream);

            //If we use encryption
            if (useEncryption)
            {
                // Create a new AES instance
                Aes aes = Aes.Create();

                // Set our encryption mode to Cipher Block Chain
                aes.Mode = CipherMode.CBC;

                //Create an array of correct size based on ASE IV
                byte[] outputIV = new byte[aes.IV.Length];

                // Read the IV from the file
                stream.Read(outputIV, 0, outputIV.Length);

                // Create cryptostream around the filestream
                CryptoStream cStream = new CryptoStream(stream, aes.CreateDecryptor(cryptoKey, outputIV), CryptoStreamMode.Read);

                // Update the reader with our cryptostream
                reader = new StreamReader(cStream);
            }

            //Read the entire file into a string value
            string jSave = reader.ReadToEnd();

            //Close the stream
            stream.Close();

            //Returns the string converted to json then to our GameData type
            return JsonUtility.FromJson<T>(jSave);
        }
        else
        {
            Debug.Log("Save file not found in " + GetPath(_saveIndex));
            return null;
        }
    }

    /// <summary>
    /// Saves our data object to disk
    /// </summary>
    /// <typeparam name="T">The data type</typeparam>
    /// <param name="data">The data object to save</param>
    protected void SaveDataObject<T>(T data , int _saveIndex) where T : SaveData
    {
        //Creates the save directory if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(GetPath(_saveIndex)));

        //Convert the This Game Data object into json then put into a string
        string jSave = JsonUtility.ToJson(data);

        //Create a filestream to create files
        FileStream stream = new FileStream(GetPath(_saveIndex), FileMode.Create);

        //Create our stream writer to write the data 
        StreamWriter writer = new StreamWriter(stream);

        // If we are using encryption
        if (useEncryption)
        {
            // Create a new AES instance
            Aes aes = Aes.Create();

            // Set our encryption mode to Cipher Block Chain
            aes.Mode = CipherMode.CBC;

            // Save newly generated IV
            byte[] inputIV = aes.IV;

            // Write the IV to the Filestream unencrypted
            stream.Write(inputIV, 0, inputIV.Length);

            // Create cryptostream wrapping filestream
            CryptoStream cStream = new CryptoStream(stream, aes.CreateEncryptor(cryptoKey, aes.IV), CryptoStreamMode.Write);

            // Create Streamwriter
            writer = new StreamWriter(cStream);

            // Write the innermost stream which we will encrypt
            writer.Write(jSave);

            //Close Streamwriter
            writer.Close();

            // Close Cryptostream
            cStream.Close();
        }
        else
        {
            //Write the data from the jSave string
            writer.Write(jSave);

            //Close the stream writer
            writer.Close();
        }

        //Close Filestream
        stream.Close();
    }

    /// <summary>
    /// Deletes our Game Data Object
    /// </summary>
    protected void DeleteDataObject(int _SaveIndex)
    {
        if (File.Exists(GetPath(_SaveIndex)))
        {
            Debug.Log("Deleting file " + fileName);
            File.Delete(GetPath(_SaveIndex));
        }
        else
        {
            Debug.Log("No file found in " + GetPath(_SaveIndex));
        }
    }

    private void FindAllSaves()
    {
        int saves = 0;
        for (int i = 0; i < maxSaves + 1; i++)
        {
            if (File.Exists(GetPath(i)))
            {
                saveDatas[i] = LoadDataObject<SaveData>(i);
                saves++;
            }
            else
            {
                break;
            }
        }

    }

}
