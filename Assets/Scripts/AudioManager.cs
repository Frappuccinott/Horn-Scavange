using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource source;

    [Header("Pickup Sounds")]
    public AudioClip instantTrashPickup;
    public AudioClip carryTrashPickup;
    public AudioClip goldenHornPickup;

    [Header("Delivery Sounds")]
    public AudioClip carryTrashDeliver;

    [Header("Error / Feedback Sounds")]
    public AudioClip cannotPickupSound;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0f; // 2D ses
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        source.PlayOneShot(clip);
    }
}
