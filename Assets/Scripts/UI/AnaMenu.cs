using UnityEngine;
using UnityEngine.SceneManagement;

public class AnaMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    public void Play() => SceneManager.LoadScene(1);
    public void ExitGame() => Application.Quit();

    public void OpenSetPanel() => settingsPanel?.SetActive(true);
    public void CloseSetPanel() => settingsPanel?.SetActive(false);
    public void OpenCreditsPanel() => creditsPanel?.SetActive(true);
    public void CloseCreditsPanel() => creditsPanel?.SetActive(false);
}