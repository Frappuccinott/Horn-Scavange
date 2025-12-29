using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Events;

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
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float cameraSpeed = 2f;
    [SerializeField] private float cameraHoldDuration = 1f;

    [Header("Zone Detection")]
    [SerializeField] private string playerTag = "Character";

    [SerializeField] private UnityEvent onDialogueComplete;

    private CharacterControls inputActions;
    private InputAction interactAction;
    private AudioSource audioSource;
    private Transform playerTransform;
    private Vector3 originalCameraPosition;
    private bool playerInZone;
    private bool isPlayingDialogue;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        inputActions = new CharacterControls();
        interactAction = inputActions.Character.Interact;

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
        interactionPrompt?.ShowPrompt(interactAction);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInZone = false;
        playerTransform = null;
        interactionPrompt?.HidePrompt();
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

        if (mainCamera != null)
            originalCameraPosition = mainCamera.position;

        interactionPrompt?.HidePrompt();
        SetPlayerMovement(false);

        foreach (DialogueData.DialogueLine line in dialogueData.dialogueLines)
        {
            dialogueUI?.ShowDialogue(line.characterName, line.dialogueText);

            if (line.voiceClip != null)
            {
                audioSource.clip = line.voiceClip;
                audioSource.Play();

                if (dialogueUI != null)
                    StartCoroutine(dialogueUI.AnimateDialogueWithAudio(line.voiceClip.length));

                yield return new WaitForSeconds(line.voiceClip.length);
            }
            else
            {
                yield return new WaitForSeconds(3f);
            }

            dialogueUI?.HideDialogue();
            yield return new WaitForSeconds(line.delayAfterLine);
        }

        if (cameraTarget != null)
            yield return StartCoroutine(MoveCameraToTarget());

        SetPlayerMovement(true);
        onDialogueComplete?.Invoke();
        isPlayingDialogue = false;

    }

    private IEnumerator MoveCameraToTarget()
    {
        if (mainCamera == null || cameraTarget == null) yield break;

        Vector3 targetPosition = cameraTarget.position;
        targetPosition.z = mainCamera.position.z;

        float duration = 1f / cameraSpeed;

        yield return StartCoroutine(MoveCamera(mainCamera.position, targetPosition, duration));
        yield return new WaitForSeconds(cameraHoldDuration);

        Vector3 returnPosition = originalCameraPosition;
        returnPosition.z = mainCamera.position.z;
        yield return StartCoroutine(MoveCamera(mainCamera.position, returnPosition, duration));
    }

    private IEnumerator MoveCamera(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mainCamera.position = Vector3.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        mainCamera.position = to;
    }

    private void SetPlayerMovement(bool enabled)
    {
        if (playerTransform == null) return;

        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null && !enabled)
            rb.linearVelocity = Vector2.zero;

        foreach (MonoBehaviour script in playerTransform.GetComponents<MonoBehaviour>())
        {
            string scriptName = script.GetType().Name.ToLower();

            if (scriptName.Contains("movement") ||
                scriptName.Contains("controller") ||
                scriptName.Contains("player"))
            {
                script.enabled = enabled;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D zoneCollider = GetComponentInChildren<BoxCollider2D>();
        if (zoneCollider != null && zoneCollider.isTrigger)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(zoneCollider.transform.position, zoneCollider.size);
        }

        if (cameraTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(cameraTarget.position, 0.5f);
            Gizmos.DrawLine(transform.position, cameraTarget.position);
        }
    }
}