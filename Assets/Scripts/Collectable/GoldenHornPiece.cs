using UnityEngine;

public class GoldenHornPiece : MonoBehaviour, ICollectable
{
    [SerializeField] private CollectableType type = CollectableType.GoldenHorn;

    public CollectableType GetCollectableType()
    {
        return type;
    }

    public void Interact()
    {
        // Play pickup sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.goldenHornPickup);
        }

        // Notify game manager
        // TODO: GameManager.Instance.OnGoldenHornCollected();

        // Destroy object
        Destroy(gameObject);
    }
}