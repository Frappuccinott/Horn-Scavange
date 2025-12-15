using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteract : MonoBehaviour
{
    private ICollectable currentTarget;

    private void OnEnable()
    {
        if (InputManager.controls == null) return;

        // Unsubscribe first to prevent duplicate subscriptions
        InputManager.controls.Character.Interact.started -= OnInteract;
        InputManager.controls.Character.Interact.started += OnInteract;
    }

    private void OnDisable()
    {
        if (InputManager.controls == null) return;
        InputManager.controls.Character.Interact.started -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    // Fallback input method for testing
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentTarget != null)
            {
                currentTarget.Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for ICollectable in: self -> parent -> children
        var collectible = collision.GetComponent<ICollectable>()
                       ?? collision.GetComponentInParent<ICollectable>()
                       ?? collision.GetComponentInChildren<ICollectable>();

        if (collectible != null)
        {
            currentTarget = collectible;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check for ICollectable in: self -> parent -> children
        var collectible = collision.GetComponent<ICollectable>()
                       ?? collision.GetComponentInParent<ICollectable>()
                       ?? collision.GetComponentInChildren<ICollectable>();

        // Clear target only if it matches the exiting collider
        if (collectible != null && collectible == currentTarget)
        {
            currentTarget = null;
        }
    }
}