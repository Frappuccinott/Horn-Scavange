using UnityEngine;

/// <summary>
/// Anlık toplanan çöp. Dokunulduğunda hemen yok olur.
/// </summary>
public class InstantTrash : MonoBehaviour, ICollectable
{
    [SerializeField] private CollectableType type = CollectableType.InstantTrash;

    public CollectableType GetCollectableType() => type;

    public void Interact()
    {
        // Random ses çal
        AudioManager.Instance?.PlayRandomInstantTrash();

        // Task manager'a bildir
        CollectableTaskManager.Instance?.OnCollected(type);

        // Objeyi yok et
        Destroy(gameObject);
    }
}