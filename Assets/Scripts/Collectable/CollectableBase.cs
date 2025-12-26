using UnityEngine;

public abstract class CollectableBase : MonoBehaviour, ICollectable
{
    [SerializeField] protected CollectableType type;
    [SerializeField] private Color glowColor = Color.yellow;
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float minAlpha = 0.5f;
    [SerializeField] private float maxAlpha = 1f;

    private SpriteRenderer glowRenderer;

    protected virtual void Start()
    {
        AddGlow();
    }

    private void AddGlow()
    {
        SpriteRenderer originalSprite = GetComponent<SpriteRenderer>();
        if (originalSprite == null) return;

        GameObject glowObj = new GameObject("Glow");
        glowObj.transform.SetParent(transform);
        glowObj.transform.localPosition = Vector3.zero;
        glowObj.transform.localRotation = Quaternion.identity;
        glowObj.transform.localScale = Vector3.one * 1.15f;

        glowRenderer = glowObj.AddComponent<SpriteRenderer>();
        glowRenderer.sprite = originalSprite.sprite;
        glowRenderer.color = glowColor;
        glowRenderer.sortingLayerName = originalSprite.sortingLayerName;
        glowRenderer.sortingOrder = originalSprite.sortingOrder - 1;
    }

    private void Update()
    {
        if (glowRenderer != null)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            Color color = glowRenderer.color;
            color.a = alpha;
            glowRenderer.color = color;
        }
    }

    public abstract void Interact();
    public CollectableType GetCollectableType() => type;
}