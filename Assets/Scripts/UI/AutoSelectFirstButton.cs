using UnityEngine;
using UnityEngine.EventSystems;

public class AutoSelectFirstButton : MonoBehaviour
{
    [Header("First Selected")]
    [SerializeField] private GameObject firstSelectedButton;

    [Header("Select on Disable (For sub-menus)")]
    [SerializeField] private bool selectPreviousMenuOnDisable = false;
    [SerializeField] private AutoSelectFirstButton previousMenu;

    private void OnEnable()
    {
        SelectFirst();
    }

    private void OnDisable()
    {
        // Alt menü kapanýrken ana menünün butonunu seç
        if (selectPreviousMenuOnDisable && previousMenu != null)
        {
            previousMenu.SelectFirst();
        }
    }

    public void SelectFirst()
    {
        if (firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }
}