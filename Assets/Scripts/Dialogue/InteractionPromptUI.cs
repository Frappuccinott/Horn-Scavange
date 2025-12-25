using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class InteractionPromptUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Settings")]
    [SerializeField] private string keyName;
    [SerializeField] private float blinkSpeed = 2f;

    private CanvasGroup canvasGroup;
    private Coroutine blinkCoroutine;

    private void Awake()
    {
        if (promptPanel != null)
        {
            canvasGroup = promptPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = promptPanel.AddComponent<CanvasGroup>();
        }

        HidePrompt();
    }

    public void ShowPrompt(InputAction action)
    {
        if (promptPanel == null || promptText == null) return;

        promptText.text = keyName;
        promptPanel.SetActive(true);

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        blinkCoroutine = StartCoroutine(BlinkEffect());
    }

    public void HidePrompt()
    {
        if (promptPanel == null) return;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        promptPanel.SetActive(false);
    }

    private IEnumerator BlinkEffect()
    {
        if (canvasGroup == null) yield break;

        while (true)
        {
            while (canvasGroup.alpha > 0.3f)
            {
                canvasGroup.alpha -= Time.deltaTime * blinkSpeed;
                yield return null;
            }

            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime * blinkSpeed;
                yield return null;
            }
        }
    }
}