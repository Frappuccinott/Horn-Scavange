using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI characterNameText;

    [Header("Animation Settings")]
    [SerializeField] private float fadeSpeed = 5f;

    private CanvasGroup canvasGroup;
    private Image fillImage;

    private void Awake()
    {
        if (dialoguePanel != null)
        {
            canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = dialoguePanel.AddComponent<CanvasGroup>();
        }

        CreateFillImageIfNeeded();
        HideDialogue();
    }

    private void CreateFillImageIfNeeded()
    {
        Transform existingFill = dialoguePanel?.transform.Find("FillImage");
        if (existingFill != null)
        {
            fillImage = existingFill.GetComponent<Image>();
            return;
        }

        if (dialoguePanel != null)
        {
            GameObject fillObj = new GameObject("FillImage");
            fillObj.transform.SetParent(dialoguePanel.transform, false);
            fillObj.transform.SetAsFirstSibling();

            fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(1, 1, 1, 0.1f);
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.fillAmount = 0f;

            RectTransform fillRect = fillImage.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
        }
    }

    public void ShowDialogue(string characterName, string text)
    {
        if (dialoguePanel == null || dialogueText == null) return;

        characterNameText.text = characterName;
        dialogueText.text = text;

        dialoguePanel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void HideDialogue()
    {
        if (dialoguePanel == null) return;

        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    public IEnumerator AnimateDialogueWithAudio(float duration)
    {
        if (fillImage == null) yield break;

        fillImage.fillAmount = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        fillImage.fillAmount = 1f;
    }

    private IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 0f;

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        if (canvasGroup == null) yield break;

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        dialoguePanel.SetActive(false);

        if (fillImage != null)
            fillImage.fillAmount = 0f;
    }
}