//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class AnaMenu : MonoBehaviour
//{
//    public void Play()
//    {
//        // Time scale ve cursor reset, güvenlik için
//        Time.timeScale = 1f;
//        Cursor.visible = false;
//        Cursor.lockState = CursorLockMode.Locked;

//        // Level sahnesini yükle
//        SceneManager.LoadScene(1); // Level sahne index veya adý
//    }

//    public void ExitGame()
//    {
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//        Application.Quit();
//#endif      
//    }
//}


using UnityEngine;
using UnityEngine.SceneManagement;

public class AnaMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Awake()
    {
        // Ana menüye döndüðünde time scale'i sýfýrla
        Time.timeScale = 1f;
    }

    // Oyun baþlatma ve çýkýþ
    public void Play() => SceneManager.LoadScene(1);
    public void ExitGame() => Application.Quit();

    // Panel yönetimi
    public void OpenSetPanel() => SetPanelActive(settingsPanel, true);
    public void CloseSetPanel() => SetPanelActive(settingsPanel, false);
    public void OpenCreditsPanel() => SetPanelActive(creditsPanel, true);
    public void CloseCreditsPanel() => SetPanelActive(creditsPanel, false);
    private void SetPanelActive(GameObject panel, bool active) => panel?.SetActive(active);
}