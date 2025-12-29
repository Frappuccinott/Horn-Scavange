using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static CharacterControls controls;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (controls != null) return;

        controls = new CharacterControls();
        controls.Character.Enable();
        controls.Minigame.Enable();
    }

    private void OnEnable()
    {
        if (controls != null)
        {
            controls.Character.Enable();
            controls.Minigame.Enable();
        }
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Character.Disable();
            controls.Minigame.Disable();
        }
    }
        public static class InputHelper
    {
        public static void EnableCharacter()
        {
            InputManager.controls.UI.Disable();
            InputManager.controls.Character.Enable();
        }

        public static void EnableUI()
        {
            InputManager.controls.Character.Disable();
            InputManager.controls.UI.Enable();
        }

        public static void DisableAll()
        {
            InputManager.controls.Character.Disable();
            InputManager.controls.UI.Disable();
        }
    }

}