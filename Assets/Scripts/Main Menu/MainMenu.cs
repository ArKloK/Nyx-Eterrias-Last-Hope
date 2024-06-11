using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using Unity.VisualScripting;

public class MainMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Image darkOverlay;
    [SerializeField] TMP_Dropdown textVelocityDropdown;

    [Header("Sliders")]
    [SerializeField] Slider brightnessSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;

    void Start()
    {
        LoadVolume();
        LoadDarkOverlay();
        LoadTextVelocity();
        DontDestroyOnLoadScript.DestroyAll();
        if (DataPersistenceManager.Instance.HasGameData())
        {
            loadGameButton.gameObject.SetActive(true);
        }
        else
        {
            loadGameButton.gameObject.SetActive(false);
        }
    }
    void OnEnable()
    {
        LoadVolume();
        LoadDarkOverlay();
        LoadTextVelocity();
    }
    void Update()
    {
        ChangeDarkOverlay();
        UpdateMusicVolume();
        UpdateSFXVolume();
    }
    void OnApplicationQuit()
    {
        SaveVolume();
        SaveDarkOverlay();
        SaveTextVelocity();
    }
    #region Buttons
    public void OnNewGameButtonClicked()
    {
        DisableMenuButtons();
        DataPersistenceManager.Instance.CreateNewGame = true;
        LevelManager.Instance.LoadScene("FirstLevel", "CrossFade");
    }
    public void OnLoadGameButtonClicked()
    {
        DisableMenuButtons();
        SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("currentSceneIndex", 1));
    }
    public void OnQuitGameButtonClicked()
    {
        Application.Quit();
    }
    private void DisableMenuButtons()
    {
        newGameButton.interactable = false;
        loadGameButton.interactable = false;
    }
    #endregion
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
