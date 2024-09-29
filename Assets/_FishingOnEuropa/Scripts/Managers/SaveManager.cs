
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;



// This is the save manager It manages all the save data Inside the game and is responsible for saving and loading files from the players computer
// If you would like more info in the SaveManager you can find documentation at https://app.clickup.com/9014574275/v/dc/8cmyz63-1474/8cmyz63-2114


namespace Europa
{
    public class SaveManager : Singleton<SaveManager>
    {
        [Header("Saves")]
        [Tooltip("A List that holds all the Stored Save Data once they are loaded")] //SaveData is a list of SavedSaveData that is a class that holds all the data in a form that can be serialized by the Jason converter. 
        public List<FOEDataFromSave> saveDatas = new List<FOEDataFromSave>();
        [Tooltip("A Class that holds the Stored Save Data thats been loaded from one of the save data's")] // this is a class of CurrentSaveData that holds information about all the saved items in the game within a soap format, allowing them to be accessed where they are needed and saved without ever needing to communicate with the save manager. 
        public FOEDataThatHasBeenLoaded currentSave;
        [Tooltip("In index that stores the current save")] //current save Index is Used to store the Current File that is being used. This number starts at one, so if you need to access the array make sure to -1 from this values to get its location. 
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

        /// <summary>
        /// Called at the start of the game Though An event Listener attached to this GameObject.
        /// </summary>
        public void StartGame()
        {

            FindAllSaves();
        }

        /// <summary>
        /// Called at the End of the game and when the game is unfocused though an event listener attached to this GameObject.
        /// </summary>
        public void SaveGame()
        {
            if (saveDatas.Count == 0 || saveDatas == null)
                return;

            Save();
        }


        /// <summary>
        /// REMOVE AS SOON AS THE ABOVE IS SET UP!!
        /// </summary>
        private void OnApplicationQuit()
        {
            if(saveDatas.Count == 0 || saveDatas == null)
                return;

            Save();
        }

        private void OnApplicationFocus(bool appInFocus)
        {
            //if (!appInFocus)
            //    Save();
        }


        /// <summary>
        /// Creates a new save file if under the maximum number of saves
        /// </summary>
        /// <returns>True if a new save is created, otherwise false</returns>
        [ContextMenu("NewSave")]
        public bool NewSave()
        {
            //Checks to see if the Max file size is equal to or has exceeded the limit. If it has it means the player has created a new save inside the save file, we need to find a way to warn the player about this. 
            if (saveDatas.Count >= maxSaves)
            {
                return false;
            }

            currentSaveIndex = saveDatas.Count;                     //Sets the SaveIndex to the current number of saves so when a new save is made its numbered after the current max file. 
            FOEDataFromSave _newSave = new FOEDataFromSave();           //Creates a new SavedSaveData class as a container for the new save. 
            Debug.Log("New game save created");

            _newSave = InitializeNewSave(_newSave);                 //Initialize The newSave with the default Values.                            

            saveDatas.Add(_newSave);                                //Adds the New save to the saveDatas
            LoadData(_newSave);                                     //Loads the New Nave into the current save data

            Save();                                                 //Saves the new File to the computers files. 

            return true;
        }

       

        /// <summary>
        /// Initialize A new save file setting the default values that a new player will start with. 
        /// </summary>
        private FOEDataFromSave InitializeNewSave(FOEDataFromSave _NewSave)
        {
            print("InitializingSave");
            _NewSave.saveName = fileName + currentSaveIndex;
            _NewSave.saveDate = MakeTimestampNow();                         //Calls the MakeTimeStampNow that returns a string value that holds the current Timestamp

            _NewSave.hybridFarmList = new List<SaveHybrid>();
            _NewSave.hybridDomeList = new List<SaveHybrid>();
            _NewSave.hybridsInventoryList = new List<SaveHybrid>();

            _NewSave.player.currentScene = (int)CurrentScenes._TUTORIAL;
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

            _NewSave.itemsInFarmList = new List<SaveItem>();
            _NewSave.itemsInDomeList = new List<SaveItem>();
            _NewSave.itemsInInventoryList = new List<SaveItem>();

            _NewSave.cropsPlanted = new List<SaveCrop>();

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

            FOEDataFromSave _Save = LoadDataObject<FOEDataFromSave>(saveIndex);
            LoadData(_Save);

            currentSaveIndex = saveIndex;
            
        }

        /// <summary>
        /// Will save the data currently stored in the current save to its corresponding file. 
        /// </summary>
        [ContextMenu("Save")]
        public void Save()
        {
            // Call event to save all objects that need to be saved
            SaveData();
            SaveDataObject(saveDatas[currentSaveIndex], currentSaveIndex);
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
        protected T LoadDataObject<T>(int saveIndex) where T : FOEDataFromSave
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
        protected void SaveDataObject<T>(T data, int saveIndex) where T : FOEDataFromSave
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
        /// Finds all save data in our Directory and loads them into the saveDatas List
        /// </summary>
        private void FindAllSaves()
        {
            Debug.Log("Locating saves");
            for (int i = 0; i < maxSaves; i++)
            {
                //Checks to see if the File exists with an index of 1
                if (File.Exists(GetPath(i)))
                {
                    Debug.Log($"Save found: {GetPath(i)}");
                    saveDatas.Add(LoadDataObject<FOEDataFromSave>(i));
                }
            }

            //Do we need to check if the player has more then the max number of saves? If so we should do it here. 

            
            if (saveDatas.Count == 0) //Creates a new save if there is no saves loaded first time entering the game. 
            {
                NewSave();
            }
            else // Loads the first save in the list, This will need to be changed to another file, that contains info about the last played file, and the temp audio settings from that last file. 
            {
                Load(0);
            }
        }

        //If you wish to add a new object to the save go to these docs. https://app.clickup.com/9014574275/v/dc/8cmyz63-1474/8cmyz63-6174
        /// <summary>
        /// This loads the Data from the SavedSaveData to the Current Save Data
        /// </summary>
        /// <param name="savedData"></param>
        private void LoadData(FOEDataFromSave savedData)
        {

            // Load Basic Information
            currentSave.saveName.Value = savedData.saveName;
            currentSave.saveDate.Value = savedData.saveDate;

            // Load Player Data
            currentSave.player.currentScene.Value = savedData.player.currentScene;
            currentSave.player.location.Value = savedData.player.location;
            currentSave.player.rotation.Value = savedData.player.rotation;

            // Load Settings
            currentSave.settings.moveSettings.turnType.Value = savedData.settings.moveSettings.turnType;
            currentSave.settings.moveSettings.turnAngle.Value = savedData.settings.moveSettings.turnAngle;
            currentSave.settings.moveSettings.turnSpeed.Value = savedData.settings.moveSettings.turnSpeed;

            currentSave.settings.audioSettings.masterVolume.Value = savedData.settings.audioSettings.masterVolume;
            currentSave.settings.audioSettings.musicVolume.Value = savedData.settings.audioSettings.musicVolume;
            currentSave.settings.audioSettings.sFXVolume.Value = savedData.settings.audioSettings.sFXVolume;
            currentSave.settings.audioSettings.hybridVolume.Value = savedData.settings.audioSettings.hybridVolume;
            currentSave.settings.audioSettings.voicesVolume.Value = savedData.settings.audioSettings.voicesVolume;

            currentSave.settings.graphicSettings.gameQuality.Value = savedData.settings.graphicSettings.gameQuality;
            currentSave.settings.graphicSettings.postProcessing.Value = savedData.settings.graphicSettings.postProcessing;
            currentSave.settings.graphicSettings.bloomAmount.Value = savedData.settings.graphicSettings.bloomAmount;

            currentSave.settings.languageSettings.language.Value = savedData.settings.languageSettings.language;
            currentSave.settings.languageSettings.fontSize.Value = savedData.settings.languageSettings.fontSize;

            //Load Time

            currentSave.time.days.Value = savedData.time.days;
            currentSave.time.hours.Value = savedData.time.hours;
            currentSave.time.minutes.Value = savedData.time.minutes;

            //Load Hybrids

            foreach (SaveHybrid _savedHybrid in savedData.hybridFarmList)
            {
                FOESaveItem_Hybrid _loadHybrid = new FOESaveItem_Hybrid();
                currentSave.hybridFarmList.Add(SetHybridData(_loadHybrid, _savedHybrid));
            }

            foreach (SaveHybrid _savedHybrid in savedData.hybridDomeList)
            {
                FOESaveItem_Hybrid _loadHybrid = new FOESaveItem_Hybrid();
                currentSave.hybridDomeList.Add(SetHybridData(_loadHybrid, _savedHybrid));
            }

            foreach (SaveHybrid _savedHybrid in savedData.hybridsInventoryList)
            {
                FOESaveItem_Hybrid _loadHybrid = new FOESaveItem_Hybrid();
                currentSave.hybridsInventoryList.Add(SetHybridData(_loadHybrid, _savedHybrid));
            }

            // Load Items

            foreach (SaveItem _savedItem in savedData.itemsInFarmList)
            {
                FOESaveItem _loadItem = new FOESaveItem();
                currentSave.itemsInFarmList.Add(SetItemData(_loadItem, _savedItem));
            }
            foreach (SaveItem _savedItem in savedData.itemsInDomeList)
            {
                FOESaveItem _loadItem = new FOESaveItem();
                currentSave.itemsInDomeList.Add(SetItemData(_loadItem, _savedItem));
            }
            foreach (SaveItem _savedItem in savedData.itemsInInventoryList)
            {
                FOESaveItem _loadItem = new FOESaveItem();
                currentSave.itemsInInventoryList.Add(SetItemData(_loadItem, _savedItem));
            }

            //Load Crops

            foreach (SaveCrop _savedItem in savedData.cropsPlanted)
            {
                FOESaveItem_Crop _loadItem = new FOESaveItem_Crop();
                currentSave.cropsPlanted.Add(SetCropData(_loadItem, _savedItem));
            }


        }

        /// <summary>
        /// Converts a FOESaveItem To a SaveHybrid The FOESaveItem_Hybrid is found in a scriptible list that contains a list of FOESaveItem_Hybrids
        /// </summary>
        /// <param name="_loadHybrid"></param>
        /// <param name="_savedHybrid"></param>
        /// <returns></returns>
        private FOESaveItem_Hybrid SetHybridData(FOESaveItem_Hybrid _loadHybrid, SaveHybrid _savedHybrid)
        {
            _loadHybrid.itemID = _savedHybrid.itemID;
            _loadHybrid.hybridName = _savedHybrid.hybridName;
            _loadHybrid.hybridShiny = _savedHybrid.hybridShiny;
            _loadHybrid.itemPos.position = _savedHybrid.itemPosition;
            _loadHybrid.itemPos.rotation = Quaternion.Euler(_savedHybrid.itemRotation);
            _loadHybrid.itemLocation = (ItemLocation)_savedHybrid.itemSaveLocation;
            _loadHybrid.itemInventorySlot = _savedHybrid.itemInventorySlot;

            return _loadHybrid;
        }

        private FOESaveItem SetItemData(FOESaveItem _loadItem, SaveItem _savedItem)
        {
            _loadItem.itemID = _savedItem.itemID;
            _loadItem.itemPos.position = _savedItem.itemPosition;
            _loadItem.itemPos.rotation = Quaternion.Euler(_savedItem.itemRotation);
            _loadItem.itemLocation = (ItemLocation)_savedItem.itemSaveLocation;
            _loadItem.itemInventorySlot = _savedItem.itemInventorySlot;

            return _loadItem;
        }

        private FOESaveItem_Crop SetCropData(FOESaveItem_Crop _loadItem, SaveCrop _savedItem)
        {
            _loadItem.itemID = _savedItem.itemID;
            _loadItem.itemPos.position = _savedItem.itemPosition;
            _loadItem.itemPos.rotation = Quaternion.Euler(_savedItem.itemRotation);
            _loadItem.growthStage = (CropState)_savedItem.growthStage;
            _loadItem.itemLocation = (ItemLocation)_savedItem.itemSaveLocation;

            return _loadItem;
        }

        private void SaveData()
        {
            FOEDataFromSave saveData = saveDatas[currentSaveIndex];

            // Save Basic Information
            saveData.saveName = currentSave.saveName.Value;
            saveData.saveDate = currentSave.saveDate.Value;

            // Save Player Data
            saveData.player.currentScene = currentSave.player.currentScene.Value;
            saveData.player.location = currentSave.player.location.Value;
            saveData.player.rotation = currentSave.player.rotation.Value;

            // Save Settings
            saveData.settings.moveSettings.turnType = currentSave.settings.moveSettings.turnType.Value;
            saveData.settings.moveSettings.turnAngle = currentSave.settings.moveSettings.turnAngle.Value;
            saveData.settings.moveSettings.turnSpeed = currentSave.settings.moveSettings.turnSpeed.Value;

            saveData.settings.audioSettings.masterVolume = currentSave.settings.audioSettings.masterVolume.Value;
            saveData.settings.audioSettings.musicVolume = currentSave.settings.audioSettings.musicVolume.Value;
            saveData.settings.audioSettings.sFXVolume = currentSave.settings.audioSettings.sFXVolume.Value;
            saveData.settings.audioSettings.hybridVolume = currentSave.settings.audioSettings.hybridVolume.Value;
            saveData.settings.audioSettings.voicesVolume = currentSave.settings.audioSettings.voicesVolume.Value;

            saveData.settings.graphicSettings.gameQuality = currentSave.settings.graphicSettings.gameQuality.Value;
            saveData.settings.graphicSettings.postProcessing = currentSave.settings.graphicSettings.postProcessing.Value;
            saveData.settings.graphicSettings.bloomAmount = currentSave.settings.graphicSettings.bloomAmount.Value;

            saveData.settings.languageSettings.language = currentSave.settings.languageSettings.language.Value;
            saveData.settings.languageSettings.fontSize = currentSave.settings.languageSettings.fontSize.Value;

            // Save Time
            saveData.time.days = currentSave.time.days.Value;
            saveData.time.hours = currentSave.time.hours.Value;
            saveData.time.minutes = currentSave.time.minutes.Value;

            // Save Hybrids
            saveData.hybridFarmList.Clear();
            foreach (FOESaveItem_Hybrid currentHybrid in currentSave.hybridFarmList)
            {
                SaveHybrid savedHybrid = SetHybridSaveData(currentHybrid);
                saveData.hybridFarmList.Add(savedHybrid);
            }

            saveData.hybridDomeList.Clear();
            foreach (FOESaveItem_Hybrid currentHybrid in currentSave.hybridDomeList)
            {
                SaveHybrid savedHybrid = SetHybridSaveData(currentHybrid);
                saveData.hybridDomeList.Add(savedHybrid);
            }

            saveData.hybridsInventoryList.Clear();
            foreach (FOESaveItem_Hybrid currentHybrid in currentSave.hybridsInventoryList)
            {
                SaveHybrid savedHybrid = SetHybridSaveData(currentHybrid);
                saveData.hybridsInventoryList.Add(savedHybrid);
            }

            // Save Items
            saveData.itemsInFarmList.Clear();
            foreach (FOESaveItem currentItem in currentSave.itemsInFarmList)
            {
                SaveItem savedItem = SetItemSaveData(currentItem);
                saveData.itemsInFarmList.Add(savedItem);
            }

            saveData.itemsInDomeList.Clear();
            foreach (FOESaveItem currentItem in currentSave.itemsInDomeList)
            {
                SaveItem savedItem = SetItemSaveData(currentItem);
                saveData.itemsInDomeList.Add(savedItem);
            }

            saveData.itemsInInventoryList.Clear();
            foreach (FOESaveItem currentItem in currentSave.itemsInInventoryList)
            {
                SaveItem savedItem = SetItemSaveData(currentItem);
                saveData.itemsInInventoryList.Add(savedItem);
            }

            // Save Crops
            saveData.cropsPlanted.Clear();
            foreach (FOESaveItem_Crop currentCrop in currentSave.cropsPlanted)
            {
                SaveCrop savedCrop = SetCropSaveData(currentCrop);
                saveData.cropsPlanted.Add(savedCrop);
            }
        }

        private SaveHybrid SetHybridSaveData(FOESaveItem_Hybrid currentHybrid)
        {
            SaveHybrid savedHybrid = new SaveHybrid
            {
                
                itemID = currentHybrid.itemID,
                itemPosition = currentHybrid.itemPos.position,
                itemRotation = currentHybrid.itemPos.rotation.eulerAngles,
                itemSaveLocation = (int)currentHybrid.itemLocation,
                itemInventorySlot = currentHybrid.itemInventorySlot,
                hybridName = currentHybrid.hybridName,
                hybridShiny = currentHybrid.hybridShiny
            };

            return savedHybrid;
        }
    
        

        private SaveItem SetItemSaveData(FOESaveItem currentItem)
        {
            SaveItem savedItem = new SaveItem
            {
                itemID = currentItem.itemID,
                itemPosition = currentItem.itemPos.position,
                itemRotation = currentItem.itemPos.rotation.eulerAngles,
                itemSaveLocation = (int)currentItem.itemLocation,
                itemInventorySlot = currentItem.itemInventorySlot,

            };

            return savedItem;
        }

        private SaveCrop SetCropSaveData(FOESaveItem_Crop currentCrop)
        {
            SaveCrop savedCrop = new SaveCrop
            {

                itemID = currentCrop.itemID,
                itemPosition = currentCrop.itemPos.position,
                itemRotation = currentCrop.itemPos.rotation.eulerAngles,
                itemSaveLocation = (int)currentCrop.itemLocation,
                growthStage = (int)currentCrop.growthStage,
                
            };
                

            return savedCrop;
        }
    }
}









