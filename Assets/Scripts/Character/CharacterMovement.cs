using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private float originalSpeed;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    private Vector2 lastMovement;

    private void Awake()
    {
        originalSpeed = moveSpeed;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastMovement = Vector2.down;
    }

    private void Update()
    {
        if (InputManager.controls == null) return;

        // Read input
        movement = InputManager.controls.Character.Move.ReadValue<Vector2>();

        // Track last non-zero direction for idle animations
        if (movement != Vector2.zero)
        {
            lastMovement = movement.normalized;
        }

        // Update animator parameters
        anim.SetFloat("moveX", lastMovement.x);
        anim.SetFloat("moveY", lastMovement.y);
        anim.SetFloat("speed", movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        // Apply movement using physics
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void ModifySpeed(float multiplier)
    {
        moveSpeed = originalSpeed * multiplier;
    }

    public void ResetSpeed()
    {
        moveSpeed = originalSpeed;
    }
}