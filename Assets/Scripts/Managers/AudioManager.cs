//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Audio;
//using UnityEngine.SceneManagement;

//public class AudioManager : MonoBehaviour
//{
//    public static AudioManager Instance;

//    [Header("Mixer")]
//    [SerializeField] private AudioMixer mixer;
//    [SerializeField] private AudioMixerGroup musicGroup;
//    [SerializeField] private AudioMixerGroup sfxGroup;
//    [SerializeField] private AudioMixerGroup dubbingGroup;

//    [Header("Music")]
//    [SerializeField] private AudioClip mainMenuMusic;

//    [Header("UI")]
//    [SerializeField] private AudioClip uiClickSound;

//    [Header("Gameplay SFX")]
//    [SerializeField] private AudioClip[] instantTrashPickups;
//    [SerializeField] private AudioClip[] carryTrashPickups;
//    [SerializeField] private AudioClip[] carryTrashDelivers;
//    [SerializeField] private AudioClip goldenHornPickup;
//    [SerializeField] private AudioClip cannotPickupSound;

//    private AudioSource musicSource;
//    private AudioSource sfxSource;
//    private AudioSource dubbingSource;

//    private Coroutine musicRoutine;
//    private List<AudioClip> musicPool = new();

//    private const string MUSIC_PARAM = "Music";
//    private const string SFX_PARAM = "SFX";
//    private const string DUBBING_PARAM = "Dubbing";

//    #region UNITY

//    private void Awake()
//    {
//        if (Instance != null)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        DontDestroyOnLoad(gameObject);

//        CreateSources();
//    }

//    private void Start()
//    {
//        LoadAudioSettings();
//    }

//    private void OnEnable()
//    {
//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }

//    private void OnDisable()
//    {
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }

//    #endregion

//    #region SETUP

//    private void CreateSources()
//    {
//        musicSource = gameObject.AddComponent<AudioSource>();
//        musicSource.loop = false;
//        musicSource.playOnAwake = false;
//        musicSource.outputAudioMixerGroup = musicGroup;

//        sfxSource = gameObject.AddComponent<AudioSource>();
//        sfxSource.playOnAwake = false;
//        sfxSource.outputAudioMixerGroup = sfxGroup;

//        dubbingSource = gameObject.AddComponent<AudioSource>();
//        dubbingSource.playOnAwake = false;
//        dubbingSource.outputAudioMixerGroup = dubbingGroup;
//    }

//    #endregion

//    #region SCENE MUSIC

//    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        if (musicRoutine != null)
//            StopCoroutine(musicRoutine);

//        musicSource.Stop();

//        if (scene.buildIndex == 0)
//        {
//            PlayMainMenuMusic();
//            return;
//        }

//        LevelMusic levelMusic = FindAnyObjectByType<LevelMusic>();
//        if (levelMusic == null || levelMusic.musicClips.Count == 0)
//            return;

//        StartLevelMusic(levelMusic.musicClips);
//    }

//    private void PlayMainMenuMusic()
//    {
//        if (mainMenuMusic == null) return;

//        musicSource.loop = true;
//        musicSource.clip = mainMenuMusic;
//        musicSource.Play();
//    }

//    private void StartLevelMusic(List<AudioClip> clips)
//    {
//        musicPool = new List<AudioClip>(clips);
//        musicRoutine = StartCoroutine(LevelMusicRoutine(clips));
//    }

//    private IEnumerator LevelMusicRoutine(List<AudioClip> fullPool)
//    {
//        while (true)
//        {
//            if (musicPool.Count == 0)
//                musicPool = new List<AudioClip>(fullPool);

//            AudioClip clip = musicPool[Random.Range(0, musicPool.Count)];
//            musicPool.Remove(clip);

//            musicSource.clip = clip;
//            musicSource.Play();

//            yield return new WaitForSeconds(clip.length);
//        }
//    }

//    #endregion

//    #region SFX

//    public void PlayUIClick() => PlaySFX(uiClickSound);
//    public void PlayRandomInstantTrash() => PlayRandom(instantTrashPickups);
//    public void PlayRandomCarryPickup() => PlayRandom(carryTrashPickups);
//    public void PlayRandomCarryDeliver() => PlayRandom(carryTrashDelivers);
//    public void PlayGoldenHorn() => PlaySFX(goldenHornPickup);
//    public void PlayCannotPickup() => PlaySFX(cannotPickupSound);

//    private void PlaySFX(AudioClip clip)
//    {
//        if (clip == null) return;
//        sfxSource.PlayOneShot(clip);
//    }

//    private void PlayRandom(AudioClip[] clips)
//    {
//        if (clips == null || clips.Length == 0) return;
//        sfxSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
//    }

//    #endregion

//    #region DUBBING

//    public void PlayDubbing(AudioClip clip)
//    {
//        if (clip == null) return;
//        dubbingSource.Stop();
//        dubbingSource.PlayOneShot(clip);
//    }

//    #endregion

//    #region VOLUME

//    public void SetMusicVolume(float volume)
//    {
//        float volumeToSet = Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20;
//        if (volume == 0f) volumeToSet = -80f;

//        mixer.SetFloat(MUSIC_PARAM, volumeToSet);
//        PlayerPrefs.SetFloat("MusicVolume", volume);
//        PlayerPrefs.Save();
//    }

//    public void SetSFXVolume(float volume)
//    {
//        float volumeToSet = Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20;
//        if (volume == 0f) volumeToSet = -80f;

//        mixer.SetFloat(SFX_PARAM, volumeToSet);
//        PlayerPrefs.SetFloat("SFXVolume", volume);
//        PlayerPrefs.Save();
//    }

//    public void SetDubbingVolume(float volume)
//    {
//        float volumeToSet = Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20;
//        if (volume == 0f) volumeToSet = -80f;

//        mixer.SetFloat(DUBBING_PARAM, volumeToSet);
//        PlayerPrefs.SetFloat("DubbingVolume", volume);
//        PlayerPrefs.Save();
//    }

//    private void LoadAudioSettings()
//    {
//        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
//        SetMusicVolume(savedMusicVolume);

//        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
//        SetSFXVolume(savedSFXVolume);

//        float savedDubbingVolume = PlayerPrefs.GetFloat("DubbingVolume", 0.75f);
//        SetDubbingVolume(savedDubbingVolume);
//    }

//    #endregion
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup dubbingGroup;

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;

    [Header("UI")]
    [SerializeField] private AudioClip uiClickSound;

    [Header("Gameplay SFX")]
    [SerializeField] private AudioClip[] instantTrashPickups;
    [SerializeField] private AudioClip[] carryTrashPickups;
    [SerializeField] private AudioClip[] carryTrashDelivers;
    [SerializeField] private AudioClip goldenHornPickup;
    [SerializeField] private AudioClip cannotPickupSound;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource dubbingSource;

    private Coroutine musicRoutine;
    private List<AudioClip> musicPool = new();

    private const string MUSIC_PARAM = "Music";
    private const string SFX_PARAM = "SFX";
    private const string DUBBING_PARAM = "Dubbing";
    private const string MUSIC_PREF = "MusicVolume";
    private const string SFX_PREF = "SFXVolume";
    private const string DUBBING_PREF = "DubbingVolume";
    private const float DEFAULT_VOLUME = 0.75f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateSources();
    }

    private void Start() => LoadAudioSettings();

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    // Audio Source oluşturma
    private void CreateSources()
    {
        musicSource = CreateAudioSource(musicGroup, false);
        sfxSource = CreateAudioSource(sfxGroup, false);
        dubbingSource = CreateAudioSource(dubbingGroup, false);
    }

    private AudioSource CreateAudioSource(AudioMixerGroup group, bool loop)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = group;
        source.playOnAwake = false;
        source.loop = loop;
        return source;
    }

    // Sahne müziği yönetimi
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (musicRoutine != null) StopCoroutine(musicRoutine);
        musicSource.Stop();

        if (scene.buildIndex == 0)
        {
            PlayMainMenuMusic();
            return;
        }

        LevelMusic levelMusic = FindAnyObjectByType<LevelMusic>();
        if (levelMusic?.musicClips.Count > 0)
            StartLevelMusic(levelMusic.musicClips);
    }

    private void PlayMainMenuMusic()
    {
        if (!mainMenuMusic) return;
        musicSource.loop = true;
        musicSource.clip = mainMenuMusic;
        musicSource.Play();
    }

    private void StartLevelMusic(List<AudioClip> clips)
    {
        musicPool = new List<AudioClip>(clips);
        musicRoutine = StartCoroutine(LevelMusicRoutine(clips));
    }

    private IEnumerator LevelMusicRoutine(List<AudioClip> fullPool)
    {
        while (true)
        {
            if (musicPool.Count == 0)
                musicPool = new List<AudioClip>(fullPool);

            AudioClip clip = musicPool[Random.Range(0, musicPool.Count)];
            musicPool.Remove(clip);

            musicSource.clip = clip;
            musicSource.Play();

            yield return new WaitForSeconds(clip.length);
        }
    }

    // SFX çalma
    public void PlayUIClick() => PlaySFX(uiClickSound);
    public void PlayRandomInstantTrash() => PlayRandom(instantTrashPickups);
    public void PlayRandomCarryPickup() => PlayRandom(carryTrashPickups);
    public void PlayRandomCarryDeliver() => PlayRandom(carryTrashDelivers);
    public void PlayGoldenHorn() => PlaySFX(goldenHornPickup);
    public void PlayCannotPickup() => PlaySFX(cannotPickupSound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip) sfxSource.PlayOneShot(clip);
    }

    private void PlayRandom(AudioClip[] clips)
    {
        if (clips?.Length > 0) sfxSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

    // Dublaj çalma
    public void PlayDubbing(AudioClip clip)
    {
        if (!clip) return;
        dubbingSource.Stop();
        dubbingSource.PlayOneShot(clip);
    }

    // Ses seviyesi ayarları
    public void SetMusicVolume(float volume) => SetVolume(MUSIC_PARAM, MUSIC_PREF, volume);
    public void SetSFXVolume(float volume) => SetVolume(SFX_PARAM, SFX_PREF, volume);
    public void SetDubbingVolume(float volume) => SetVolume(DUBBING_PARAM, DUBBING_PREF, volume);

    private void SetVolume(string param, string pref, float volume)
    {
        float db = volume == 0f ? -80f : Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20;
        mixer.SetFloat(param, db);
        PlayerPrefs.SetFloat(pref, volume);
    }

    private void LoadAudioSettings()
    {
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_PREF, DEFAULT_VOLUME));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_PREF, DEFAULT_VOLUME));
        SetDubbingVolume(PlayerPrefs.GetFloat(DUBBING_PREF, DEFAULT_VOLUME));
    }
}