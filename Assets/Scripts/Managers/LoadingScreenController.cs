using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

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

    [Header("Text")]
    [SerializeField] private TMP_Text continueTMP;
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private string continueMessage = "Press Interact to continue";
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

    private void OnEnable()
    {
        InputManager.controls.Character.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        InputManager.controls.Character.Interact.performed -= OnInteract;
    }

    public void StartLoading()
    {
        InputManager.InputHelper.DisableAll();
        EventSystem.current.sendNavigationEvents = false;
        EventSystem.current.SetSelectedGameObject(null);

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

        yield return MoveTruck();
        yield return TypeText();
        yield return FadeTextCanvasGroup(0f, 1f, textFadeDuration);

        blinkText = true;
        StartCoroutine(BlinkText());

        // 🎮 SADECE Character aktif
        InputManager.InputHelper.EnableCharacter();

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

    private IEnumerator MoveTruck()
    {
        float elapsed = 0f;

        while (elapsed < truckMoveDuration && !isDestroying)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / truckMoveDuration);
            truck.anchoredPosition = Vector2.Lerp(truckStartPos, truckEndPos, t);
            yield return null;
        }

        truck.anchoredPosition = truckEndPos;
    }

    private IEnumerator TypeText()
    {
        continueTMP.text = "";

        foreach (char c in continueMessage)
        {
            continueTMP.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator FadeTextCanvasGroup(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        textCanvasGroup.alpha = to;
    }

    private IEnumerator BlinkText()
    {
        float half = blinkSpeed * 0.5f;

        while (blinkText)
        {
            yield return FadeTMPAlpha(1f, 0f, half);
            yield return FadeTMPAlpha(0f, 1f, half);
        }
    }

    private IEnumerator FadeTMPAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            continueTMP.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        continueTMP.alpha = to;
    }

    private void PlayEngineSound()
    {
        if (engineSource == null || engineClip == null) return;

        engineSource.clip = engineClip;
        engineSource.loop = true;
        engineSource.Play();
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = to;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!canContinue || isDestroying) return;

        canContinue = false;
        blinkText = false;
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        isDestroying = true;

        if (engineSource != null)
            engineSource.Stop();

        yield return Fade(0f, 1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();

        if (engineSource != null)
            engineSource.Stop();
    }
}
