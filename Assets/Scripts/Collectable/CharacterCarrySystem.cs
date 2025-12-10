using UnityEngine;

public class CharacterCarrySystem : MonoBehaviour
{
    private CarryTrash carriedTrash;

    [Header("Movement")]
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private float slowMultiplier = 0.6f;

    public bool IsCarrying => carriedTrash != null;

    private void Start()
    {
        if (movement == null)
        {
            movement = GetComponent<CharacterMovement>();
        }
    }

    /// <summary>
    /// Pick up trash and slow down movement
    /// </summary>
    public bool PickUp(CarryTrash trash)
    {
        if (carriedTrash != null)
        {
            return false;
        }

        carriedTrash = trash;

        // Slow down movement speed
        if (movement != null)
        {
            movement.ModifySpeed(slowMultiplier);
        }

        return true;
    }

    /// <summary>
    /// Deliver carried trash and restore normal speed
    /// </summary>
    public void DeliverCarriedTrash()
    {
        if (carriedTrash == null)
        {
            return;
        }

        carriedTrash = null;

        // Restore normal speed
        if (movement != null)
        {
            movement.ResetSpeed();
        }
    }

    public CarryTrash GetCarriedTrash() => carriedTrash;
}