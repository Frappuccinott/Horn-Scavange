using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel Referanslarý")]
    public GameObject pausePanel;
    public GameObject settingsPanel;


    private bool isPaused = false;
    private bool pauseWasActiveBeforeSettings = false;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        bool escPressed = false;

        if (!escPressed)
            escPressed = Input.GetKeyUp(KeyCode.Escape);
        if (escPressed)
        {
            // Önce ayarlar açýksa onu kapat
            if (settingsPanel != null && !settingsPanel.activeSelf)
            {
                CloseSettings();
                return;
            }

            // Deðilse pause aç/kapat
            if (isPaused)
                ResumeGame();

            else
                PauseGame();
        }
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
        Debug.Log("Settings panel açýlýyor..."); // Konsolda kontrol et
        if (settingsPanel == null || pausePanel == null)
        {
            Debug.LogWarning("Settings veya Pause panel referansý eksik!");
            return;
        }

        pauseWasActiveBeforeSettings = pausePanel.activeSelf;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
        settingsPanel.transform.SetAsLastSibling();

        Debug.Log("Settings panel aktif: " + settingsPanel.activeSelf);
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
        Debug.Log("Oyun kapatýldý!");
        Application.Quit();
    }

}



