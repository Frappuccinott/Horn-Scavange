using UnityEngine;

/// <summary>
/// Carryable trash object that can be picked up and delivered by the character.
/// </summary>
public class CarryTrash : MonoBehaviour, ICollectable
{
    [SerializeField] private CollectableType type = CollectableType.CarryTrash;

    private bool isCarried = false;
    private CharacterCarrySystem carrySystem;

    public CollectableType GetCollectableType() => type;

    private void Awake()
    {
        carrySystem = Object.FindFirstObjectByType<CharacterCarrySystem>();
    }

    public void Interact()
    {
        if (!isCarried)
        {
            if (carrySystem != null && !carrySystem.IsCarrying)
            {
                PickupTrash();
            }
            else
            {
                AudioManager.Instance?.PlayCannotPickup();
            }
        }
    }

    private void PickupTrash()
    {
        isCarried = true;
        carrySystem.PickUp(this);

        // Attach to character's hand
        transform.SetParent(carrySystem.transform);
        transform.localPosition = new Vector3(0, 0.5f, 0);

        // Disable collider to prevent re-pickup
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Trigger first trash dubbing and play pickup sound
        CollectableTaskManager.Instance?.OnCollected(type, countOnly: false);
        AudioManager.Instance?.PlayRandomCarryPickup();
    }

    /// <summary>
    /// Called by DeliveryZone when trash is delivered
    /// </summary>
    public void TryDeliver()
    {
        if (!isCarried) return;

        AudioManager.Instance?.PlayRandomCarryDeliver();

        // Decrease task count (only on delivery)
        CollectableTaskManager.Instance?.OnCollected(type, countOnly: true);

        carrySystem.DeliverCarriedTrash();
        Destroy(gameObject);
        isCarried = false;
    }

    public bool IsCarried() => isCarried;
}