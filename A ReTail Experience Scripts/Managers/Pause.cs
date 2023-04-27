using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Pause : MonoBehaviour
{
    public static bool is_paused = false;
    public GameObject pause_screen;
    public static bool need_cursor_after_pause = false;
    public static bool in_maze = false;
    public Volume vol;
    private Bloom bloom;
    private DepthOfField dof;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private void Start()
    {
        vol.profile.TryGet<Bloom>(out bloom);
        vol.profile.TryGet<DepthOfField>(out dof);
    }

    public void pause_game()
    {
        dof.focusDistance.value = 0.1f;
        bloom.active = false;
        pause_screen.SetActive(true);
        if (in_maze)
        {
            Cursor.SetCursor(null, hotSpot, cursorMode);
        }
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        is_paused = true;
    }

    public void resume_game()
    {
        dof.focusDistance.value = 1.83f;
        bloom.active = true;
        pause_screen.SetActive(false);
        if (!need_cursor_after_pause)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (in_maze)
        {
            StartCoroutine("reset_mouse_for_maze");
        }
        Time.timeScale = 1f;
        is_paused = false;
    }

    IEnumerator reset_mouse_for_maze()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        Cursor.lockState = CursorLockMode.Locked;
        yield return null;
        Cursor.lockState = CursorLockMode.None;
    }
}
