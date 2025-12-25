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

    [Header("Blink Settings")]
    [SerializeField] private float blinkSpeed = 2f;

    [SerializeField] private string keyName;

    private CanvasGroup canvasGroup;
    private Coroutine blinkCoroutine;
    private InputAction interactAction;

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

        interactAction = action;

        // Input action'dan tuþ ismini al
        //string keyName = GetKeyName(action);
        //promptText.text = $"Press {keyName} to interact";
        promptText.text = $"{keyName}";

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

    private string GetKeyName(InputAction action)
    {
        if (action == null || action.bindings.Count == 0)
            return "E";

        // Ýlk binding'i al
        string bindingPath = action.bindings[0].effectivePath;

        // Basitleþtir
        if (bindingPath.Contains("keyboard"))
        {
            string key = bindingPath.Replace("<Keyboard>/", "").ToUpper();
            return key;
        }
        else if (bindingPath.Contains("gamepad"))
        {
            if (bindingPath.Contains("buttonWest"))
                return "Square (PlayStation) / X (Xbox)";
            else if (bindingPath.Contains("buttonSouth"))
                return "Cross (PlayStation) / A (Xbox)";
            else
                return bindingPath.Replace("<Gamepad>/", "").ToUpper();
        }

        return "E";
    }

    private IEnumerator BlinkEffect()
    {
        if (canvasGroup == null) yield break;

        while (true)
        {
            // Fade out
            while (canvasGroup.alpha > 0.3f)
            {
                canvasGroup.alpha -= Time.deltaTime * blinkSpeed;
                yield return null;
            }

            // Fade in
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime * blinkSpeed;
                yield return null;
            }
        }
    }
}