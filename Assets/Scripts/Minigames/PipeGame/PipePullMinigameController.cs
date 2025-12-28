using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PipePullMinigameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private RectTransform cursor;
    [SerializeField] private RectTransform pipe;
    [SerializeField] private RectTransform item;
    [SerializeField] private RectTransform targetZone;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Settings")]
    [SerializeField] private float cursorSpeed = 500f;
    [SerializeField] private float pullSpeed = 200f;
    [SerializeField] private float itemStartY = -200f;
    [SerializeField] private float grabRadius = 50f;
    [SerializeField] private float timeLimit = 10f;
    [SerializeField] private GameObject[] npcFailZones;

    private bool isMinigameActive = false;
    private bool isGrabbing = false;
    private Vector2 cursorInput;
    private Vector2 itemStartPos;
    private float remainingTime;

    private void Start()
    {
        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        foreach (var zone in npcFailZones)
        {
            if (zone != null)
                zone.SetActive(false);
        }
    }

    public void StartMinigame()
    {
        isMinigameActive = true;
        isGrabbing = false;
        remainingTime = timeLimit;

        if (minigamePanel != null)
            minigamePanel.SetActive(true);

        if (item != null)
        {
            itemStartPos = new Vector2(item.anchoredPosition.x, itemStartY);
            item.anchoredPosition = itemStartPos;
        }

        if (cursor != null)
            cursor.anchoredPosition = Vector2.zero;

        UpdateTimerUI();
        Time.timeScale = 0f;

        if (InputManager.controls != null)
        {
            InputManager.controls.Character.Disable();
            InputManager.controls.Minigame.Enable();
            InputManager.controls.Minigame.CursorMove.performed += OnCursorMove;
            InputManager.controls.Minigame.CursorMove.canceled += OnCursorMove;
            InputManager.controls.Minigame.MinigameInteract.performed += OnGrabPressed;
            InputManager.controls.Minigame.MinigameInteract.canceled += OnGrabReleased;
        }

        StartCoroutine(TimerCoroutine());
    }

    private void OnCursorMove(InputAction.CallbackContext context)
    {
        cursorInput = context.ReadValue<Vector2>();
    }

    private void OnGrabPressed(InputAction.CallbackContext context)
    {
        if (!isMinigameActive || cursor == null || item == null) return;

        float distance = Vector2.Distance(cursor.anchoredPosition, item.anchoredPosition);
        if (distance <= grabRadius)
        {
            isGrabbing = true;
        }
    }

    private void OnGrabReleased(InputAction.CallbackContext context)
    {
        isGrabbing = false;
    }

    private void Update()
    {
        if (!isMinigameActive) return;

        MoveCursor();

        if (isGrabbing)
        {
            PullItem();
        }

        CheckSuccess();
    }

    private void MoveCursor()
    {
        if (cursor == null || pipe == null) return;

        Vector2 movement = cursorInput * cursorSpeed * Time.unscaledDeltaTime;
        Vector2 newPos = cursor.anchoredPosition + movement;

        float halfWidth = (pipe.rect.width / 2f) - 30f;
        float halfHeight = (pipe.rect.height / 2f) - 30f;
        newPos.x = Mathf.Clamp(newPos.x, -halfWidth, halfWidth);
        newPos.y = Mathf.Clamp(newPos.y, -halfHeight, halfHeight);

        cursor.anchoredPosition = newPos;
    }

    private void PullItem()
    {
        if (item == null || cursor == null) return;

        Vector2 direction = (cursor.anchoredPosition - item.anchoredPosition).normalized;
        Vector2 movement = direction * pullSpeed * Time.unscaledDeltaTime;
        Vector2 newPos = item.anchoredPosition + movement;

        newPos.y = Mathf.Max(newPos.y, itemStartPos.y);
        item.anchoredPosition = newPos;
    }

    private IEnumerator TimerCoroutine()
    {
        while (isMinigameActive && remainingTime > 0)
        {
            yield return null;
            remainingTime -= Time.unscaledDeltaTime;
            UpdateTimerUI();

            if (remainingTime <= 0)
            {
                OnMinigameFail();
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = $"{Mathf.Max(0, remainingTime):F2}s";
    }

    private void CheckSuccess()
    {
        if (!isMinigameActive || item == null || targetZone == null) return;

        Rect itemRect = GetWorldRect(item);
        Rect targetRect = GetWorldRect(targetZone);

        if (itemRect.Overlaps(targetRect))
        {
            OnMinigameSuccess();
        }
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float yMin = corners[0].y;
        float xMax = corners[2].x;
        float yMax = corners[2].y;

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    private void OnMinigameSuccess()
    {
        isMinigameActive = false;
        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        CleanupInputs();

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

        CleanupInputs();
        FailPanelManager.Instance?.ShowFailPanel();
    }

    private void CleanupInputs()
    {
        if (InputManager.controls != null)
        {
            InputManager.controls.Minigame.CursorMove.performed -= OnCursorMove;
            InputManager.controls.Minigame.CursorMove.canceled -= OnCursorMove;
            InputManager.controls.Minigame.MinigameInteract.performed -= OnGrabPressed;
            InputManager.controls.Minigame.MinigameInteract.canceled -= OnGrabReleased;
            InputManager.controls.Minigame.Disable();
            InputManager.controls.Character.Enable();
        }
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
        CleanupInputs();
    }
}