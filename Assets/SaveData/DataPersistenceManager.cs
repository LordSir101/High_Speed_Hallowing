using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File storage Config")]
    [SerializeField] string fileName;

    private FileDataHandler dataHandler;

    private GameData gameData;
    public static DataPersistenceManager instance {get; private set;}
    private List<IDataPersistence> dataPersitenceObjects;

    private void Awake()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

        if(instance != null)
        {
            Debug.LogError("An Instance of DataPersistenceManager already exists");
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;

    }

   

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if(gameData == null)
        {
            NewGame();
        }

        foreach(IDataPersistence obj in dataPersitenceObjects)
        {
            obj.LoadData(gameData);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersitenceObjects = FindAllDataPersitenceObjects();
        LoadGame();

    }

    void OnSceneUnLoaded(Scene scene)
    {
        SaveGame();
    }

    public void SaveGame()
    {
        foreach(IDataPersistence obj in dataPersitenceObjects)
        {
            obj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersitenceObjects()
    {
        IEnumerable<IDataPersistence> objects = FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None).OfType<IDataPersistence>();

        return new List<IDataPersistence>(objects);
    }
}
