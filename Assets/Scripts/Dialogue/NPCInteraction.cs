using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData dialogueData;

    [Header("UI References")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private InteractionPromptUI interactionPrompt;

    [Header("Camera Settings")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform cameraTarget; // Diyalog bitince kameranın gideceği yer
    [SerializeField] private float cameraSpeed = 2f;
    [SerializeField] private float cameraHoldDuration = 1f;

    private Vector3 originalCameraPosition; // Kameranın başlangıç pozisyonu (otomatik kaydedilecek)

    [Header("Input Settings")]
    [Tooltip("Character InputActions asset'inden otomatik çekilecek")]
    private CharacterControls inputActions;
    private InputAction interactAction;

    [Header("Zone Detection")]
    [SerializeField] private string playerTag = "Character";

    private AudioSource audioSource;
    private bool playerInZone = false;
    private bool isPlayingDialogue = false;
    private Transform playerTransform;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Input Actions setup
        inputActions = new CharacterControls();
        interactAction = inputActions.Character.Interact;

        // Ana kamerayı bul
        if (mainCamera == null)
            mainCamera = Camera.main.transform;
    }

    private void OnEnable()
    {
        interactAction.Enable();
        interactAction.performed += OnInteractPressed;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteractPressed;
        interactAction.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInZone = true;
        playerTransform = other.transform;

        // Etkileşim promptunu göster
        if (interactionPrompt != null)
            interactionPrompt.ShowPrompt(interactAction);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInZone = false;
        playerTransform = null;

        // Promptu gizle
        if (interactionPrompt != null)
            interactionPrompt.HidePrompt();
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (!playerInZone || isPlayingDialogue) return;
        if (dialogueData == null || dialogueData.dialogueLines.Length == 0) return;

        StartCoroutine(PlayDialogueSequence());
    }

    private IEnumerator PlayDialogueSequence()
    {
        isPlayingDialogue = true;

        // Kameranın başlangıç pozisyonunu kaydet
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.position;
        }

        // Promptu gizle
        if (interactionPrompt != null)
            interactionPrompt.HidePrompt();

        // Player input'unu devre dışı bırak
        DisablePlayerMovement();

        // Her dialogue line'ı oynat
        for (int i = 0; i < dialogueData.dialogueLines.Length; i++)
        {
            DialogueData.DialogueLine line = dialogueData.dialogueLines[i];

            // Dialogue UI'ı göster
            if (dialogueUI != null)
            {
                dialogueUI.ShowDialogue(line.characterName, line.dialogueText);
            }

            // Dublaj çal
            if (line.voiceClip != null)
            {
                audioSource.clip = line.voiceClip;
                audioSource.Play();

                // Yazıyı dublaj süresi boyunca soldan sağa doldur
                if (dialogueUI != null)
                {
                    StartCoroutine(dialogueUI.AnimateDialogueWithAudio(line.voiceClip.length));
                }

                // Dublaj bitene kadar bekle
                yield return new WaitForSeconds(line.voiceClip.length);
            }
            else
            {
                // Ses yoksa default bekleme süresi
                yield return new WaitForSeconds(3f);
            }

            // Alt yazıyı gizle
            if (dialogueUI != null)
                dialogueUI.HideDialogue();

            // Sonraki konuşmaya geçmeden önce bekle
            yield return new WaitForSeconds(line.delayAfterLine);
        }

        // Tüm diyaloglar bitti - Kamerayı hareket ettir
        if (cameraTarget != null)
        {
            yield return StartCoroutine(MoveCameraToTarget());
        }

        // Player input'unu tekrar aç
        EnablePlayerMovement();

        isPlayingDialogue = false;
    }

    private IEnumerator MoveCameraToTarget()
    {
        if (mainCamera == null || cameraTarget == null) yield break;

        Vector3 targetPosition = cameraTarget.position;
        targetPosition.z = mainCamera.position.z; // Z eksenini koru

        Vector3 startPosition = mainCamera.position;
        float elapsed = 0f;
        float duration = 1f / cameraSpeed; // Hareket süresi

        // Kamerayı hedefe doğru hareket ettir
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            mainCamera.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        mainCamera.position = targetPosition;

        // Hedef noktada bekle
        yield return new WaitForSeconds(cameraHoldDuration);

        // Kamerayı başlangıç pozisyonuna geri döndür
        elapsed = 0f;
        startPosition = mainCamera.position;
        Vector3 returnPosition = originalCameraPosition;
        returnPosition.z = mainCamera.position.z; // Z eksenini koru

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            mainCamera.position = Vector3.Lerp(startPosition, returnPosition, t);
            yield return null;
        }

        mainCamera.position = returnPosition;
    }

    private void DisablePlayerMovement()
    {
        if (playerTransform == null) return;

        // Rigidbody2D varsa hareketi durdur
        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Yaygın movement script isimlerini dene
        MonoBehaviour[] scripts = playerTransform.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            string scriptName = script.GetType().Name.ToLower();

            // Movement, Controller, Player gibi isimleri içeren script'leri disable et
            if (scriptName.Contains("movement") ||
                scriptName.Contains("controller") ||
                scriptName.Contains("player"))
            {
                script.enabled = false;
            }
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerTransform == null) return;

        // Yaygın movement script isimlerini dene
        MonoBehaviour[] scripts = playerTransform.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            string scriptName = script.GetType().Name.ToLower();

            // Movement, Controller, Player gibi isimleri içeren script'leri enable et
            if (scriptName.Contains("movement") ||
                scriptName.Contains("controller") ||
                scriptName.Contains("player"))
            {
                script.enabled = true;
            }
        }
    }

    // Debug için
    private void OnDrawGizmosSelected()
    {
        // Zone collider'ın alanını göster
        BoxCollider2D zoneCollider = GetComponentInChildren<BoxCollider2D>();
        if (zoneCollider != null && zoneCollider.isTrigger)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(zoneCollider.transform.position, zoneCollider.size);
        }

        // Kamera hedefini göster
        if (cameraTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(cameraTarget.position, 0.5f);
            Gizmos.DrawLine(transform.position, cameraTarget.position);
        }
    }
}