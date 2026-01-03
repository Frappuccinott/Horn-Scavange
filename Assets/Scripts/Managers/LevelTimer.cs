using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeLimitMinutes = 7f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject failPanel;

    private float currentTime;
    private bool timerRunning = true;

    private void Start()
    {
        currentTime = timeLimitMinutes * 60f;

        if (failPanel != null)
            failPanel.SetActive(false);

        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (!timerRunning) return;

        currentTime -= Time.deltaTime;
        UpdateTimerDisplay();

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            TimeUp();
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = string.Format("Remaining: {0:00}:{1:00}", minutes, seconds);

        if (currentTime <= 60f)
            timerText.color = Color.red;
    }

    private void TimeUp()
    {
        timerRunning = false;

        if (failPanel != null)
            failPanel.SetActive(true);

        Time.timeScale = 0f;

        if (InputManager.controls != null)
            InputManager.controls.Character.Disable();
    }

    public void StopTimer()
    {
        timerRunning = false;
    }
}