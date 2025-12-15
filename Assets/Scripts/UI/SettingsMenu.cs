//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class SettingsMenu : MonoBehaviour
//{
//    [Header("Ses")]
//    [SerializeField] private Slider musicSlider;
//    [SerializeField] private Slider sfxSlider;
//    [SerializeField] private Slider dubbingSlider;

//    [Header("Görüntü")]
//    [SerializeField] private TMP_Dropdown resolutionDropdown;
//    [SerializeField] private Toggle fullscreenToggle;

//    private Resolution[] resolutions;

//    private const string MUSIC_PREF = "MusicVolume";
//    private const string SFX_PREF = "SFXVolume";
//    private const string DUBBING_PREF = "DubbingVolume";

//    private void Start()
//    {
//        SetupAudioSliders();
//        LoadAudioSettings();
//        LoadResolutionSettings();
//    }

//    private void SetupAudioSliders()
//    {
//        if (musicSlider != null)
//            musicSlider.onValueChanged.AddListener(SetMusicVolume);

//        if (sfxSlider != null)
//            sfxSlider.onValueChanged.AddListener(SetSFXVolume);

//        if (dubbingSlider != null)
//            dubbingSlider.onValueChanged.AddListener(SetDubbingVolume);
//    }

//    // ===================== SES =====================

//    public void SetMusicVolume(float value)
//    {
//        AudioManager.Instance?.SetMusicVolume(value);
//    }

//    public void SetSFXVolume(float value)
//    {
//        AudioManager.Instance?.SetSFXVolume(value);
//    }

//    public void SetDubbingVolume(float value)
//    {
//        AudioManager.Instance?.SetDubbingVolume(value);
//    }

//    private void LoadAudioSettings()
//    {
//        float savedMusicVolume = PlayerPrefs.GetFloat(MUSIC_PREF, 0.75f);
//        if (musicSlider != null)
//        {
//            musicSlider.value = savedMusicVolume;
//        }

//        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_PREF, 0.75f);
//        if (sfxSlider != null)
//        {
//            sfxSlider.value = savedSFXVolume;
//        }

//        float savedDubbingVolume = PlayerPrefs.GetFloat(DUBBING_PREF, 0.75f);
//        if (dubbingSlider != null)
//        {
//            dubbingSlider.value = savedDubbingVolume;
//        }
//    }

//    // ===================== GÖRÜNTÜ =====================

//    public void SetResolution(int index)
//    {
//        if (resolutions == null || index < 0 || index >= resolutions.Length)
//            return;

//        Resolution r = resolutions[index];
//        Screen.SetResolution(r.width, r.height, Screen.fullScreen);

//        PlayerPrefs.SetInt("ResolutionIndex", index);
//        PlayerPrefs.Save();
//    }

//    public void SetFullScreen(bool fullscreen)
//    {
//        Screen.fullScreen = fullscreen;
//        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
//        PlayerPrefs.Save();
//    }

//    private void LoadResolutionSettings()
//    {
//        resolutions = Screen.resolutions;
//        if (resolutionDropdown == null) return;

//        resolutionDropdown.ClearOptions();

//        List<string> options = new();
//        int currentIndex = 0;

//        for (int i = 0; i < resolutions.Length; i++)
//        {
//            options.Add($"{resolutions[i].width} x {resolutions[i].height}");

//            if (resolutions[i].width == Screen.currentResolution.width &&
//                resolutions[i].height == Screen.currentResolution.height)
//                currentIndex = i;
//        }

//        resolutionDropdown.AddOptions(options);

//        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", currentIndex);
//        savedIndex = Mathf.Clamp(savedIndex, 0, resolutions.Length - 1);

//        resolutionDropdown.value = savedIndex;
//        resolutionDropdown.RefreshShownValue();

//        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
//        fullscreenToggle.isOn = fullscreen;
//        Screen.fullScreen = fullscreen;

//        SetResolution(savedIndex);
//    }

//    // ===================== RESET =====================

//    public void ResetToDefaults()
//    {
//        const float defaultVolume = 0.75f;

//        if (musicSlider != null)
//        {
//            musicSlider.value = defaultVolume;
//            SetMusicVolume(defaultVolume);
//        }

//        if (sfxSlider != null)
//        {
//            sfxSlider.value = defaultVolume;
//            SetSFXVolume(defaultVolume);
//        }

//        if (dubbingSlider != null)
//        {
//            dubbingSlider.value = defaultVolume;
//            SetDubbingVolume(defaultVolume);
//        }

//        if (fullscreenToggle != null)
//        {
//            fullscreenToggle.isOn = true;
//            SetFullScreen(true);
//        }

//        if (resolutions != null && resolutions.Length > 0 && resolutionDropdown != null)
//        {
//            int index = resolutions.Length - 1;
//            resolutionDropdown.value = index;
//            resolutionDropdown.RefreshShownValue();
//            SetResolution(index);
//        }
//    }
//}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Ses")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider dubbingSlider;

    [Header("Görüntü")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private Resolution[] resolutions;

    private const string MUSIC_PREF = "MusicVolume";
    private const string SFX_PREF = "SFXVolume";
    private const string DUBBING_PREF = "DubbingVolume";
    private const string RESOLUTION_PREF = "ResolutionIndex";
    private const string FULLSCREEN_PREF = "Fullscreen";
    private const float DEFAULT_VOLUME = 0.75f;

    private void Start()
    {
        // Ses ayarlarýný yükle ve dinleyicileri ekle
        musicSlider?.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider?.onValueChanged.AddListener(SetSFXVolume);
        dubbingSlider?.onValueChanged.AddListener(SetDubbingVolume);

        LoadAudioSettings();
        LoadResolutionSettings();
    }

    // Ses Ayarlarý
    public void SetMusicVolume(float value) => AudioManager.Instance?.SetMusicVolume(value);
    public void SetSFXVolume(float value) => AudioManager.Instance?.SetSFXVolume(value);
    public void SetDubbingVolume(float value) => AudioManager.Instance?.SetDubbingVolume(value);

    private void LoadAudioSettings()
    {
        if (musicSlider) musicSlider.value = PlayerPrefs.GetFloat(MUSIC_PREF, DEFAULT_VOLUME);
        if (sfxSlider) sfxSlider.value = PlayerPrefs.GetFloat(SFX_PREF, DEFAULT_VOLUME);
        if (dubbingSlider) dubbingSlider.value = PlayerPrefs.GetFloat(DUBBING_PREF, DEFAULT_VOLUME);
    }

    // Görüntü Ayarlarý
    public void SetResolution(int index)
    {
        if (resolutions == null || index < 0 || index >= resolutions.Length) return;

        Resolution r = resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
        PlayerPrefs.SetInt(RESOLUTION_PREF, index);
    }

    public void SetFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt(FULLSCREEN_PREF, fullscreen ? 1 : 0);
    }

    private void LoadResolutionSettings()
    {
        resolutions = Screen.resolutions;
        if (!resolutionDropdown) return;

        resolutionDropdown.ClearOptions();

        List<string> options = new();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add($"{resolutions[i].width} x {resolutions[i].height}");
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentIndex = i;
        }

        resolutionDropdown.AddOptions(options);

        int savedIndex = Mathf.Clamp(PlayerPrefs.GetInt(RESOLUTION_PREF, currentIndex), 0, resolutions.Length - 1);
        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_PREF, 1) == 1;
        if (fullscreenToggle) fullscreenToggle.isOn = fullscreen;
        Screen.fullScreen = fullscreen;

        SetResolution(savedIndex);
    }

    // Varsayýlan ayarlara dön
    public void ResetToDefaults()
    {
        if (musicSlider) { musicSlider.value = DEFAULT_VOLUME; SetMusicVolume(DEFAULT_VOLUME); }
        if (sfxSlider) { sfxSlider.value = DEFAULT_VOLUME; SetSFXVolume(DEFAULT_VOLUME); }
        if (dubbingSlider) { dubbingSlider.value = DEFAULT_VOLUME; SetDubbingVolume(DEFAULT_VOLUME); }
        if (fullscreenToggle) { fullscreenToggle.isOn = true; SetFullScreen(true); }

        if (resolutions?.Length > 0 && resolutionDropdown)
        {
            int index = resolutions.Length - 1;
            resolutionDropdown.value = index;
            resolutionDropdown.RefreshShownValue();
            SetResolution(index);
        }
    }
}