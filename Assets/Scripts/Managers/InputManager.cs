//using UnityEngine;

//public class InputManager : MonoBehaviour
//{
//    private static InputManager _instance;
//    private static CharacterControls _controls;

//    public static CharacterControls controls => _controls;

//    private void Awake()
//    {
//        // Singleton pattern
//        if (_instance != null && _instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        _instance = this;
//        DontDestroyOnLoad(gameObject);

//        // Initialize input controls
//        _controls = new CharacterControls();
//    }

//    private void OnEnable()
//    {
//        _controls?.Character.Enable();
//    }

//    private void OnDisable()
//    {
//        _controls?.Character.Disable();
//    }

//    private void OnDestroy()
//    {
//        _controls?.Dispose();
//    }
//}


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
        if (_controls == null)
        {
            _controls = new CharacterControls();
        }

        // Character map'i hemen aktif et
        _controls.Character.Enable();
    }

    private void OnEnable()
    {
        // Her enable'da Character map'in aktif olduðundan emin ol
        if (_controls != null)
        {
            _controls.Character.Enable();
        }
    }

    private void OnDisable()
    {
        // Sadece uygulama kapanýrken disable et
        if (_instance == this && Application.isPlaying)
        {
            // Sahne geçiþlerinde disable etme
            return;
        }

        _controls?.Character.Disable();
    }

    private void OnDestroy()
    {
        // Sadece gerçek instance destroy olurken temizle
        if (_instance == this)
        {
            _controls?.Dispose();
            _controls = null;
            _instance = null;
        }
    }

    // Debug için - Inspector'da görmek isterseniz
    private void OnGUI()
    {
        if (_controls != null)
        {
            GUI.Label(new Rect(10, 10, 300, 20),
                $"Character Enabled: {_controls.Character.enabled}");
        }
    }
}