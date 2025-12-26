using UnityEngine;

public class AnaMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Loading")]
    [SerializeField] private LoadingScreenController loadingController;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    public void Play()
    {
        loadingController.StartLoading();
    }

    public void ExitGame() => Application.Quit();

    public void OpenSetPanel() => settingsPanel?.SetActive(true);
    public void CloseSetPanel() => settingsPanel?.SetActive(false);
    public void OpenCreditsPanel() => creditsPanel?.SetActive(true);
    public void CloseCreditsPanel() => creditsPanel?.SetActive(false);
    public void OpenControlsPanel() => controlsPanel?.SetActive(true);
    public void CloseControlsPanel() => controlsPanel?.SetActive(false);
}
