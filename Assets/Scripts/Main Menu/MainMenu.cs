using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

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
    }
    #region Buttons
    public void OnNewGameButtonClicked()
    {
        DisableMenuButtons();
        DataPersistenceManager.Instance.CreateNewGame = true;
        LevelManager.instance.LoadScene("FirstLevel", "CrossFade");
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
    #region Text Velocitys
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
