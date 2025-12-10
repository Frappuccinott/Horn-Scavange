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

//using UnityEngine;
//using UnityEngine.InputSystem;
//public class CharacterInteract : MonoBehaviour
//{
//    private ICollectable currentTarget;
//    private void OnEnable()
//    {
//        if (InputManager.controls == null) return;
//        InputManager.controls.Character.Interact.started -= OnInteract;
//        InputManager.controls.Character.Interact.started += OnInteract;
//    }
//    private void OnDisable()
//    {
//        if (InputManager.controls == null) return;
//        InputManager.controls.Character.Interact.started -= OnInteract;
//    }
//    private void OnInteract(InputAction.CallbackContext context)
//    {
//        Debug.Log("E pressed in CharacterInteract");
//        if (currentTarget != null)
//        {
//            Debug.Log("Interacting with: " + ((MonoBehaviour)currentTarget).name);
//            currentTarget.Interact();
//        }
//        else
//        {
//            Debug.Log("No target to interact with");
//        }
//    }
//    // ✅ GEÇİCİ TEST - Keyboard ile de dene
//    private void Update()
//    {
//        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
//        {
//            Debug.Log("E tuşu algılandı (Keyboard.current)");
//            if (currentTarget != null)
//            {
//                Debug.Log("Manuel Interact: " + ((MonoBehaviour)currentTarget).name);
//                currentTarget.Interact();
//            }
//            else
//            {
//                Debug.Log("Manuel - No target");
//            }
//        }
//    }
//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        Debug.Log("Trigger entered with: " + collision.gameObject.name);
//        // Önce direkt objeye bak
//        var collectible = collision.GetComponent<ICollectable>();
//        // Yoksa parent'a bak
//        if (collectible == null)
//        {
//            collectible = collision.GetComponentInParent<ICollectable>();
//        }
//        // Hala yoksa child'lara bak
//        if (collectible == null)
//        {
//            collectible = collision.GetComponentInChildren<ICollectable>();
//        }
//        if (collectible != null)
//        {
//            currentTarget = collectible;
//            Debug.Log("Target SET: " + ((MonoBehaviour)collectible).name);
//        }
//        else
//        {
//            Debug.Log("No ICollectable found on: " + collision.gameObject.name);
//        }
//    }
//    private void OnTriggerExit2D(Collider2D collision)
//    {
//        Debug.Log("Trigger exited with: " + collision.gameObject.name);
//        var collectible = collision.GetComponent<ICollectable>();
//        if (collectible == null)
//            collectible = collision.GetComponentInParent<ICollectable>();
//        if (collectible == null)
//            collectible = collision.GetComponentInChildren<ICollectable>();
//        if (collectible != null && collectible == currentTarget)
//        {
//            currentTarget = null;
//            Debug.Log("Target CLEARED");
//        }
//    }
//}
