using UnityEngine;

/// <summary>
/// Çöp teslim alaný. Karakter elindeki çöpü burada teslim edebilir.
/// </summary>
public class DeliveryZone : MonoBehaviour, ICollectable
{
    [SerializeField] private CollectableType type = CollectableType.CarryTrash;
    public bool IsPlayerInside { get; private set; }
    private CharacterCarrySystem carrySystem;

    public CollectableType GetCollectableType() => type;

    private void Awake()
    {
        carrySystem = Object.FindFirstObjectByType<CharacterCarrySystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsPlayerInside = false;
        }
    }

    public void Interact()
    {
        // Elinde çöp yoksa teslim edilemez
        if (carrySystem == null || !carrySystem.IsCarrying)
        {
            AudioManager.Instance?.PlayCannotPickup();
            return;
        }

        // Çöpü teslim et
        var trash = carrySystem.GetCarriedTrash();
        if (trash != null)
        {
            trash.TryDeliver();
        }
    }
}