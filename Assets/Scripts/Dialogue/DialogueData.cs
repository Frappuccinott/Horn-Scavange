using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        [Header("Character Info")]
        public string characterName;

        [Header("Dialogue Content")]
        [TextArea(3, 6)]
        public string dialogueText;

        [Header("Audio")]
        public AudioClip voiceClip;

        [Header("Timing")]
        [Tooltip("Konuþma bitiminden sonra bekleme süresi (saniye)")]
        public float delayAfterLine = 1f;
    }

    [Header("Dialogue Lines")]
    public DialogueLine[] dialogueLines;
}