using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private float offsetY = 1.2f;

    private Transform player;
    private Transform deliveryVehicle;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Character")?.transform;

        if (player == null)
        {
            Debug.LogError("DirectionArrow: Player with 'Character' tag not found!");
        }

        GameObject vehicle = GameObject.FindGameObjectWithTag("DeliveryVehicle");
        if (vehicle != null)
        {
            deliveryVehicle = vehicle.transform;
        }
        else
        {
            Debug.LogError("DirectionArrow: Delivery vehicle with 'DeliveryVehicle' tag not found!");
        }
    }

    private void LateUpdate()
    {
        if (player == null || deliveryVehicle == null) return;

        Vector3 arrowPosition = player.position;
        arrowPosition.y += offsetY;
        transform.position = arrowPosition;

        Vector3 direction = deliveryVehicle.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}