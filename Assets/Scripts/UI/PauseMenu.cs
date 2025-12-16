using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel Referansları")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject ingamePanel;

    private bool isPaused;
    private bool pauseWasActiveBeforeSettings;
    private CharacterControls inputActions;
    private InputAction pauseAction;

    private void Awake()
    {
        inputActions = new CharacterControls();
        pauseAction = inputActions.UI.Pause;

        // Level başladığında time scale'i garantiye al
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePressed;
        pauseAction.Disable();
    }

    private void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        SetPauseState(false);
    }

    // Input yönetimi
    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    // Pause durumu kontrolü
    private void SetPauseState(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        isPaused = pause;

        // Cursor yönetimi
        if (pause)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void PauseGame()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (ingamePanel != null) ingamePanel.SetActive(false);
        SetPauseState(true);
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (ingamePanel != null) ingamePanel.SetActive(true);
        SetPauseState(false);
    }

    // Ayarlar menüsü
    public void OpenSettings()
    {
        if (settingsPanel == null || pausePanel == null) return;

        pauseWasActiveBeforeSettings = pausePanel.activeSelf;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
        settingsPanel.transform.SetAsLastSibling();
    }

    public void CloseSettings()
    {
        if (settingsPanel == null || pausePanel == null) return;

        settingsPanel.SetActive(false);

        if (pauseWasActiveBeforeSettings)
        {
            pausePanel.SetActive(true);
            SetPauseState(true);
        }
        else
        {
            SetPauseState(false);
        }
    }

    // Menü işlemleri
    public void ClosePauseMenu() => ResumeGame();

    public void BackMainMenu()
    {
        // Time scale'i kesinlikle sıfırla
        Time.timeScale = 1f;

        // Input'ları temizle
        if (inputActions != null)
        {
            inputActions.Disable();
        }

        // Cursor'u düzelt
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}