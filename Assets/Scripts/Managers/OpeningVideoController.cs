using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class OpeningVideoController : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private GameObject videoPanel;

    [Header("Skip UI")]
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private CanvasGroup skipTextCanvasGroup;
    [SerializeField] private string skipMessage = "Press E or Square to skip";
    [SerializeField] private float skipTextFadeInDuration = 0.5f;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Loading")]
    [SerializeField] private LoadingScreenController loadingController;

    private RenderTexture renderTexture;
    private bool canSkip;
    private bool isSkipping;
    private bool videoEnded;
    private bool inputSubscribed;

    private void Awake()
    {
        if (videoPanel != null)
            videoPanel.SetActive(false);

        if (fadeGroup != null)
            fadeGroup.alpha = 0f;

        if (skipText != null)
        {
            skipText.text = skipMessage;
            skipText.gameObject.SetActive(true);
        }

        if (skipTextCanvasGroup != null)
            skipTextCanvasGroup.alpha = 0f;

        CreateRenderTexture();
    }

    private void OnEnable()
    {
        if (InputManager.controls == null) return;

        InputManager.controls.Character.Interact.performed += OnSkipPressed;
        inputSubscribed = true;
    }

    private void OnDisable()
    {
        if (!inputSubscribed || InputManager.controls == null) return;

        InputManager.controls.Character.Interact.performed -= OnSkipPressed;
        inputSubscribed = false;
    }

    private void CreateRenderTexture()
    {
        if (videoPlayer == null || rawImage == null) return;

        RectTransform rt = rawImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        renderTexture = new RenderTexture(1920, 1080, 0);
        renderTexture.Create();

        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        rawImage.texture = renderTexture;
    }
public void PlayOpeningVideo()
{
    if (videoPlayer == null || videoPlayer.clip == null || loadingController == null)
    {
        loadingController?.StartLoading();
        return;
    }

    InputManager.controls.UI.Disable();
    InputManager.controls.Character.Enable();

    EventSystem.current.sendNavigationEvents = false;
    EventSystem.current.SetSelectedGameObject(null);

    StartCoroutine(VideoSequence());
}



private IEnumerator VideoSequence()
    {
        if (AnaMenuAudioManager.Instance != null)
            AnaMenuAudioManager.Instance.StopMusic();

        if (videoPanel != null)
            videoPanel.SetActive(true);

        SetupVideoPlayer();

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        videoPlayer.Play();
        yield return null;

        if (skipTextCanvasGroup != null)
            yield return FadeSkipText(0f, 1f, skipTextFadeInDuration);

        canSkip = true;

        while (videoPlayer.isPlaying && !isSkipping && !videoEnded)
            yield return null;

        if (skipTextCanvasGroup != null)
            yield return FadeSkipText(1f, 0f, 0.2f);

        if (!isSkipping)
            yield return FadeOut();

        StartLoadingScreen();
    }

    private void SetupVideoPlayer()
    {
        if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
            AudioSource src = videoPlayer.GetComponent<AudioSource>();
            if (src == null)
                src = videoPlayer.gameObject.AddComponent<AudioSource>();

            videoPlayer.SetTargetAudioSource(0, src);

            if (AudioManager.Instance != null)
                AudioManager.Instance.SetVideoAudioSource(src);
        }

        videoPlayer.loopPointReached += OnVideoEnded;
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        videoEnded = true;
        canSkip = false;
    }

    private void OnSkipPressed(InputAction.CallbackContext ctx)
    {
        if (!canSkip || isSkipping) return;

        isSkipping = true;
        canSkip = false;
        StartCoroutine(SkipVideo());
    }

    private IEnumerator SkipVideo()
    {
        if (skipTextCanvasGroup != null)
            yield return FadeSkipText(1f, 0f, 0.2f);

        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();

        yield return FadeOut();
        StartLoadingScreen();
    }

    private IEnumerator FadeOut()
    {
        if (fadeGroup == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            yield return null;
        }

        fadeGroup.alpha = 1f;
    }

    private IEnumerator FadeSkipText(float from, float to, float duration)
    {
        if (skipTextCanvasGroup == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            skipTextCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        skipTextCanvasGroup.alpha = to;
    }

    private void StartLoadingScreen()
    {
        StartCoroutine(StartLoadingAndCleanup());
    }

    private IEnumerator StartLoadingAndCleanup()
    {
        if (skipTextCanvasGroup != null)
            skipTextCanvasGroup.alpha = 0f;

        loadingController.StartLoading();
        yield return new WaitForSeconds(0.1f);

        if (videoPanel != null)
            videoPanel.SetActive(false);

        CleanupRenderTexture();
        Destroy(gameObject);
    }

    private void CleanupRenderTexture()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnded;
            if (videoPlayer.isPlaying)
                videoPlayer.Stop();
        }

        CleanupRenderTexture();
        StopAllCoroutines();
    }
}
