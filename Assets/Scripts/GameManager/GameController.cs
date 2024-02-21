using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    SS,
    TB,
}

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] PauseMenuController pauseMenuController;
    [SerializeField] Camera SSCamera;
    GameState state;

    void Start()
    {
        battleSystem.OnBattleEnd += EndBattle;
    }
    public void StartBattle()
    {
        state = GameState.TB;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        battleSystem.gameObject.SetActive(true);
        SSCamera.gameObject.SetActive(false);
    }

    //The parameter won is going to be true if the player wins the battle, and false if the player loses the battle.
    public void EndBattle(bool won)
    {
        //TODO: Handle if the player loses the battle
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.GetComponent<PlayerController>().setStats();
        battleSystem.gameObject.SetActive(false);
        SSCamera.gameObject.SetActive(true);
        state = GameState.SS;
    }

    void Update()
    {
        if (state == GameState.SS)
        {
            player.GetComponent<PlayerMovementController.PlayerMovementController>().HandleUpdate();
            player.GetComponent<PlayerController>().HandleUpdate();
            pauseMenuController.HandleUpdate();
        }
        else if (state == GameState.TB)
        {
            battleSystem.HandleUpdate();
        }
    }

    void FixedUpdate()
    {
        if (state == GameState.SS)
        {
            player.GetComponent<PlayerMovementController.PlayerMovementController>().HandleFixedUpdate();
        }
    }
}
