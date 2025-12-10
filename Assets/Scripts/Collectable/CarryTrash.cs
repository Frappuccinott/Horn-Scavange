using UnityEngine;

/// <summary>
/// Taşınabilir çöp nesnesi. Karakter tarafından alınıp teslim edilebilir.
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
                AudioManager.Instance?.PlaySound(AudioManager.Instance.cannotPickupSound);
            }
        }
    }

    private void PickupTrash()
    {
        isCarried = true;
        carrySystem.PickUp(this);

        // Karakterin eline yapıştır
        transform.SetParent(carrySystem.transform);
        transform.localPosition = new Vector3(0, 0.5f, 0);

        // Collider'ı kapat (tekrar toplanmasın)
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        AudioManager.Instance?.PlaySound(AudioManager.Instance.carryTrashPickup);
    }

    /// <summary>
    /// DeliveryZone tarafından çağrılır
    /// </summary>
    public void TryDeliver()
    {
        if (!isCarried) return;

        AudioManager.Instance?.PlaySound(AudioManager.Instance.carryTrashDeliver);
        carrySystem.DeliverCarriedTrash();
        Destroy(gameObject);

        isCarried = false;
    }

    public bool IsCarried() => isCarried;
}