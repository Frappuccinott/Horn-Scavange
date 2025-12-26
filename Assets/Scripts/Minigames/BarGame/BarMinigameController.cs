using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class BarMinigameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private RectTransform barBackground;
    [SerializeField] private RectTransform greenZone;
    [SerializeField] private RectTransform movingLine;
    [SerializeField] private Image movingLineImage;

    [Header("Minigame Settings")]
    [SerializeField] private float lineSpeed = 300f;

    [Header("Success Settings")]
    [SerializeField] private GameObject[] npcFailZones;

    private bool isMinigameActive;
    private bool movingRight = true;
    private float minX, maxX;
    private float greenZoneMinX, greenZoneMaxX;
    private Vector2 currentLinePos;

    private void Start()
    {
        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        foreach (var zone in npcFailZones)
        {
            if (zone != null)
                zone.SetActive(false);
        }

        CalculateBounds();
    }

    private void CalculateBounds()
    {
        minX = -barBackground.rect.width / 2f;
        maxX = barBackground.rect.width / 2f;

        greenZoneMinX = greenZone.anchoredPosition.x - (greenZone.rect.width / 2f);
        greenZoneMaxX = greenZone.anchoredPosition.x + (greenZone.rect.width / 2f);

        currentLinePos = new Vector2(minX, movingLine.anchoredPosition.y);
        movingLine.anchoredPosition = currentLinePos;
    }

    private void Update()
    {
        if (isMinigameActive)
            MoveLine();
    }

    private void MoveLine()
    {
        float movement = lineSpeed * Time.unscaledDeltaTime;

        if (movingRight)
        {
            currentLinePos.x += movement;
            if (currentLinePos.x >= maxX)
            {
                currentLinePos.x = maxX;
                movingRight = false;
            }
        }
        else
        {
            currentLinePos.x -= movement;
            if (currentLinePos.x <= minX)
            {
                currentLinePos.x = minX;
                movingRight = true;
            }
        }

        movingLine.anchoredPosition = currentLinePos;
    }

    public void StartMinigame()
    {
        isMinigameActive = true;
        minigamePanel.SetActive(true);

        currentLinePos = new Vector2(minX, movingLine.anchoredPosition.y);
        movingLine.anchoredPosition = currentLinePos;
        movingRight = true;

        Time.timeScale = 0f;

        if (InputManager.controls != null)
        {
            InputManager.controls.Character.Disable();
            InputManager.controls.Minigame.Enable();
            StartCoroutine(EnableMinigameInputAfterDelay());
        }
    }

    private IEnumerator EnableMinigameInputAfterDelay()
    {
        yield return null;

        if (InputManager.controls != null)
            InputManager.controls.Minigame.MinigameInteract.performed += OnMinigameInteract;
    }

    private void OnMinigameInteract(InputAction.CallbackContext context)
    {
        if (isMinigameActive)
            CheckSuccess();
    }

    private void CheckSuccess()
    {
        float lineX = movingLine.anchoredPosition.x;

        if (lineX >= greenZoneMinX && lineX <= greenZoneMaxX)
            OnMinigameSuccess();
        else
            OnMinigameFail();
    }

    private void OnMinigameSuccess()
    {
        EndMinigame();

        Time.timeScale = 1f;

        CollectableTaskManager.Instance?.OnCollected(CollectableType.GoldenHorn);
        AudioManager.Instance?.PlayGoldenHorn();

        ActivateNPCFailZones();
    }

    private void OnMinigameFail()
    {
        EndMinigame();
        FailPanelManager.Instance?.ShowFailPanel();
    }

    private void EndMinigame()
    {
        isMinigameActive = false;
        minigamePanel.SetActive(false);

        if (InputManager.controls != null)
        {
            InputManager.controls.Minigame.MinigameInteract.performed -= OnMinigameInteract;
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
        if (InputManager.controls != null)
            InputManager.controls.Minigame.MinigameInteract.performed -= OnMinigameInteract;
    }
}