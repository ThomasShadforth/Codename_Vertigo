using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    //Tools for debugging
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private string testOverrideId = "TestBuild";

    //The name of the file, and whether or not the manager is set up for encryption
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    //The interval in which the game autosaves
    [Header("Auto Saving Config")]
    [SerializeField] private float autoSaveTimeSeconds = 60f;

    //The current game data that is being written/read
    private GameData gameData;
    //Finds all of the objects that implement the data persistence interface
    private List<IDataPersistence> dataPersistenceObjects;
    //Object reponsible for handling the data
    private FileDataHandler dataHandler;

    //The string that stores the currently selected profile id
    private string selectedProfileId = "test";

    //Keeps track of the autosavecoroutine when it's running
    private Coroutine autoSaveCoroutine;

    //Data persistencemanager singleton variable (Can be gotten publicly and can only be set in this script)
    public static DataPersistenceManager instance { get; private set; }

    //Awake is executed first
    private void Awake()
    {
        //Set the instance of the persistence manager if one doesn't exist yet.
        if(instance != null)
        {
            Debug.LogError("Found more than one data persistence manager in the scene!");
            Destroy(this.gameObject);
            return;
        }

        //Dont destroy on load, so it persists
        instance = this;
        DontDestroyOnLoad(gameObject);

        //Disable data persistence if debugging in other areas of the game
        if (disableDataPersistence)
        {
            Debug.LogWarning("Data persistence is currently disabled!");
        }

        //Creates a new fileDataHandler object
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        //Initialise the profile ID
        InitializeSelectedProfileId();

    }

    //Suscribes the object to the onsceneloaded method
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Populate the list of persistence object
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        //Load the game
        LoadGame();

        if (SceneManager.GetActiveScene().name != "Main_Menu")
        {
            //Start up auto saving
            if (autoSaveCoroutine != null)
            {
                StopCoroutine(autoSaveCoroutine);
            }

            autoSaveCoroutine = StartCoroutine(AutoSave());
        }
    }

    //Changes the currently selected profile ID (Called when clicking on a save slot
    public void ChangeSelectedProfileID(string newProfileId)
    {
        this.selectedProfileId = newProfileId;
        //call load game after this, which will use this profile specifically
        LoadGame();
    }

    //Deletes a save game when called (Called from a corresponding clear button for a save slot)
    public void DeleteProfileData(string profileId)
    {
        //Call the delete method in the dataHandler object
        dataHandler.Delete(profileId);

        //Initialises the profile id once again
        InitializeSelectedProfileId();
        //Load the game
        LoadGame();
    }

    //Responible for setting the selected profile id
    void InitializeSelectedProfileId()
    {
        //First, get the most recently updated profile ID if data exists already
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfile();

        //If overriding the profile ID for debugging, set the id to the testID instead
        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testOverrideId;
            Debug.LogWarning("Selected ID Overridden!");
        }
    }

    //Creates a new save data
    public void NewGame()
    {
        //Sets gamedata variable to a clean slate
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        //Data will not be loaded if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        //load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(selectedProfileId);

        if(this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        //If gamedata is null, start a new game

        if (this.gameData == null)
        {
            return;
        }


        //push loaded data to scripts that implement the persistence interface
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            //Each data persistence obect will have their version of the loaddata method called
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded collected relics = " + gameData.relicsCollected);
    }

    public string GetLastScene()
    {
        return dataHandler.GetDataLastScene(selectedProfileId);
    }

    //Save game is called in order to make data persist outside of the application
    public void SaveGame()
    {
        //If data persistence is disabled, then do not save any data
        if (disableDataPersistence)
        {
            return;
        }
        //Data will not be saved if the game data is null, as data needs to exist
        if(this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }
        //pass data to scripts to update it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            //Calls the respective save data in each persistence object, saving specified properties to the game data
            dataPersistenceObj.SaveData(gameData);
        }
        //Stores the date and time of the last save for this file 
        gameData.lastSaved = System.DateTime.Now.ToBinary();

        //save data to file using data handler
        dataHandler.Save(gameData, selectedProfileId);
    }

    private void OnApplicationQuit()
    {
        //Save the game when closing the application
        if (SceneManager.GetActiveScene().name != "Main_Menu")
        {
            SaveGame();
        }
    }

    public string GetSelectedID()
    {
        return selectedProfileId;
    }

    //Finds all of the persistence objects
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    //Checks whether or not game data exists within the directory at all
    public bool HasGameData()
    {
        return gameData != null;
    }

    //returns the data for all profiles (If it exists)
    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        
        return dataHandler.LoadAllProfiles();
    }

    //Auto save the game based on the interval
    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            SaveGame();
            Debug.Log("Auto Saved!");
        }
    }
}
