//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;

//public class DataPersistenceManager : MonoBehaviour
//{
//    [Header("File Storage Config")]
//    [SerializeField] private string fileName;
//    [SerializeField] private bool useEncryption;

//    private GameData gameData;

//    private List<IDataPersistence> dataPersistenceObjects;

//    private FileDataHandler dataHandler;

//    public static DataPersistenceManager instance { get; private set; }

//    private void Awake()
//    {
//        if (instance != null)
//        {
//            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
//        }
//        instance = this;
//    }

//    private void Start()
//    {
//        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
//        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
//        LoadGame();
//    }

//    public void NewGame()
//    {
//        this.gameData = new GameData();
//    }

//    public void LoadGame()
//    {
//        //Load any saved from a file using the data Handler
//        this.gameData = dataHandler.Load();

//        //if no data to load, initialise to a new game
//        if (this.gameData == null)
//        {
//            Debug.Log("No data was found. Initialising data to defults.");
//            NewGame();
//        }
//        //TODO - push the loaded data to all other scripts that need it
//        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
//        {
//            dataPersistenceObj.LoadData(gameData);
//        }
//    }

//    public void SaveGame()
//    {
//        // pass the data to other scripts so they can update it
//        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
//        {
//            dataPersistenceObj.SaveData(ref gameData);
//        }

//        // Save that data to a file using the data handler
//        dataHandler.Save(gameData);
//    }

//    private void OnApplicationQuit()
//    {
//        SaveGame();
//    }

//    private List<IDataPersistence> FindAllDataPersistenceObjects()
//    {
//        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

//        return new List<IDataPersistence>(dataPersistenceObjects);
//    }
//}
