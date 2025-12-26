using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ButtonClickMinigameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private Button clickButton;
    [SerializeField] private TextMeshProUGUI clickCountText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Settings")]
    [SerializeField] private int targetClicks = 20;
    [SerializeField] private float timeLimit = 5f;
    [SerializeField] private GameObject[] npcFailZones;

    [Header("Visual Feedback")]
    [SerializeField] private float buttonPressScale = 0.9f;
    [SerializeField] private float buttonPressDuration = 0.1f;

    private bool isMinigameActive = false;
    private int currentClicks = 0;
    private float remainingTime;
    private Vector3 buttonOriginalScale;
    private Coroutine buttonAnimCoroutine;

    private void Start()
    {
        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        foreach (var zone in npcFailZones)
        {
            if (zone != null)
                zone.SetActive(false);
        }

        if (clickButton != null)
        {
            clickButton.onClick.AddListener(OnButtonClick);
            buttonOriginalScale = clickButton.transform.localScale;
        }
    }

    public void StartMinigame()
    {
        isMinigameActive = true;
        currentClicks = 0;
        remainingTime = timeLimit;

        if (minigamePanel != null)
            minigamePanel.SetActive(true);

        UpdateUI();
        Time.timeScale = 0f;

        if (InputManager.controls != null)
        {
            InputManager.controls.Character.Disable();
            InputManager.controls.Minigame.Enable();
            StartCoroutine(EnableMinigameInputAfterDelay());
        }

        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator EnableMinigameInputAfterDelay()
    {
        yield return null;
        if (InputManager.controls != null)
        {
            InputManager.controls.Minigame.MinigameInteract.performed += OnMinigameInteract;
        }
    }

    private void OnMinigameInteract(InputAction.CallbackContext context)
    {
        if (isMinigameActive)
            OnButtonClick();
    }

    private void OnButtonClick()
    {
        if (!isMinigameActive) return;

        currentClicks++;
        UpdateUI();
        PlayButtonAnimation();

        if (currentClicks >= targetClicks)
        {
            OnMinigameSuccess();
        }
    }

    private void PlayButtonAnimation()
    {
        if (clickButton == null) return;

        if (buttonAnimCoroutine != null)
            StopCoroutine(buttonAnimCoroutine);

        buttonAnimCoroutine = StartCoroutine(ButtonPressAnimation());
    }

    private IEnumerator ButtonPressAnimation()
    {
        Transform buttonTransform = clickButton.transform;
        float elapsed = 0f;
        float halfDuration = buttonPressDuration / 2f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / halfDuration;
            buttonTransform.localScale = Vector3.Lerp(buttonOriginalScale, buttonOriginalScale * buttonPressScale, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / halfDuration;
            buttonTransform.localScale = Vector3.Lerp(buttonOriginalScale * buttonPressScale, buttonOriginalScale, t);
            yield return null;
        }

        buttonTransform.localScale = buttonOriginalScale;
    }

    private IEnumerator TimerCoroutine()
    {
        while (isMinigameActive && remainingTime > 0)
        {
            yield return null;
            remainingTime -= Time.unscaledDeltaTime;
            UpdateUI();

            if (remainingTime <= 0)
            {
                OnMinigameFail();
            }
        }
    }

    private void UpdateUI()
    {
        if (clickCountText != null)
            clickCountText.text = $"{currentClicks}/{targetClicks}";

        if (timerText != null)
            timerText.text = $"{Mathf.Max(0, remainingTime):F2}s";
    }

    private void OnMinigameSuccess()
    {
        isMinigameActive = false;
        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (InputManager.controls != null)
        {
            InputManager.controls.Minigame.MinigameInteract.performed -= OnMinigameInteract;
            InputManager.controls.Minigame.Disable();
            InputManager.controls.Character.Enable();
        }

        Time.timeScale = 1f;
        CollectableTaskManager.Instance?.OnCollected(CollectableType.GoldenHorn);
        AudioManager.Instance?.PlayGoldenHorn();
        ActivateNPCFailZones();
    }

    private void OnMinigameFail()
    {
        isMinigameActive = false;
        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (InputManager.controls != null)
        {
            InputManager.controls.Minigame.MinigameInteract.performed -= OnMinigameInteract;
            InputManager.controls.Minigame.Disable();
        }

        FailPanelManager.Instance?.ShowFailPanel();
    }

    private void ActivateNPCFailZones()
    {
        foreach (var zone in npcFailZones)
        {
            if (zone != null)
                zone.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (clickButton != null)
            clickButton.onClick.RemoveListener(OnButtonClick);

        if (InputManager.controls != null)
        {
            InputManager.controls.Minigame.MinigameInteract.performed -= OnMinigameInteract;
        }
    }
}