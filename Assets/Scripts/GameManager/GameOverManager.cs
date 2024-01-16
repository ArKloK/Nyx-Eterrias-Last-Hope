using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;
    public GameObject gameOverPanel;

    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += HandlePlayerDeath;
        GameOverPanel.OnRestartButtonClicked += HandleRestartButtonClicked;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= HandlePlayerDeath;
        GameOverPanel.OnRestartButtonClicked -= HandleRestartButtonClicked;
    }

    private void HandlePlayerDeath(object sender, PlayerEventArgs e)
    {
        gameOverPanel.SetActive(true);
        playerController = e.PlayerController;
        player = playerController.gameObject;
        player.SetActive(false);
        Debug.Log("Player has died");
    }

    private void HandleRestartButtonClicked()
    {
        Vector3 respawnPoint = RespawnController.instance.respawnPoint.position;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        player.transform.position = RespawnController.instance.respawnPoint.position;
        gameOverPanel.SetActive(false);
        player.SetActive(true);
        player.GetComponent<PlayerController>().setmaxHealthPoints();
    }
}
