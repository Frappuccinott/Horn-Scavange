using UnityEngine;

public class CharacterCarrySystem : MonoBehaviour
{
    private CarryTrash carriedTrash;

    [Header("Movement")]
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private float slowMultiplier = 0.6f;

    [Header("Carry Point")]
    [SerializeField] private Transform carryPoint;

    public bool IsCarrying => carriedTrash != null;

    private void Start()
    {
        if (movement == null)
            movement = GetComponent<CharacterMovement>();

        if (carryPoint == null)
        {
            GameObject carryObj = new GameObject("CarryPoint");
            carryPoint = carryObj.transform;
            carryPoint.SetParent(transform);
            carryPoint.localPosition = new Vector3(0, 0.5f, 0);
        }
    }

    public bool PickUp(CarryTrash trash)
    {
        if (carriedTrash != null) return false;

        carriedTrash = trash;

        if (movement != null)
            movement.ModifySpeed(slowMultiplier);

        return true;
    }

    public void DeliverCarriedTrash()
    {
        if (carriedTrash == null) return;

        carriedTrash = null;

        if (movement != null)
            movement.ResetSpeed();
    }

    public Transform GetCarryPoint() => carryPoint;
    public CarryTrash GetCarriedTrash() => carriedTrash;
}