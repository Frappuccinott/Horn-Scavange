using System.Collections;
using UnityEngine;

public class DeliveryZone : MonoBehaviour, ICollectable
{
    [SerializeField] private CollectableType type = CollectableType.CarryTrash;
    [SerializeField] private LoadingScreenController loadingController;
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    public bool IsPlayerInside { get; private set; }

    private CharacterCarrySystem carrySystem;
    private Coroutine blinkRoutine;
    private bool isBlinking;
    private bool isLoadingStarted;

    public CollectableType GetCollectableType() => type;

    private void Awake()
    {
        carrySystem = FindFirstObjectByType<CharacterCarrySystem>();

        if (panelGroup != null)
        {
            panelGroup.gameObject.SetActive(true);
            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (panelGroup == null || isLoadingStarted) return;

        bool allTasksComplete = CollectableTaskManager.Instance?.AreAllTasksCompleted() ?? false;
        bool shouldBlink = IsPlayerInside && allTasksComplete;

        if (shouldBlink && !isBlinking)
            StartBlink();
        else if (!shouldBlink && isBlinking)
            StopBlink();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
            IsPlayerInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
            IsPlayerInside = false;
    }

    public void Interact()
    {
        if (isLoadingStarted) return;

        if (CollectableTaskManager.Instance?.AreAllTasksCompleted() ?? false)
        {
            isLoadingStarted = true;
            StopBlink();
            loadingController?.StartLoading();
            return;
        }

        // Çöp taşınmıyorsa ses çal
        if (carrySystem == null || !carrySystem.IsCarrying)
        {
            AudioManager.Instance?.PlayCannotPickup();
            return;
        }

        // Çöpü teslim et
        carrySystem.GetCarriedTrash()?.TryDeliver();
    }

    private void StartBlink()
    {
        if (isBlinking) return;

        isBlinking = true;
        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    private void StopBlink()
    {
        if (!isBlinking) return;

        isBlinking = false;
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }
        panelGroup.alpha = 0f;
    }

    private IEnumerator BlinkRoutine()
    {
        while (isBlinking)
        {
            yield return Fade(0f, 1f);
            yield return new WaitForSeconds(blinkInterval);
            yield return Fade(1f, 0f);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            panelGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        panelGroup.alpha = to;
    }
}