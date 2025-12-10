using UnityEngine;

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
        // Check if character is carrying trash
        if (carrySystem == null || !carrySystem.IsCarrying)
        {
            AudioManager.Instance?.PlaySound(AudioManager.Instance.cannotPickupSound);
            return;
        }

        // Deliver the carried trash
        var trash = carrySystem.GetCarriedTrash();
        if (trash != null)
        {
            trash.TryDeliver();
        }
    }
}