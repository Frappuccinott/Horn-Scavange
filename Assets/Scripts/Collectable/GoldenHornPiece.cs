using UnityEngine;

/// <summary>
/// Golden Horn parçasý. Toplanýnca özel ses çalar.
/// </summary>
public class GoldenHornPiece : MonoBehaviour, ICollectable
{
    [SerializeField] private CollectableType type = CollectableType.GoldenHorn;

    public CollectableType GetCollectableType() => type;

    public void Interact()
    {
        // Golden Horn sesi
        AudioManager.Instance?.PlayGoldenHorn();

        // Task manager'a bildir
        CollectableTaskManager.Instance?.OnCollected(type);

        // Objeyi yok et
        Destroy(gameObject);
    }
}