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
}