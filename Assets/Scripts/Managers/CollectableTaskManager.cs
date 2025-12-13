using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages collectable item tasks and voice-over triggers.
/// </summary>
public class CollectableTaskManager : MonoBehaviour
{
    public static CollectableTaskManager Instance;

    [Header("Level Dubbing")]
    [SerializeField] private AudioClip firstTrashDubbing;
    [SerializeField] private AudioClip goldenHornDubbing;

    private bool firstTrashCollected = false;
    private bool goldenHornCollected = false;

    [System.Serializable]
    public class CollectableTask
    {
        public CollectableType type;
        public int count;
        public TextMeshProUGUI text;
    }

    [Header("Tasks")]
    public List<CollectableTask> tasks = new();

    private Dictionary<CollectableType, CollectableTask> taskDict;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetupDictionary();
        UpdateAllTexts();
    }

    private void SetupDictionary()
    {
        taskDict = new Dictionary<CollectableType, CollectableTask>();
        foreach (var task in tasks)
        {
            taskDict[task.type] = task;
        }
    }

    private void UpdateAllTexts()
    {
        foreach (var task in tasks)
        {
            UpdateText(task);
        }
    }

    private void UpdateText(CollectableTask task)
    {
        if (task.text != null)
        {
            task.text.text = $"{task.type}: {task.count}";
        }
    }

    /// <summary>
    /// Called when a collectable is picked up or delivered.
    /// InstantTrash → Play dubbing and decrease count immediately
    /// CarryTrash → Play dubbing on pickup, decrease count on delivery
    /// GoldenHorn → Play dubbing and decrease count immediately
    /// </summary>
    /// <param name="type">The type of collectable</param>
    /// <param name="countOnly">If true, only decrease count without playing dubbing</param>
    public void OnCollected(CollectableType type, bool countOnly = false)
    {
        if (!taskDict.ContainsKey(type)) return;

        // Play dubbing (when countOnly = false)
        if (!countOnly)
        {
            // First trash dubbing (InstantTrash or CarryTrash)
            if (!firstTrashCollected &&
                (type == CollectableType.InstantTrash || type == CollectableType.CarryTrash))
            {
                firstTrashCollected = true;
                AudioManager.Instance?.PlayDubbing(firstTrashDubbing);
            }

            // Golden Horn dubbing
            if (!goldenHornCollected && type == CollectableType.GoldenHorn)
            {
                goldenHornCollected = true;
                AudioManager.Instance?.PlayDubbing(goldenHornDubbing);
            }
        }

        // Decrease count (when countOnly = true or for InstantTrash/GoldenHorn)
        if (countOnly || type == CollectableType.InstantTrash || type == CollectableType.GoldenHorn)
        {
            var task = taskDict[type];
            task.count = Mathf.Max(0, task.count - 1);
            UpdateText(task);
        }
    }
}