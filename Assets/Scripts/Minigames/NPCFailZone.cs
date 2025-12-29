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

//using UnityEngine;
//using System.Collections;

//[RequireComponent(typeof(Collider2D))]
//public class NPCFailZone : MonoBehaviour
//{
//    [Header("Fail Settings")]
//    [SerializeField] private float failDelay = 0.5f; // 0.5 saniye

//    [Header("Visual Settings")]
//    [SerializeField] private SpriteRenderer spriteRenderer; // Görsel için sprite renderer
//    [SerializeField] private Color normalColor = new Color(1f, 0f, 0f, 0.3f); // Kýrmýzý, yarý saydam
//    [SerializeField] private Color warningColor = new Color(1f, 0f, 0f, 0.7f); // Daha koyu kýrmýzý

//    [Header("Animation Settings")]
//    [SerializeField] private float animationDistance = 7f; // Karakter bu mesafede ise animate olur
//    [SerializeField] private float distanceCheckInterval = 0.15f; // Mesafe kontrolü aralýðý (optimizasyon)
//    [SerializeField] private bool alwaysAnimate = false; // Test için: her zaman animate et

//    private bool isCharacterInside = false;
//    private Transform characterTransform;
//    private Coroutine failCoroutine;
//    private Coroutine warningAnimCoroutine;
//    private Coroutine distanceCheckCoroutine;
//    private bool isAnimating = false;

//    private void Start()
//    {
//        // Collider'ýn trigger olduðundan emin ol
//        Collider2D col = GetComponent<Collider2D>();
//        if (col != null)
//        {
//            col.isTrigger = true;
//        }
//        else
//        {
//            Debug.LogError($"NPCFailZone '{gameObject.name}' - Collider2D bulunamadý!");
//        }

//        // Sprite renderer varsa kontrol et
//        if (spriteRenderer != null)
//        {
//            // Sprite atanmamýþsa otomatik oluþtur
//            if (spriteRenderer.sprite == null)
//            {
//                Debug.LogWarning($"NPCFailZone '{gameObject.name}' - Sprite atanmamýþ! Otomatik basit sprite oluþturuluyor...");
//                CreateDefaultSprite();
//            }

//            spriteRenderer.color = normalColor;
//            Debug.Log($"NPCFailZone '{gameObject.name}' - SpriteRenderer rengi ayarlandý: {normalColor}, Sprite: {spriteRenderer.sprite?.name ?? "NULL"}");
//        }
//        else
//        {
//            Debug.LogWarning($"NPCFailZone '{gameObject.name}' - SpriteRenderer atanmamýþ! Inspector'da atamalýsýnýz.");
//        }

//        // Karakteri bul (sadece bir kere)
//        GameObject characterObj = GameObject.FindGameObjectWithTag("Character");
//        if (characterObj != null)
//        {
//            characterTransform = characterObj.transform;
//            Debug.Log($"NPCFailZone '{gameObject.name}' - Karakter bulundu, mesafe bazlý animasyon aktif.");
//        }
//        else
//        {
//            Debug.LogWarning($"NPCFailZone '{gameObject.name}' - 'Character' tag'li obje bulunamadý! Mesafe bazlý animasyon çalýþmayacak.");
//        }
//    }

//    private void CreateDefaultSprite()
//    {
//        // Basit beyaz kare texture oluþtur
//        Texture2D tex = new Texture2D(100, 100);
//        Color[] pixels = new Color[100 * 100];
//        for (int i = 0; i < pixels.Length; i++)
//        {
//            pixels[i] = Color.white;
//        }
//        tex.SetPixels(pixels);
//        tex.Apply();

//        // Sprite oluþtur
//        spriteRenderer.sprite = Sprite.Create(
//            tex,
//            new Rect(0, 0, 100, 100),
//            new Vector2(0.5f, 0.5f),
//            100f
//        );

//        Debug.Log($"NPCFailZone '{gameObject.name}' - Otomatik sprite oluþturuldu!");
//    }

//    private void OnEnable()
//    {
//        // Zone aktif olduðunda sprite'ý kontrol et
//        if (spriteRenderer != null)
//        {
//            // Sprite yoksa oluþtur
//            if (spriteRenderer.sprite == null)
//            {
//                CreateDefaultSprite();
//            }

//            spriteRenderer.color = normalColor;
//            Debug.Log($"NPCFailZone '{gameObject.name}' aktif oldu - Sprite: {spriteRenderer.sprite?.name}, Görünür olmalý!");
//        }

//        // Mesafe kontrolünü baþlat
//        if (distanceCheckCoroutine != null)
//        {
//            StopCoroutine(distanceCheckCoroutine);
//        }
//        distanceCheckCoroutine = StartCoroutine(CheckDistanceRoutine());
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Character"))
//        {
//            isCharacterInside = true;

//            // Fail timer'ý baþlat
//            if (failCoroutine != null)
//            {
//                StopCoroutine(failCoroutine);
//            }
//            failCoroutine = StartCoroutine(FailDelayCoroutine());
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Character"))
//        {
//            isCharacterInside = false;

//            // Timer'ý iptal et
//            if (failCoroutine != null)
//            {
//                StopCoroutine(failCoroutine);
//                failCoroutine = null;
//            }
//        }
//    }

//    private IEnumerator CheckDistanceRoutine()
//    {
//        while (true)
//        {
//            // Always animate açýksa veya karakter yoksa skip
//            if (alwaysAnimate)
//            {
//                if (!isAnimating)
//                {
//                    StartWarningAnimation();
//                }
//                yield return new WaitForSeconds(distanceCheckInterval);
//                continue;
//            }

//            // Karakter bulunamazsa animasyon yapma
//            if (characterTransform == null)
//            {
//                if (isAnimating)
//                {
//                    StopWarningAnimation();
//                }
//                yield return new WaitForSeconds(distanceCheckInterval);
//                continue;
//            }

//            // Mesafeyi hesapla
//            float distance = Vector2.Distance(transform.position, characterTransform.position);

//            // Karakter yakýnsa animasyon baþlat
//            if (distance <= animationDistance)
//            {
//                if (!isAnimating)
//                {
//                    StartWarningAnimation();
//                }
//            }
//            // Karakter uzaksa animasyon durdur
//            else
//            {
//                if (isAnimating)
//                {
//                    StopWarningAnimation();
//                }
//            }

//            // Bir süre bekle (optimizasyon)
//            yield return new WaitForSeconds(distanceCheckInterval);
//        }
//    }

//    private void StartWarningAnimation()
//    {
//        isAnimating = true;

//        if (warningAnimCoroutine != null)
//        {
//            StopCoroutine(warningAnimCoroutine);
//        }
//        warningAnimCoroutine = StartCoroutine(WarningAnimation());
//    }

//    private void StopWarningAnimation()
//    {
//        isAnimating = false;

//        if (warningAnimCoroutine != null)
//        {
//            StopCoroutine(warningAnimCoroutine);
//            warningAnimCoroutine = null;
//        }

//        // Normal renge dön
//        if (spriteRenderer != null)
//        {
//            spriteRenderer.color = normalColor;
//        }
//    }

//    private IEnumerator FailDelayCoroutine()
//    {
//        // 0.5 saniye bekle
//        yield return new WaitForSeconds(failDelay);

//        // Hala içerideyse fail
//        if (isCharacterInside)
//        {
//            TriggerFail();
//        }
//    }

//    private IEnumerator WarningAnimation()
//    {
//        // Animasyon aktif olduðu sürece renk yanýp sönsün
//        while (isAnimating)
//        {
//            // Normal'den warning'e geç
//            float elapsed = 0f;
//            float duration = 0.25f;

//            while (elapsed < duration && isAnimating)
//            {
//                elapsed += Time.deltaTime;
//                float t = elapsed / duration;
//                if (spriteRenderer != null)
//                {
//                    spriteRenderer.color = Color.Lerp(normalColor, warningColor, t);
//                }
//                yield return null;
//            }

//            // Warning'den normal'e geç
//            elapsed = 0f;
//            while (elapsed < duration && isAnimating)
//            {
//                elapsed += Time.deltaTime;
//                float t = elapsed / duration;
//                if (spriteRenderer != null)
//                {
//                    spriteRenderer.color = Color.Lerp(warningColor, normalColor, t);
//                }
//                yield return null;
//            }
//        }
//    }

//    private void TriggerFail()
//    {
//        Debug.Log("NPC Fail Zone triggered!");

//        // Fail panel'i göster
//        FailPanelManager.Instance?.ShowFailPanel();

//        // Coroutine'i temizle
//        failCoroutine = null;
//    }

//    private void OnDisable()
//    {
//        // Zone deaktif edilince tüm coroutine'leri temizle
//        if (failCoroutine != null)
//        {
//            StopCoroutine(failCoroutine);
//            failCoroutine = null;
//        }

//        if (distanceCheckCoroutine != null)
//        {
//            StopCoroutine(distanceCheckCoroutine);
//            distanceCheckCoroutine = null;
//        }

//        if (warningAnimCoroutine != null)
//        {
//            StopCoroutine(warningAnimCoroutine);
//            warningAnimCoroutine = null;
//        }

//        isCharacterInside = false;
//        isAnimating = false;
//    }
//}