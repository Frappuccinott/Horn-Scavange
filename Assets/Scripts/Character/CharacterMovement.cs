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

    private const float deadZone = 0.1f;

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

        movement = InputManager.controls.Character.Move.ReadValue<Vector2>();

        if (movement.magnitude < deadZone)
            movement = Vector2.zero;

        if (movement != Vector2.zero)
            lastMovement = movement.normalized;

        anim.SetFloat("moveX", movement.x);
        anim.SetFloat("moveY", movement.y);
        anim.SetFloat("speed", movement.magnitude);
        anim.SetFloat("idleX", lastMovement.x);
        anim.SetFloat("idleY", lastMovement.y);
    }

    private void FixedUpdate()
    {
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