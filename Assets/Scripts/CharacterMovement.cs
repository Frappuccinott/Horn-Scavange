using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    Rigidbody2D rb;
    Animator anim;
    Vector2 movement;
    Vector2 lastMovement; // Son hareket yönünü sakla
    PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastMovement = Vector2.down; // Başlangıç yönü (aşağı)
    }

    void Update()
    {
        movement = controls.Player.Move.ReadValue<Vector2>();

        // Eğer hareket varsa, son hareketi güncelle
        if (movement != Vector2.zero)
        {
            lastMovement = movement.normalized;
        }

        // Animator parametrelerini ayarla
        anim.SetFloat("moveX", lastMovement.x);
        anim.SetFloat("moveY", lastMovement.y);
        anim.SetFloat("speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}