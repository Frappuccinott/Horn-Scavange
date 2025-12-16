using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    private static CharacterControls _controls;

    public static CharacterControls controls => _controls;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (_controls == null)
        {
            _controls = new CharacterControls();
        }

        _controls.Character.Enable();
    }

    private void OnEnable()
    {
        _controls?.Character.Enable();
    }

    private void OnDisable()
    {
        if (_instance == this && Application.isPlaying) return;
        _controls?.Character.Disable();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _controls?.Dispose();
            _controls = null;
            _instance = null;
        }
    }
}