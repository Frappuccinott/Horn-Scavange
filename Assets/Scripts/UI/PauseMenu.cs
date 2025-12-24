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
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        SetPauseState(false);
    }

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

    private void SetPauseState(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        isPaused = pause;

        Cursor.visible = pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void PauseGame()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(true);
        settingsPanel?.SetActive(false);
        ingamePanel?.SetActive(false);
        SetPauseState(true);
    }

    public void ResumeGame()
    {
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        ingamePanel?.SetActive(true);
        SetPauseState(false);
    }

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

    public void ClosePauseMenu() => ResumeGame();

    public void BackMainMenu()
    {
        Time.timeScale = 1f;
        inputActions?.Disable();

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