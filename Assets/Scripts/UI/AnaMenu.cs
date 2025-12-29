using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnaMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Opening Video")]
    [SerializeField] private OpeningVideoController openingVideoController;

    [Header("Loading")]
    [SerializeField] private LoadingScreenController loadingController;

    [Header("Buttons")]
    [SerializeField] private Button playButton;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    public void Play()
    {
        if (playButton != null)
            playButton.interactable = false;

        EventSystem.current.SetSelectedGameObject(null);

        InputManager.InputHelper.DisableAll();

        if (openingVideoController != null)
            openingVideoController.PlayOpeningVideo();
        else
            loadingController?.StartLoading();
    }

    public void ExitGame() => Application.Quit();

    public void OpenSetPanel() => settingsPanel?.SetActive(true);
    public void CloseSetPanel() => settingsPanel?.SetActive(false);

    public void OpenCreditsPanel() => creditsPanel?.SetActive(true);
    public void CloseCreditsPanel() => creditsPanel?.SetActive(false);

    public void OpenControlsPanel() => controlsPanel?.SetActive(true);
    public void CloseControlsPanel() => controlsPanel?.SetActive(false);
}