using UnityEngine;

public class InstantTrash : CollectableBase
{
    public override void Interact()
    {
        AudioManager.Instance?.PlayRandomInstantTrash();
        CollectableTaskManager.Instance?.OnCollected(type);
        Destroy(gameObject);
    }
}