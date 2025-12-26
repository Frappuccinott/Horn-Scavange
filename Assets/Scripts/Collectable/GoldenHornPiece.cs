using UnityEngine;

public class GoldenHornPiece : CollectableBase
{
    public override void Interact()
    {
        AudioManager.Instance?.PlayGoldenHorn();
        CollectableTaskManager.Instance?.OnCollected(type);
        Destroy(gameObject);
    }
}