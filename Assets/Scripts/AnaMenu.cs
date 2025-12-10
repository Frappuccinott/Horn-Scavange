using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AnaMenu : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;

    public void PLAY()
    {
        SceneManager.LoadScene("Game");
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
   