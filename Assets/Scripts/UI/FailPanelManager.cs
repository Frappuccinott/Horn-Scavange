using UnityEngine;
using UnityEngine.SceneManagement;

public class FailPanelManager : MonoBehaviour
{
    public static FailPanelManager Instance { get; private set; }

    [SerializeField] private GameObject FailPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (FailPanel != null)
            FailPanel.SetActive(false);
    }

    public void ShowFailPanel()
    {
        if (FailPanel != null)
        {
            FailPanel.SetActive(true);
            Time.timeScale = 0f;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (InputManager.controls != null)
                InputManager.controls.Disable();
        }
    }

    public void HideFailPanel()
    {
        if (FailPanel != null)
            FailPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}