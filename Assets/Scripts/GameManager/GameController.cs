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
    public GameObject player;
    public BattleSystem battleSystem;
    GameState state;
    public Camera SSCamera;

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

    public void EndBattle(bool won)
    {
        state = GameState.SS;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        battleSystem.gameObject.SetActive(false);
        SSCamera.gameObject.SetActive(true);
    }

    void Update()
    {
        if (state == GameState.SS)
        {
            player.GetComponent<PlayerMovementController.PlayerMovementController>().HandleUpdate();
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
