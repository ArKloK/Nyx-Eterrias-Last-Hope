using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;

    void Start()
    {
        DontDestroyOnLoadScript.DestroyAll();
        if(DataPersistenceManager.Instance.HasGameData())
        {
            loadGameButton.gameObject.SetActive(true);
        }
        else
        {
            loadGameButton.gameObject.SetActive(false);
        }
    }

    public void OnNewGameButtonClicked()
    {
        DisableMenuButtons();
        DataPersistenceManager.Instance.CreateNewGame = true;
        SceneManager.LoadSceneAsync("FirstLevel");
    }

    public void OnLoadGameButtonClicked()
    {
        DisableMenuButtons();
        SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("currentSceneIndex", 1));
    }

    private void DisableMenuButtons()
    {
        newGameButton.interactable = false;
        loadGameButton.interactable = false;
    }
}
