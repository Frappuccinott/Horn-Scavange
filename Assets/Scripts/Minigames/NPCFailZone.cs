using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class NPCFailZone : MonoBehaviour
{
    [Header("Fail Settings")]
    [SerializeField] private float failDelay = 0.5f;

    [Header("Visual Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color normalColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private Color warningColor = new Color(1f, 0f, 0f, 0.7f);

    [Header("Animation Settings")]
    [SerializeField] private float animationDistance = 7f;
    [SerializeField] private float distanceCheckInterval = 0.15f;
    [SerializeField] private bool alwaysAnimate = false;

    private bool isCharacterInside;
    private Transform characterTransform;
    private Coroutine failCoroutine;
    private Coroutine warningAnimCoroutine;
    private Coroutine distanceCheckCoroutine;
    private bool isAnimating;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (spriteRenderer != null)
        {
            if (spriteRenderer.sprite == null)
                CreateDefaultSprite();

            spriteRenderer.color = normalColor;
        }

        GameObject characterObj = GameObject.FindGameObjectWithTag("Character");
        if (characterObj != null)
            characterTransform = characterObj.transform;
    }

    private void CreateDefaultSprite()
    {
        Texture2D tex = new Texture2D(100, 100);
        Color[] pixels = new Color[10000];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;

        tex.SetPixels(pixels);
        tex.Apply();

        spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f), 100f);
    }

    private void OnEnable()
    {
        if (spriteRenderer != null)
        {
            if (spriteRenderer.sprite == null)
                CreateDefaultSprite();

            spriteRenderer.color = normalColor;
        }

        if (distanceCheckCoroutine != null)
            StopCoroutine(distanceCheckCoroutine);

        distanceCheckCoroutine = StartCoroutine(CheckDistanceRoutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Character"))
        {
            isCharacterInside = true;

            if (failCoroutine != null)
                StopCoroutine(failCoroutine);

            failCoroutine = StartCoroutine(FailDelayCoroutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Character"))
        {
            isCharacterInside = false;

            if (failCoroutine != null)
            {
                StopCoroutine(failCoroutine);
                failCoroutine = null;
            }
        }
    }

    private IEnumerator CheckDistanceRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(distanceCheckInterval);

        while (true)
        {
            bool shouldAnimate = alwaysAnimate ||
                (characterTransform != null && Vector2.Distance(transform.position, characterTransform.position) <= animationDistance);

            if (shouldAnimate && !isAnimating)
                StartWarningAnimation();
            else if (!shouldAnimate && isAnimating)
                StopWarningAnimation();

            yield return wait;
        }
    }

    private void StartWarningAnimation()
    {
        isAnimating = true;

        if (warningAnimCoroutine != null)
            StopCoroutine(warningAnimCoroutine);

        warningAnimCoroutine = StartCoroutine(WarningAnimation());
    }

    private void StopWarningAnimation()
    {
        isAnimating = false;

        if (warningAnimCoroutine != null)
        {
            StopCoroutine(warningAnimCoroutine);
            warningAnimCoroutine = null;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;
    }

    private IEnumerator FailDelayCoroutine()
    {
        yield return new WaitForSeconds(failDelay);

        if (isCharacterInside)
            FailPanelManager.Instance?.ShowFailPanel();

        failCoroutine = null;
    }

    private IEnumerator WarningAnimation()
    {
        const float duration = 0.25f;

        while (isAnimating)
        {
            // Normal -> Warning
            yield return AnimateColor(normalColor, warningColor, duration);

            // Warning -> Normal
            yield return AnimateColor(warningColor, normalColor, duration);
        }
    }

    private IEnumerator AnimateColor(Color from, Color to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration && isAnimating)
        {
            elapsed += Time.deltaTime;
            if (spriteRenderer != null)
                spriteRenderer.color = Color.Lerp(from, to, elapsed / duration);

            yield return null;
        }
    }

    private void OnDisable()
    {
        if (failCoroutine != null)
        {
            StopCoroutine(failCoroutine);
            failCoroutine = null;
        }

        if (distanceCheckCoroutine != null)
        {
            StopCoroutine(distanceCheckCoroutine);
            distanceCheckCoroutine = null;
        }

        if (warningAnimCoroutine != null)
        {
            StopCoroutine(warningAnimCoroutine);
            warningAnimCoroutine = null;
        }

        isCharacterInside = false;
        isAnimating = false;
    }
}