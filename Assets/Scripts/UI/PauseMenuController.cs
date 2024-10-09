using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    // Player Input Actions
    private PlayerInputActions playerInputActions;

    // Game State
    private static bool canPause = true;
    [SerializeField] private bool isPaused = false;

    // UI References
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject inventoryUI;

    // Inventory System
    [Header("Inventory")]
    [SerializeField] private Inventory inventory;

    // Settings
    [Header("Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Image darkOverlay;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Dropdown textVelocityDropdown;

    // Properties
    public static bool CanPause { get => canPause; set => canPause = value; }

    // Events
    public static event Action OnPause;
    public static event Action OnResume;

    void Awake()
    {
        playerInputActions = InputManager.PlayerInputActions;
        playerInputActions.Player.PauseGame.started += PauseGame;
        playerInputActions.UI.Submit.started += (callbackContext) => { Debug.Log("Submit"); };
        playerInputActions.UI.Cancel.started += Cancel;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadVolume();
        LoadDarkOverlay();
        LoadTextVelocity();
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
        UpdateMusicVolume();
        UpdateSFXVolume();
    }

    public void PauseGame(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started && CanPause)
        {
            if (!isPaused)
            {
                Pause();
            }
        }
    }

    public void Cancel(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started && isPaused)
        {
            if (inventoryUI.activeSelf)
            {
                inventoryUI.SetActive(false);
                return;
            }
            Resume();
        }
    }
    public void Pause()
    {
        OnPause?.Invoke();
        isPaused = true;
        pauseMenuUI.SetActive(true);
    }
    public void Resume()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        OnResume?.Invoke();
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
    public void UpdateMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", musicSlider.value);
    }
    public void UpdateSFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", sfxSlider.value);
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
            UpdateMusicVolume();
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            UpdateSFXVolume();
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
