//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Audio;

//public class AudioManager : MonoBehaviour
//{
//    public static AudioManager Instance;

//    [Header("Mixer")]
//    [SerializeField] private AudioMixer mixer;
//    [SerializeField] private AudioMixerGroup musicGroup;
//    [SerializeField] private AudioMixerGroup sfxGroup;
//    [SerializeField] private AudioMixerGroup dubbingGroup;

//    [Header("UI SFX")]
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
//    private const string MUSIC_PREF = "MusicVolume";
//    private const string SFX_PREF = "SFXVolume";
//    private const string DUBBING_PREF = "DubbingVolume";
//    private const float DEFAULT_VOLUME = 0.75f;

//    private void Awake()
//    {
//        // Singleton - ama DontDestroyOnLoad YOK
//        if (Instance != null)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        CreateSources();
//    }

//    private void Start()
//    {
//        LoadAudioSettings();
//        StartLevelMusic();
//    }

//    private void OnDestroy()
//    {
//        // Coroutine temizliği
//        if (musicRoutine != null)
//        {
//            StopCoroutine(musicRoutine);
//        }

//        // Instance temizliği
//        if (Instance == this)
//        {
//            Instance = null;
//        }
//    }

//    // Audio Source oluşturma
//    private void CreateSources()
//    {
//        musicSource = CreateAudioSource(musicGroup, false);
//        sfxSource = CreateAudioSource(sfxGroup, false);
//        dubbingSource = CreateAudioSource(dubbingGroup, false);
//    }

//    private AudioSource CreateAudioSource(AudioMixerGroup group, bool loop)
//    {
//        AudioSource source = gameObject.AddComponent<AudioSource>();
//        source.outputAudioMixerGroup = group;
//        source.playOnAwake = false;
//        source.loop = loop;
//        return source;
//    }

//    // Level müziği
//    private void StartLevelMusic()
//    {
//        LevelMusic levelMusic = FindAnyObjectByType<LevelMusic>();
//        if (levelMusic?.musicClips.Count > 0)
//        {
//            musicPool = new List<AudioClip>(levelMusic.musicClips);
//            musicRoutine = StartCoroutine(LevelMusicRoutine(levelMusic.musicClips));
//        }
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

//    // UI SFX
//    public void PlayUIClick() => PlaySFX(uiClickSound);

//    // Gameplay SFX
//    public void PlayRandomInstantTrash() => PlayRandom(instantTrashPickups);
//    public void PlayRandomCarryPickup() => PlayRandom(carryTrashPickups);
//    public void PlayRandomCarryDeliver() => PlayRandom(carryTrashDelivers);
//    public void PlayGoldenHorn() => PlaySFX(goldenHornPickup);
//    public void PlayCannotPickup() => PlaySFX(cannotPickupSound);

//    private void PlaySFX(AudioClip clip)
//    {
//        if (clip) sfxSource.PlayOneShot(clip);
//    }

//    private void PlayRandom(AudioClip[] clips)
//    {
//        if (clips?.Length > 0) sfxSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
//    }

//    // Dublaj
//    public void PlayDubbing(AudioClip clip)
//    {
//        if (!clip) return;
//        dubbingSource.Stop();
//        dubbingSource.PlayOneShot(clip);
//    }

//    // Ses seviyesi ayarları
//    public void SetMusicVolume(float volume) => SetVolume(MUSIC_PARAM, MUSIC_PREF, volume);
//    public void SetSFXVolume(float volume) => SetVolume(SFX_PARAM, SFX_PREF, volume);
//    public void SetDubbingVolume(float volume) => SetVolume(DUBBING_PARAM, DUBBING_PREF, volume);

//    private void SetVolume(string param, string pref, float volume)
//    {
//        float db = volume == 0f ? -80f : Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20;
//        mixer.SetFloat(param, db);
//        PlayerPrefs.SetFloat(pref, volume);
//    }

//    private void LoadAudioSettings()
//    {
//        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_PREF, DEFAULT_VOLUME));
//        SetSFXVolume(PlayerPrefs.GetFloat(SFX_PREF, DEFAULT_VOLUME));
//        SetDubbingVolume(PlayerPrefs.GetFloat(DUBBING_PREF, DEFAULT_VOLUME));
//    }
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

    [Header("UI SFX")]
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
        CreateSources();

        DontDestroyOnLoad(gameObject); //unutma!!!
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        LoadAudioSettings();
        // ❌ StartLevelMusic BURADA YOK
    }

    private void OnDestroy()
    {
        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        if (Instance == this)
            Instance = null;
    }

    // ===================== SCENE CALLBACK =====================
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ana menü değilse level müziğini başlat
        if (scene.name != "AnaMenu")
        {
            StartLevelMusic();
        }
    }

    // ===================== AUDIO SOURCE =====================
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

    // ===================== LEVEL MUSIC =====================
    private void StartLevelMusic()
    {
        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        LevelMusic levelMusic = FindAnyObjectByType<LevelMusic>();
        if (levelMusic == null || levelMusic.musicClips.Count == 0)
            return;

        musicPool = new List<AudioClip>(levelMusic.musicClips);
        musicRoutine = StartCoroutine(LevelMusicRoutine(levelMusic.musicClips));
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

    // ===================== SFX =====================
    public void PlayUIClick() => PlaySFX(uiClickSound);
    public void PlayRandomInstantTrash() => PlayRandom(instantTrashPickups);
    public void PlayRandomCarryPickup() => PlayRandom(carryTrashPickups);
    public void PlayRandomCarryDeliver() => PlayRandom(carryTrashDelivers);
    public void PlayGoldenHorn() => PlaySFX(goldenHornPickup);
    public void PlayCannotPickup() => PlaySFX(cannotPickupSound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip)
            sfxSource.PlayOneShot(clip);
    }

    private void PlayRandom(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
            sfxSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

    // ===================== DUBBING =====================
    public void PlayDubbing(AudioClip clip)
    {
        if (!clip) return;

        dubbingSource.Stop();
        dubbingSource.PlayOneShot(clip);
    }

    // ===================== VOLUME =====================
    public void SetMusicVolume(float volume) => SetVolume(MUSIC_PARAM, MUSIC_PREF, volume);
    public void SetSFXVolume(float volume) => SetVolume(SFX_PARAM, SFX_PREF, volume);
    public void SetDubbingVolume(float volume) => SetVolume(DUBBING_PARAM, DUBBING_PREF, volume);

    private void SetVolume(string param, string pref, float volume)
    {
        float db = volume <= 0f ? -80f : Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20f;
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
