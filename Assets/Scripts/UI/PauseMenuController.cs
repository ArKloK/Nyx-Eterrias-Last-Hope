using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    private PlayerMovementController.PlayerMovementController playerMovementController;
    public static bool canPause = true;
    [SerializeField] static bool isPaused = false;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject inventoryUI;
    [SerializeField] Inventory inventory;
    [Header("Settings")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Image darkOverlay;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] TMP_Dropdown textVelocityDropdown;

    // Start is called before the first frame update
    void Start()
    {
        LoadVolume();
        LoadDarkOverlay();
        LoadTextVelocity();
        playerMovementController = FindObjectOfType<PlayerMovementController.PlayerMovementController>();
    }

    void OnEnable()
    {
        LoadVolume();
        LoadDarkOverlay();
        LoadTextVelocity();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        ChangeDarkOverlay();
        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
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

    #region Music and SFX Volume
    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }
    public void UpdateSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }
    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    public void LoadVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            UpdateMusicVolume(musicSlider.value);
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            UpdateSFXVolume(sfxSlider.value);
        }
    }
    #endregion
    #region Dark Overlay
    private void ChangeDarkOverlay()
    {
        var tempColor = darkOverlay.color;
        tempColor.a = brightnessSlider.value;
        darkOverlay.color = tempColor;
    }
    public void LoadDarkOverlay()
    {
        if (PlayerPrefs.HasKey("DarkOverlay"))
        {
            brightnessSlider.value = PlayerPrefs.GetFloat("DarkOverlay");
        }
    }
    public void SaveDarkOverlay()
    {
        PlayerPrefs.SetFloat("DarkOverlay", brightnessSlider.value);
    }
    #endregion
    #region Text Velocity
    public void LoadTextVelocity()
    {
        if (PlayerPrefs.HasKey("TextVelocity"))
        {
            textVelocityDropdown.value = PlayerPrefs.GetInt("TextVelocity");
        }
    }
    public void SaveTextVelocity()
    {
        PlayerPrefs.SetInt("TextVelocity", textVelocityDropdown.value);
    }
    public void OnTextVelocityDropdownValueChanged()
    {
        PlayerPrefs.SetInt("TextVelocity", textVelocityDropdown.value);
    }
    #endregion
}
