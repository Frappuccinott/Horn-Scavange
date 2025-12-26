using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteract : MonoBehaviour
{
    private ICollectable currentTarget;

    private void OnEnable()
    {
        if (InputManager.controls != null)
        {
            InputManager.controls.Character.Interact.performed += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (InputManager.controls != null)
        {
            InputManager.controls.Character.Interact.performed -= OnInteract;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        currentTarget?.Interact();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICollectable collectible = GetCollectible(collision);
        if (collectible != null)
        {
            currentTarget = collectible;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ICollectable collectible = GetCollectible(collision);
        if (collectible != null && collectible == currentTarget)
        {
            currentTarget = null;
        }
    }

    private ICollectable GetCollectible(Collider2D collision)
    {
        return collision.GetComponent<ICollectable>()
            ?? collision.GetComponentInParent<ICollectable>()
            ?? collision.GetComponentInChildren<ICollectable>();
    }
}