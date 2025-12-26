using UnityEngine;

public class TrashItem : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    private bool isCleared = false;
    private TrashDragMinigameController controller;
    private RectTransform goldenHornRect;

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(TrashDragMinigameController ctrl, RectTransform goldenHorn)
    {
        controller = ctrl;
        goldenHornRect = goldenHorn;
    }

    public void ResetPosition()
    {
        isCleared = false;
        if (rectTransform != null)
        {
            gameObject.SetActive(true);
        }
    }

    public void StartDrag()
    {
        // Drag baþladýðýnda yapýlacak (opsiyonel efektler)
    }

    public void DragTo(Vector2 position)
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition = position;
    }

    public void EndDrag()
    {
        if (isCleared) return;

        bool overlaps = CheckOverlapWithGoldenHorn();

        if (!overlaps)
        {
            isCleared = true;

            if (controller != null)
                controller.OnTrashCleared();
        }
    }

    private bool CheckOverlapWithGoldenHorn()
    {
        if (goldenHornRect == null || rectTransform == null) return false;

        Rect trashRect = GetWorldRect(rectTransform);
        Rect goldenRect = GetWorldRect(goldenHornRect);

        return trashRect.Overlaps(goldenRect);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float yMin = corners[0].y;
        float xMax = corners[2].x;
        float yMax = corners[2].y;

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    public bool IsCleared()
    {
        return isCleared;
    }

    public Vector2 GetPosition()
    {
        return rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero;
    }
}