using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum GameState
{
    SS,
    TB,
}

public class GameController : MonoBehaviour, IDataPersistence
{
    [SerializeField] GameObject player;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] HumanModelAI humanModelAI;
    [SerializeField] PauseMenuController pauseMenuController;
    [SerializeField] Camera SSCamera;
    [SerializeField] bool TBDemo;
    GameState state;

    void Start()
    {
        BattleSystem.OnBattleEnd += EndBattle;
        if (TBDemo)
        {
            state = GameState.TB;
            //StartBattle();
        }
        else
        {
            DialogueManager.OnStartTBCombat += StartBattle;
        }
    }
    void OnDestroy()
    {
        BattleSystem.OnBattleEnd -= EndBattle;
        DialogueManager.OnStartTBCombat -= StartBattle;
    }
    public void StartBattle()
    {
        state = GameState.TB;
        if (!TBDemo)
        {
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            SSCamera.gameObject.SetActive(false);
            battleSystem.gameObject.SetActive(true);
        }
        else
        {
            //StartCoroutine(battleSystem.SetupBattle());
            
        }
    }
    //The parameter won is going to be true if the player wins the battle, and false if the player loses the battle.
    public void EndBattle(bool won)
    {

        if (TBDemo)
        {
            StartBattle();
        }
        else
        {
            //TODO: Handle if the player loses the battle
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            player.GetComponent<PlayerController>().SetLocalStats();
            SSCamera.gameObject.SetActive(true);
            battleSystem.gameObject.SetActive(false);
            state = GameState.SS;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    void Update()
    {
        if (state == GameState.SS)
        {
            // if (player == null)
            // {
            //     player = GameObject.FindGameObjectWithTag("Player");
            // }
            player.GetComponent<PlayerMovementController.PlayerMovementController>().HandleUpdate();
            player.GetComponent<PlayerController>().HandleUpdate();
            pauseMenuController.HandleUpdate();
            if (Input.GetKeyDown(KeyCode.M))
            {
                DataPersistenceManager.Instance.SaveGame();
                SceneManager.LoadSceneAsync("Main Menu");
            }
        }
        else if (state == GameState.TB)
        {
            battleSystem.HandleUpdate();
        }
    }

    void FixedUpdate()
    {
        // if (player == null)
        // {
        //     player = GameObject.FindGameObjectWithTag("Player");
        // }
        if (state == GameState.SS)
        {
            player.GetComponent<PlayerMovementController.PlayerMovementController>().HandleFixedUpdate();
        }
    }

    public void LoadData(GameData gameData)
    {
        //SceneManager.LoadSceneAsync(gameData.currentSceneIndex);
    }

    public void SaveData(GameData gameData)
    {
        PlayerPrefs.SetInt("currentSceneIndex", SceneManager.GetActiveScene().buildIndex);
    }
}
