using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SettingsMenu : MonoBehaviour
{
    [Header("Ses")]
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Görüntü")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;

    void Start()
    {
        LoadAudioSettings();
        LoadResolutionSettings();
        LoadControlSettings();
    }

    // ---------------- SES ----------------//
    public void SetMusicVolume(float volume)
    {
        if (audioMixer == null) return;

        float volumeToSet = Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20f;
        if (volume == 0f) volumeToSet = -80f;

        audioMixer.SetFloat("MusicVolume", volumeToSet);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer == null) return;

        float volumeToSet = Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20f;
        if (volume == 0f) volumeToSet = -80f;

        audioMixer.SetFloat("SFXVolume", volumeToSet);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    // ---------------- GÖRÜNTÜ ----------------//
    public void SetResolution(int resolutionIndex)
    {
        if (resolutions == null || resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
            return;

        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullScreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadResolutionSettings()
    {
        resolutions = Screen.resolutions;

        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();

            int currentResolutionIndex = 0;
            var options = new List<string>();

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }

            }

            resolutionDropdown.AddOptions(options);

            int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
            savedIndex = Mathf.Clamp(savedIndex, 0, resolutions.Length - 1);
            resolutionDropdown.value = savedIndex;
            resolutionDropdown.RefreshShownValue();

            // Listener ekle
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);

            bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = isFullscreen;
                fullscreenToggle.onValueChanged.RemoveAllListeners();
                fullscreenToggle.onValueChanged.AddListener(SetFullScreen);
            }

            Screen.fullScreen = isFullscreen;

            //Uygula
            SetResolution(savedIndex);

        }

        else
        {
            // Eðer dropdown yoksa yine de fullscreen ayarýný uygula
            bool isFullscreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
            Screen.fullScreen = isFullscreen;
            if (fullscreenToggle != null) fullscreenToggle.isOn = isFullscreen;
        }
    }

    // ---------------- AUDIO AYARLARI YÜKLE ----------------//
    private void LoadAudioSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
            // Eskiden eklenmiþ olabilecek listener'larý kaldýrýp tekrar ekleyelim
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        SetMusicVolume(musicVolume);

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        SetSFXVolume(sfxVolume);
    }

    // ---------------- KONTROL AYARLARI (PLACEHOLDER) ----------------//
   
    private void LoadControlSettings()
    {
        // Eðer kontrol tuþlarý/ayarlarý varsa burada yükleyip uygulayabilirsin.
        // Þu anda bir placeholder: eðer kullanmayacaksan Start() içindeki çaðrýyý kaldýrabilirsin.
    }

    // ---------------- RESET ---------------- //

    public void ResetToDefaults()
    {
        // Müzik ve SFX sesleri //
        float defaultVolume = 0.75f;
        if (musicSlider  != null) musicSlider.value = defaultVolume;
        SetMusicVolume(defaultVolume);

        float defaultSFXVolume = 0.75f;
        if (sfxSlider != null) sfxSlider.value = defaultSFXVolume;

        // Fullscreen //
        if (fullscreenToggle != null) fullscreenToggle.isOn = true;
        SetFullScreen(true);

        // Çözünürlük - en yüksek çözünürlüðü varsayalým //
        if (resolutions != null && resolutions.Length > 0 && resolutionDropdown != null)
        {
            int defaultResolutionIndex = resolutions.Length - 1;
            resolutionDropdown.value = defaultResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            SetResolution(defaultResolutionIndex);
        }

        PlayerPrefs.Save();


}


}



