using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectableHighlight : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = new Color(1f, 0f, 0f, 1f); // #FF0000
    [SerializeField] private Vector2 outlineDistance = new Vector2(8f, 8f);

    private Outline outline;
    private GameObject targetObject;

    private void Start()
    {
        // Hedef objeyi belirle
        targetObject = GetTargetObject();

        if (targetObject == null)
        {
            Debug.LogWarning($"[SelectableHighlight] {gameObject.name} üzerinde Image component bulunamadý!");
            return;
        }

        // Outline yoksa ekle
        outline = targetObject.GetComponent<Outline>();
        if (outline == null)
        {
            outline = targetObject.AddComponent<Outline>();
        }

        // Ayarlarý uygula
        outline.effectColor = outlineColor;
        outline.effectDistance = outlineDistance;
        outline.enabled = false;
    }

    private GameObject GetTargetObject()
    {
        // Önce bu objede Image var mý kontrol et (Button, Toggle için)
        if (GetComponent<Image>() != null)
        {
            return gameObject;
        }

        // Slider için Background'u ara
        Slider slider = GetComponent<Slider>();
        if (slider != null)
        {
            Transform background = transform.Find("Background");
            if (background != null && background.GetComponent<Image>() != null)
            {
                return background.gameObject;
            }
        }

        // Dropdown için Label'ý ara
        Dropdown dropdown = GetComponent<Dropdown>();
        if (dropdown != null)
        {
            Transform label = transform.Find("Label");
            if (label != null && label.GetComponent<Image>() != null)
            {
                return label.gameObject;
            }
        }

        // Hiçbiri deðilse ilk Image component'i olan child'ý bul
        Image childImage = GetComponentInChildren<Image>();
        if (childImage != null)
        {
            return childImage.gameObject;
        }

        return null;
    }

    private void Update()
    {
        if (outline != null)
        {
            outline.enabled = (EventSystem.current.currentSelectedGameObject == gameObject);
        }
    }
}