//using UnityEngine;

//public class CarryTrash : MonoBehaviour, ICollectable
//{
//    [SerializeField] private CollectableType type = CollectableType.CarryTrash;

//    private bool isCarried = false;
//    private CharacterCarrySystem carrySystem;

//    public CollectableType GetCollectableType() => type;

//    private void Awake()
//    {
//        carrySystem = Object.FindFirstObjectByType<CharacterCarrySystem>();
//    }

//    public void Interact()
//    {
//        if (!isCarried)
//        {
//            if (carrySystem != null && !carrySystem.IsCarrying)
//            {
//                PickupTrash();
//            }
//            else
//            {
//                AudioManager.Instance?.PlayCannotPickup();
//            }
//        }
//    }

//    private void PickupTrash()
//    {
//        isCarried = true;
//        carrySystem.PickUp(this);

//        Transform carryPoint = carrySystem.GetCarryPoint();
//        transform.SetParent(carryPoint);
//        transform.localPosition = Vector3.zero;
//        transform.localRotation = Quaternion.identity;

//        var col = GetComponent<Collider2D>();
//        if (col != null) col.enabled = false;

//        CollectableTaskManager.Instance?.OnCollected(type, countOnly: false);
//        AudioManager.Instance?.PlayRandomCarryPickup();
//    }

//    public void TryDeliver()
//    {
//        if (!isCarried) return;

//        AudioManager.Instance?.PlayRandomCarryDeliver();
//        CollectableTaskManager.Instance?.OnCollected(type, countOnly: true);
//        carrySystem.DeliverCarriedTrash();
//        Destroy(gameObject);
//        isCarried = false;
//    }

//    public bool IsCarried() => isCarried;
//}

using UnityEngine;

public class CarryTrash : CollectableBase
{
    private bool isCarried = false;
    private CharacterCarrySystem carrySystem;

    private void Awake()
    {
        carrySystem = Object.FindFirstObjectByType<CharacterCarrySystem>();
    }

    public override void Interact()
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

        Transform carryPoint = carrySystem.GetCarryPoint();
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        CollectableTaskManager.Instance?.OnCollected(type, countOnly: false);
        AudioManager.Instance?.PlayRandomCarryPickup();
    }

    public void TryDeliver()
    {
        if (!isCarried) return;

        AudioManager.Instance?.PlayRandomCarryDeliver();
        CollectableTaskManager.Instance?.OnCollected(type, countOnly: true);
        carrySystem.DeliverCarriedTrash();
        Destroy(gameObject);
        isCarried = false;
    }

    public bool IsCarried() => isCarried;
}