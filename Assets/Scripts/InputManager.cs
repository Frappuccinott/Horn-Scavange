using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    private static CharacterControls _controls;

    public static CharacterControls controls => _controls;

    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize input controls
        _controls = new CharacterControls();
    }

    private void OnEnable()
    {
        _controls?.Character.Enable();
    }

    private void OnDisable()
    {
        _controls?.Character.Disable();
    }

    private void OnDestroy()
    {
        _controls?.Dispose();
    }
}