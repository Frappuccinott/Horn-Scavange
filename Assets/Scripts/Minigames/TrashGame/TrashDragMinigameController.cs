using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TrashDragMinigameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private RectTransform cursor;
    [SerializeField] private RectTransform goldenHornRect;
    [SerializeField] private GameObject goldenHornObject;
    [SerializeField] private Button goldenHornButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private List<TrashItem> trashItems;

    [Header("Settings")]
    [SerializeField] private float cursorSpeed = 500f;
    [SerializeField] private float timeLimit = 15f;
    [SerializeField] private GameObject[] npcFailZones;

    private bool isMinigameActive = false;
    private bool isDragging = false;
    private TrashItem currentTrash = null;
    private Vector2 cursorInput;
    private float remainingTime;
    private int trashedCount = 0;

    private void Start()
    {
        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        foreach (var zone in npcFailZones)
        {
            if (zone != null)
                zone.SetActive(false);
        }

        if (goldenHornButton != null)
            goldenHornButton.onClick.AddListener(OnGoldenHornClicked);
    }

    public void StartMinigame()
    {
        isMinigameActive = true;
        isDragging = false;
        currentTrash = null;
        trashedCount = 0;
        remainingTime = timeLimit;

        if (minigamePanel != null)
            minigamePanel.SetActive(true);

        if (goldenHornObject != null)
            goldenHornObject.SetActive(true);

        if (goldenHornButton != null)
            goldenHornButton.interactable = false;

        foreach (var trash in trashItems)
        {
            if (trash != null)
            {
                trash.Initialize(this, goldenHornRect);
                trash.ResetPosition();
            }
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
        if (!isMinigameActive || isDragging) return;

        // Önce Golden Horn týklanabilir mi kontrol et
        if (goldenHornButton != null && goldenHornButton.interactable && cursor != null && goldenHornRect != null)
        {
            float distanceToGoldenHorn = Vector2.Distance(cursor.anchoredPosition, goldenHornRect.anchoredPosition);
            if (distanceToGoldenHorn <= 80f)
            {
                OnGoldenHornClicked();
                return;
            }
        }

        // Deðilse çöp tut
        if (cursor == null) return;

        foreach (var trash in trashItems)
        {
            if (trash == null || trash.IsCleared()) continue;

            float distance = Vector2.Distance(cursor.anchoredPosition, trash.GetPosition());
            if (distance <= 80f)
            {
                currentTrash = trash;
                isDragging = true;
                trash.StartDrag();
                break;
            }
        }
    }

    private void OnGrabReleased(InputAction.CallbackContext context)
    {
        if (!isDragging || currentTrash == null) return;

        isDragging = false;
        currentTrash.EndDrag();
        currentTrash = null;
    }

    private void Update()
    {
        if (!isMinigameActive) return;

        MoveCursor();

        if (isDragging && currentTrash != null)
        {
            currentTrash.DragTo(cursor.anchoredPosition);
        }
    }

    private void MoveCursor()
    {
        if (cursor == null) return;

        Vector2 movement = cursorInput * cursorSpeed * Time.unscaledDeltaTime;
        Vector2 newPos = cursor.anchoredPosition + movement;

        newPos.x = Mathf.Clamp(newPos.x, -350f, 350f);
        newPos.y = Mathf.Clamp(newPos.y, -230f, 230f);

        cursor.anchoredPosition = newPos;
    }

    public void OnTrashCleared()
    {
        trashedCount++;

        if (trashedCount >= trashItems.Count)
        {
            RevealGoldenHorn();
        }
    }

    private void RevealGoldenHorn()
    {
        if (goldenHornButton != null)
            goldenHornButton.interactable = true;
    }

    private void OnGoldenHornClicked()
    {
        if (!isMinigameActive) return;
        OnMinigameSuccess();
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
        if (goldenHornButton != null)
            goldenHornButton.onClick.RemoveListener(OnGoldenHornClicked);

        CleanupInputs();
    }
}