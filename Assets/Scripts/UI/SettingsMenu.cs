using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider dubbingSlider;

    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    // PlayerPrefs anahtarları
    private const string MUSIC_PREF = "MusicVolume";
    private const string SFX_PREF = "SFXVolume";
    private const string DUBBING_PREF = "DubbingVolume";
    private const string RESOLUTION_PREF = "ResolutionIndex";
    private const string FULLSCREEN_PREF = "Fullscreen";

    // AudioMixer parametreleri
    private const string MUSIC_PARAM = "Music";
    private const string SFX_PARAM = "SFX";
    private const string DUBBING_PARAM = "Dubbing";

    // Varsayılan değerler
    private const float DEFAULT_VOLUME = 0.75f;

    private Resolution[] resolutions;

    private void Start()
    {
        // Listener'ları ekle
        if (musicSlider) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        if (dubbingSlider) dubbingSlider.onValueChanged.AddListener(SetDubbingVolume);
        if (fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        if (resolutionDropdown) resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // Resolution dropdown'ı doldur
        SetupResolutionDropdown();

        // Ayarları yükle
        LoadAllSettings();
    }

    #region Audio Settings

    public void SetMusicVolume(float value)
    {
        float dbValue = VolumeToDecibel(value);
        audioMixer.SetFloat(MUSIC_PARAM, dbValue);
        PlayerPrefs.SetFloat(MUSIC_PREF, value);
    }

    public void SetSFXVolume(float value)
    {
        float dbValue = VolumeToDecibel(value);
        audioMixer.SetFloat(SFX_PARAM, dbValue);
        PlayerPrefs.SetFloat(SFX_PREF, value);
    }

    public void SetDubbingVolume(float value)
    {
        float dbValue = VolumeToDecibel(value);
        audioMixer.SetFloat(DUBBING_PARAM, dbValue);
        PlayerPrefs.SetFloat(DUBBING_PREF, value);
    }

    private float VolumeToDecibel(float volume)
    {
        // 0-1 arası değeri -80 ile 0 dB arasına çevirir
        return volume > 0 ? Mathf.Log10(volume) * 20f : -80f;
    }

    #endregion

    #region Graphics Settings

    private void SetupResolutionDropdown()
    {
        if (!resolutionDropdown) return;

        // Desteklenen tüm çözünürlükleri al
        resolutions = Screen.resolutions;

        // Benzersiz çözünürlükleri filtrele (aynı genişlik/yükseklik)
        resolutions = resolutions
            .Select(resolution => new Resolution { width = resolution.width, height = resolution.height })
            .Distinct()
            .ToArray();

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Mevcut çözünürlüğü bul
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutions == null || resolutionIndex >= resolutions.Length) return;

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt(RESOLUTION_PREF, resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FULLSCREEN_PREF, isFullscreen ? 1 : 0);
    }

    #endregion

    #region Load Settings

    private void LoadAllSettings()
    {
        LoadAudioSettings();
        LoadGraphicsSettings();
    }

    private void LoadAudioSettings()
    {
        // Music
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_PREF, DEFAULT_VOLUME);
        if (musicSlider)
        {
            musicSlider.value = musicVolume;
        }
        SetMusicVolume(musicVolume);

        // SFX
        float sfxVolume = PlayerPrefs.GetFloat(SFX_PREF, DEFAULT_VOLUME);
        if (sfxSlider)
        {
            sfxSlider.value = sfxVolume;
        }
        SetSFXVolume(sfxVolume);

        // Dubbing
        float dubbingVolume = PlayerPrefs.GetFloat(DUBBING_PREF, DEFAULT_VOLUME);
        if (dubbingSlider)
        {
            dubbingSlider.value = dubbingVolume;
        }
        SetDubbingVolume(dubbingVolume);
    }

    private void LoadGraphicsSettings()
    {
        // Fullscreen
        int fullscreenValue = PlayerPrefs.GetInt(FULLSCREEN_PREF, 1); // Varsayılan tam ekran
        bool isFullscreen = fullscreenValue == 1;
        if (fullscreenToggle)
        {
            fullscreenToggle.isOn = isFullscreen;
        }
        Screen.fullScreen = isFullscreen;

        // Resolution
        int resolutionIndex = PlayerPrefs.GetInt(RESOLUTION_PREF, resolutions.Length - 1);
        if (resolutionDropdown && resolutions != null && resolutionIndex < resolutions.Length)
        {
            resolutionDropdown.value = resolutionIndex;
            SetResolution(resolutionIndex);
        }
    }

    #endregion

    #region Reset

    public void ResetToDefaults()
    {
        // Audio
        if (musicSlider)
        {
            musicSlider.value = DEFAULT_VOLUME;
            SetMusicVolume(DEFAULT_VOLUME);
        }

        if (sfxSlider)
        {
            sfxSlider.value = DEFAULT_VOLUME;
            SetSFXVolume(DEFAULT_VOLUME);
        }

        if (dubbingSlider)
        {
            dubbingSlider.value = DEFAULT_VOLUME;
            SetDubbingVolume(DEFAULT_VOLUME);
        }

        // Graphics
        if (fullscreenToggle)
        {
            fullscreenToggle.isOn = true;
            SetFullscreen(true);
        }

        if (resolutionDropdown && resolutions != null)
        {
            int defaultResolution = resolutions.Length - 1; // En yüksek çözünürlük
            resolutionDropdown.value = defaultResolution;
            SetResolution(defaultResolution);
        }

        // PlayerPrefs'i kaydet
        PlayerPrefs.Save();
    }

    #endregion

    private void OnDestroy()
    {
        // Listener'ları temizle
        if (musicSlider) musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
        if (dubbingSlider) dubbingSlider.onValueChanged.RemoveListener(SetDubbingVolume);
        if (fullscreenToggle) fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        if (resolutionDropdown) resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
    }
}