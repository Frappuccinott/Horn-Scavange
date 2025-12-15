using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AnaMenu : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenSetPanel()
    {
        SettingsPanel.SetActive(true);
    }

    public void CloseSetPanel()
    {
        SettingsPanel.SetActive(false);
    }

    public void OpenCreditsPanel()
    {
        CreditsPanel.SetActive(true);
    }

    public void CloseCreditsPanel()
    {
        CreditsPanel.SetActive(false);
    }

}

//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class AnaMenu : MonoBehaviour
//{
//    [SerializeField] private GameObject settingsPanel;
//    [SerializeField] private GameObject creditsPanel;

//    // Oyun baþlatma ve çýkýþ
//    public void Play() => SceneManager.LoadScene(1);
//    public void ExitGame() => Application.Quit();

//    // Panel yönetimi
//    public void OpenSetPanel() => SetPanelActive(settingsPanel, true);
//    public void CloseSetPanel() => SetPanelActive(settingsPanel, false);
//    public void OpenCreditsPanel() => SetPanelActive(creditsPanel, true);
//    public void CloseCreditsPanel() => SetPanelActive(creditsPanel, false);

//    private void SetPanelActive(GameObject panel, bool active) => panel?.SetActive(active);
//}