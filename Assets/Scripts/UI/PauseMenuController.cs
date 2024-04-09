using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private PlayerMovementController.PlayerMovementController playerMovementController;
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject inventoryUI;
    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        playerMovementController = FindObjectOfType<PlayerMovementController.PlayerMovementController>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryUI.activeSelf)
            {
                inventoryUI.SetActive(false);
                return;
            }
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        playerMovementController.canMove = false;
        //Time.timeScale = 0f;
        isPaused = true;
        pauseMenuUI.SetActive(true);
    }
    public void Resume()
    {
        playerMovementController.canMove = true;
        //Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI.SetActive(false);
    }
    public void OpenInventory()
    {
        inventoryUI.SetActive(true);
        inventory.LaunchInventoryChange();
    }
    public void SaveGame()
    {
        DataPersistenceManager.Instance.SaveGame();
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
}
