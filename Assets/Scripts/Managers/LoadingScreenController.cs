using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class LoadingScreenController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loadingPanel;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Truck")]
    [SerializeField] private RectTransform truck;
    [SerializeField] private float truckMoveDuration = 3f;

    [Header("Sound")]
    [SerializeField] private AudioSource engineSource;
    [SerializeField] private AudioClip engineClip;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.3f;

    [Header("Text")]
    [SerializeField] private TMP_Text continueTMP;
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private string continueMessage = "Press E to continue";
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float textFadeDuration = 0.5f;
    [SerializeField] private float blinkSpeed = 0.6f;

    [Header("Scene")]
    [SerializeField] private string nextSceneName;

    private bool canContinue;
    private bool blinkText;
    private bool isDestroying;

    private readonly Vector2 truckStartPos = new(-390, -150);
    private readonly Vector2 truckEndPos = new(1550, -150);

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartLoading()
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        StartCoroutine(LoadingSequence());
    }

    private IEnumerator LoadingSequence()
    {
        ResetState();

        yield return Fade(0f, 1f);

        loadingPanel.SetActive(true);

        yield return Fade(1f, 0f);

        PlayEngineSound();

        yield return MoveTruckWithPitch();
        yield return StopSoundAfterDelay(0.5f);
        yield return TypeText();
        yield return FadeTextCanvasGroup(0f, 1f, textFadeDuration);

        blinkText = true;
        StartCoroutine(BlinkText());

        canContinue = true;
    }

    private void ResetState()
    {
        canContinue = false;
        blinkText = false;
        loadingPanel.SetActive(false);
        fadeGroup.alpha = 0f;
        continueTMP.text = "";
        continueTMP.alpha = 1f;
        textCanvasGroup.alpha = 0f;
        truck.anchoredPosition = truckStartPos;
    }

    private IEnumerator MoveTruckWithPitch()
    {
        float elapsed = 0f;

        while (elapsed < truckMoveDuration && !isDestroying)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / truckMoveDuration);

            truck.anchoredPosition = Vector2.Lerp(truckStartPos, truckEndPos, t);
            engineSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);

            yield return null;
        }

        if (!isDestroying)
            truck.anchoredPosition = truckEndPos;
    }

    private IEnumerator TypeText()
    {
        continueTMP.text = "";

        foreach (char c in continueMessage)
        {
            if (isDestroying) yield break;
            continueTMP.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator FadeTextCanvasGroup(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration && !isDestroying)
        {
            elapsed += Time.deltaTime;
            textCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        if (!isDestroying)
            textCanvasGroup.alpha = to;
    }

    private IEnumerator BlinkText()
    {
        float halfBlink = blinkSpeed * 0.5f;

        while (blinkText && !isDestroying)
        {
            yield return FadeTMPAlpha(1f, 0f, halfBlink);
            if (!blinkText || isDestroying) break;
            yield return FadeTMPAlpha(0f, 1f, halfBlink);
        }
    }

    private IEnumerator FadeTMPAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration && !isDestroying)
        {
            elapsed += Time.deltaTime;
            continueTMP.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        if (!isDestroying)
            continueTMP.alpha = to;
    }

    private void PlayEngineSound()
    {
        engineSource.clip = engineClip;
        engineSource.loop = true;
        engineSource.pitch = minPitch;
        engineSource.Play();
    }

    private IEnumerator StopSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isDestroying && engineSource != null)
        {
            engineSource.Stop();
            engineSource.pitch = minPitch;
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration && !isDestroying)
        {
            elapsed += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        if (!isDestroying)
            fadeGroup.alpha = to;
    }

    private void Update()
    {
        if (canContinue && !isDestroying && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            canContinue = false;
            blinkText = false;
            StartCoroutine(LoadSceneAsync());
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        isDestroying = true;

        yield return Fade(0f, 1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;
        yield return new WaitForSeconds(0.1f);

        if (this != null && gameObject != null)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        isDestroying = true;
        StopAllCoroutines();
    }
}