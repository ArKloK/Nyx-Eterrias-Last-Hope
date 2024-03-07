using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] PlayerControllerData playerControllerData;
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;
    public static DataPersistenceManager Instance { get; private set; }
    private GameData gameData;
    private FileDataHandler fileDataHandler;
    private List<IDataPersistence> dataPersistenceObjects;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one DataPersistenceManager in the scene");
        }
        Instance = this;
    }

    //THIS METHOD IS ONLY FOR TESTING PURPOSES, THE FINAL GAME WILL HAVE A MENU TO START A NEW GAME OR LOAD A SAVED GAME
    private void Start()
    {
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     LoadGame();
        // }
    }

    public void NewGame()
    {
        gameData = new GameData(playerControllerData);
    }

    public void LoadGame()
    {
        gameData = fileDataHandler.LoadData(playerControllerData);

        if (gameData == null)
        {
            Debug.Log("No saved game found");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(gameData);
        }

        fileDataHandler.SaveData(gameData);
    }

    //THIS METHOD IS ONLY FOR TESTING PURPOSES, THE FINAL GAME WILL HAVE A MENU TO SAVE THE GAME
    void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
