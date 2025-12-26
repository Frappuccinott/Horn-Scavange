using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectableTaskManager : MonoBehaviour
{
    public static CollectableTaskManager Instance;

    [System.Serializable]
    public class CollectableTask
    {
        public CollectableType type;
        public int count;
        public TextMeshProUGUI text;
    }

    [Header("Level Dubbing")]
    [SerializeField] private AudioClip firstTrashDubbing;
    [SerializeField] private string firstTrashSubtitle;
    [SerializeField] private AudioClip goldenHornDubbing;
    [SerializeField] private string goldenHornSubtitle;

    [Header("Subtitle UI")]
    [SerializeField] private GameObject subtitlePanel;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private CanvasGroup subtitleCanvasGroup;
    [SerializeField] private float padding = 40f;
    [SerializeField] private float minWidth = 200f;
    [SerializeField] private float fadeDuration = 0.2f;

    [Header("Tasks")]
    public List<CollectableTask> tasks = new();

    private Dictionary<CollectableType, CollectableTask> taskDict;
    private Queue<DubbingRequest> dubbingQueue = new();
    private bool isDubbingPlaying;
    private bool firstTrashCollected;
    private bool goldenHornCollected;

    private struct DubbingRequest
    {
        public AudioClip clip;
        public string subtitle;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeTasks();

        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (subtitleCanvasGroup != null) subtitleCanvasGroup.alpha = 0f;
    }

    private void InitializeTasks()
    {
        taskDict = new Dictionary<CollectableType, CollectableTask>();

        foreach (var task in tasks)
        {
            taskDict[task.type] = task;
            if (task.text != null)
                task.text.text = $"{task.type}: {task.count}";
        }
    }

    public void OnCollected(CollectableType type, bool countOnly = false)
    {
        if (!taskDict.TryGetValue(type, out var task)) return;

        // Dubbing tetikle (sadece bir kez)
        if (!countOnly)
        {
            if (!firstTrashCollected &&
                (type == CollectableType.InstantTrash || type == CollectableType.CarryTrash))
            {
                firstTrashCollected = true;
                EnqueueDubbing(firstTrashDubbing, firstTrashSubtitle);
            }
            else if (!goldenHornCollected && type == CollectableType.GoldenHorn)
            {
                goldenHornCollected = true;
                EnqueueDubbing(goldenHornDubbing, goldenHornSubtitle);
            }
        }

        // Sayacı güncelle
        if (countOnly || type == CollectableType.InstantTrash || type == CollectableType.GoldenHorn)
        {
            task.count = Mathf.Max(0, task.count - 1);
            if (task.text != null)
                task.text.text = $"{task.type}: {task.count}";
        }
    }

    private void EnqueueDubbing(AudioClip clip, string subtitle)
    {
        if (clip == null || string.IsNullOrEmpty(subtitle)) return;

        dubbingQueue.Enqueue(new DubbingRequest { clip = clip, subtitle = subtitle });

        if (!isDubbingPlaying)
            StartCoroutine(ProcessDubbingQueue());
    }

    private IEnumerator ProcessDubbingQueue()
    {
        isDubbingPlaying = true;

        while (dubbingQueue.Count > 0)
        {
            var request = dubbingQueue.Dequeue();
            yield return PlayDubbingWithSubtitle(request.clip, request.subtitle);
        }

        isDubbingPlaying = false;
    }

    private IEnumerator PlayDubbingWithSubtitle(AudioClip clip, string subtitle)
    {
        AudioSource source = AudioManager.Instance.PlayDubbing(clip);

        // Altyazıyı göster
        subtitlePanel.SetActive(true);
        subtitleText.text = subtitle;

        // Arka plan genişliğini ayarla
        float width = Mathf.Max(subtitleText.preferredWidth + padding * 2f, minWidth);
        subtitlePanel.GetComponent<Image>().rectTransform
            .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        yield return FadeSubtitle(0f, 1f);
        yield return new WaitWhile(() => source != null && source.isPlaying);
        yield return FadeSubtitle(1f, 0f);

        subtitlePanel.SetActive(false);
    }

    private IEnumerator FadeSubtitle(float from, float to)
    {
        float elapsed = 0f;
        subtitleCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            subtitleCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        subtitleCanvasGroup.alpha = to;
    }

    public bool AreAllTasksCompleted()
    {
        foreach (var task in tasks)
            if (task.count > 0) return false;

        return true;
    }
}