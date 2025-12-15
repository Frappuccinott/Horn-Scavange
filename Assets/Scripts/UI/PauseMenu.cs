using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel Referansları")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    private bool isPaused = false;
    private bool pauseWasActiveBeforeSettings = false;

    private CharacterControls inputActions;
    private InputAction pauseAction;

    private void Awake()
    {
        inputActions = new CharacterControls();
        pauseAction = inputActions.UI.Pause;
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

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        // Ayarlar açıksa önce onu kapat
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

    public void PauseGame()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
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
            isPaused = true;
            Time.timeScale = 0f;
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1f;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Oyun kapatıldı!");
        Application.Quit();
    }
    public void BackMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ClosePauseMenu()
    {
        pausePanel.SetActive(false);
    }
}
