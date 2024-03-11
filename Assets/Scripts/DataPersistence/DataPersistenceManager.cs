using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;

public class DataPersistenceManager : MonoBehaviour
{
    public static event Action<Vector3> OnSceneLoadedEvent;
    [SerializeField] PlayerControllerData playerControllerData;

    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull;
    [SerializeField] private bool useLoadandSave;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;
    public static DataPersistenceManager Instance { get; private set; }
    private bool createNewGame;
    private bool useSavedPosition;
    private GameData gameData;
    private FileDataHandler fileDataHandler;
    private List<IDataPersistence> dataPersistenceObjects;

    public bool CreateNewGame { get => createNewGame; set => createNewGame = value; }
    public bool UseSavedPosition { get => useSavedPosition; set => useSavedPosition = value; }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("There is more than one DataPersistenceManager in the scene. Deleting one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

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
        Debug.Log("Scene loaded: " + scene.name);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

        // Check if the loaded scene is the second level
        if (scene.name == "SecondLevel")
        {
            // UseSavedPosition se establece en false para indicar que no se debe usar la posición guardada
            if (UseSavedPosition)
            {
                OnSceneLoadedEvent?.Invoke(gameData.playerPosition);
            }
            else
            {
                // Establecer la posición del jugador según el objeto en la escena
                GameObject spawnPoint = GameObject.Find("FirstRespawnPoint");
                if (spawnPoint != null)
                {
                    OnSceneLoadedEvent?.Invoke(spawnPoint.transform.position);
                }
            }
        }
    }

    public void NewGame()
    {
        Debug.Log("Creating a new game.");
        gameData = new GameData(playerControllerData);
    }

    public void LoadGame()
    {
        if (!useLoadandSave) return;//For debugging purposes
        gameData = fileDataHandler.LoadData(playerControllerData);
        //Forces the creation of a new game if the createNewGame flag is true
        if(createNewGame) gameData = null;

        //We create a new game if the game data is null and the initializeDataIfNull flag is true for debugging purposes
        if (gameData == null && initializeDataIfNull)
        {
            NewGame();
        }
        
        if (gameData == null)
        {
            Debug.Log("No saved game found. A new game has to be created to load the game data.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(gameData);
        }

        createNewGame = false;
    }

    public void SaveGame()
    {
        if (!useLoadandSave) return;//For debugging purposes
        if (gameData == null)
        {
            Debug.Log("No game data to save.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(gameData);
            
        }
        gameData.IsEmpty = false;
        fileDataHandler.SaveData(gameData);
    }

    //THIS METHOD IS ONLY FOR TESTING PURPOSES, THE FINAL GAME WILL HAVE A MENU TO SAVE THE GAME
    // void OnDestroy()
    // {
    //     SaveGame();
    // }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return !gameData.IsEmpty;
    }
}
