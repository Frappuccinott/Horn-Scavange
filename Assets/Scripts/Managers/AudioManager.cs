using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer mixer;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup dubbingGroup;

    private AudioSource sfxSource;
    private AudioSource dubbingSource;

    [Header("Instant Trash SFX (Random)")]
    public AudioClip[] instantTrashPickups;

    [Header("Carry Trash SFX (Random)")]
    public AudioClip[] carryTrashPickups;
    public AudioClip[] carryTrashDelivers;

    [Header("Golden Horn SFX")]
    public AudioClip goldenHornPickup;

    [Header("Error / Feedback")]
    public AudioClip cannotPickupSound;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateSources();
    }

    private void CreateSources()
    {
        // SFX source for game sounds
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;
        sfxSource.outputAudioMixerGroup = sfxGroup;

        // Dubbing source for voice/dialog
        dubbingSource = gameObject.AddComponent<AudioSource>();
        dubbingSource.playOnAwake = false;
        dubbingSource.spatialBlend = 0f;
        dubbingSource.outputAudioMixerGroup = dubbingGroup;
    }

    // Play random instant trash pickup sound
    public void PlayRandomInstantTrash()
    {
        PlayRandomFromArray(instantTrashPickups);
    }

    // Play random carry trash pickup sound
    public void PlayRandomCarryPickup()
    {
        PlayRandomFromArray(carryTrashPickups);
    }

    // Play random carry trash delivery sound
    public void PlayRandomCarryDeliver()
    {
        PlayRandomFromArray(carryTrashDelivers);
    }

    // Play golden horn pickup sound
    public void PlayGoldenHornSFX()
    {
        PlaySFX(goldenHornPickup);
    }

    // Play error/cannot pickup sound
    public void PlayCannotPickup()
    {
        PlaySFX(cannotPickupSound);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    private void PlayRandomFromArray(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        sfxSource.PlayOneShot(clips[index]);
    }

    // Play voice/dialog clip (stops previous dubbing to prevent overlap)
    public void PlayDubbing(AudioClip clip)
    {
        if (clip == null) return;

        dubbingSource.Stop();
        dubbingSource.PlayOneShot(clip);
    }

    // Set SFX volume from UI slider (0-1 range)
    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
    }

    // Set dubbing volume from UI slider (0-1 range)
    public void SetDubbingVolume(float value)
    {
        mixer.SetFloat("DubbingVolume", Mathf.Log10(value) * 20f);
    }
}