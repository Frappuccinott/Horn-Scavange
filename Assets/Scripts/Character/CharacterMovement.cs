using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float footstepCooldown = 0.15f;

    private float originalSpeed;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    private Vector2 lastMovement;
    private const float deadZone = 0.1f;

    private float lastFootstepTime = -999f;

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

    public void PlayFootstep()
    {
        if (movement.magnitude < deadZone) return;

        if (Time.time - lastFootstepTime < footstepCooldown)
        {
            return;
        }

        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("CharacterMovement: AudioManager not found!");
            return;
        }

        if (footstepSounds == null || footstepSounds.Length == 0)
        {
            Debug.LogWarning("CharacterMovement: No footstep sounds assigned!");
            return;
        }

        int randomIndex = Random.Range(0, footstepSounds.Length);
        AudioClip footstepClip = footstepSounds[randomIndex];

        AudioManager.Instance.PlayFootstep(footstepClip);

        lastFootstepTime = Time.time;
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