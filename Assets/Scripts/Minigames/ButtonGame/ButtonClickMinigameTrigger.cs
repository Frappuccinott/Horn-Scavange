using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ButtonClickMinigameTrigger : MonoBehaviour, ICollectable
{
    [SerializeField] private ButtonClickMinigameController minigameController;
    [SerializeField] private GameObject visualObject;

    private bool hasBeenTriggered = false;

    private void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    public void Interact()
    {
        if (!hasBeenTriggered)
        {
            TriggerMinigame();
        }
    }

    public CollectableType GetCollectableType()
    {
        return CollectableType.GoldenHorn;
    }

    private void TriggerMinigame()
    {
        hasBeenTriggered = true;

        if (minigameController != null)
            minigameController.StartMinigame();

        if (visualObject != null)
            visualObject.SetActive(false);

        gameObject.SetActive(false);
    }
}