using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick();
    }
}
