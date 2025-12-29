using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalVideoManager : MonoBehaviour
{
    [Header("Final NPC")]
    [SerializeField] private GameObject cingan;

    [Header("Video")]
    [SerializeField] private VideoClip finalVideoClip;

    [Header("UI")]
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private RawImage videoRawImage;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("Fade")]
    [SerializeField] private float cameraFadeDuration = 0.5f;
    [SerializeField] private float videoEndFadeDuration = 2f;

    [Header("Scene")]
    [SerializeField] private int mainMenuSceneIndex = 0;

    private VideoPlayer videoPlayer;
    private RenderTexture renderTexture;
    private bool sequenceStarted;

    private void Awake()
    {
        if (videoPanel != null)
            videoPanel.SetActive(false);

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;
    }

    public void PlayFinalVideo()
    {
        if (sequenceStarted) return;
        sequenceStarted = true;

        StartCoroutine(FinalSequence());
    }

    private IEnumerator FinalSequence()
    {
        yield return FadeToBlack(cameraFadeDuration);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeOutMusic(0.5f);
            yield return new WaitForSeconds(0.5f);
        }

        SetupVideo();
        yield return WaitForPrepared();

        videoPanel.SetActive(true);
        videoPlayer.Play();

        yield return new WaitForSeconds(0.5f);
        if (cingan != null)
            cingan.SetActive(false);

        yield return FadeFromBlack(1f);

        while (videoPlayer.isPlaying)
            yield return null;

        yield return FadeToBlack(videoEndFadeDuration);

        CleanupVideo();
        CreateFinalTxt();
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    private void SetupVideo()
    {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.clip = finalVideoClip;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

        int w = finalVideoClip.width > 0 ? (int)finalVideoClip.width : 1920;
        int h = finalVideoClip.height > 0 ? (int)finalVideoClip.height : 1080;

        renderTexture = new RenderTexture(w, h, 0);
        renderTexture.Create();

        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;

        videoRawImage.texture = renderTexture;

        RectTransform rt = videoRawImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        videoPlayer.Prepare();
    }

    private IEnumerator WaitForPrepared()
    {
        float t = 0f;
        while (!videoPlayer.isPrepared && t < 10f)
        {
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeToBlack(float duration)
    {
        float t = 0f;
        float start = fadeCanvasGroup.alpha;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(start, 1f, t / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeFromBlack(float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
    }

    private void CleanupVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            Destroy(videoPlayer);
        }

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }

    private void CreateFinalTxt()
    {
        string documentsPath = System.Environment.GetFolderPath(
            System.Environment.SpecialFolder.MyDocuments
        );

        string path = Path.Combine(documentsPath, "final.txt");

        File.WriteAllText(path, "To be continued\n- Scavanges Team");
    }

    private void OnDestroy()
    {
        CleanupVideo();
    }
}
