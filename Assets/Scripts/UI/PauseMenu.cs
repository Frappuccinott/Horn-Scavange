using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel Referansları")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    private bool isPaused;
    private bool pauseWasActiveBeforeSettings;
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
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        SetPauseState(false);
    }

    // Input yönetimi
    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (settingsPanel?.activeSelf == true)
        {
            CloseSettings();
            return;
        }

        if (isPaused) ResumeGame();
        else PauseGame();
    }

    // Pause durumu kontrolü
    private void SetPauseState(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        isPaused = pause;
    }

    public void PauseGame()
    {
        if (!pausePanel) return;
        pausePanel.SetActive(true);
        settingsPanel?.SetActive(false);
        SetPauseState(true);
    }

    public void ResumeGame()
    {
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        SetPauseState(false);
    }

    // Ayarlar menüsü
    public void OpenSettings()
    {
        if (!settingsPanel || !pausePanel) return;
        pauseWasActiveBeforeSettings = pausePanel.activeSelf;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
        settingsPanel.transform.SetAsLastSibling();
    }

    public void CloseSettings()
    {
        if (!settingsPanel || !pausePanel) return;
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame() => Application.Quit();
}