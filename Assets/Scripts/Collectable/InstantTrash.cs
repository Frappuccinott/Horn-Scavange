using UnityEngine;

public class InstantTrash : MonoBehaviour, ICollectable
{
    [SerializeField] private CollectableType type = CollectableType.InstantTrash;

    public CollectableType GetCollectableType()
    {
        return type;
    }

    public void Interact()
    {
        // Play pickup sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.instantTrashPickup);
        }

        // Notify game manager
        // TODO: GameManager.Instance.OnInstantTrashCollected();

        // Destroy object
        Destroy(gameObject);
    }
}