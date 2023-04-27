using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Maze : MonoBehaviour, IPointerEnterHandler
{
    public Pick_Minigame minigame_Manager;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotspot = new Vector2(16, 0);
    private AudioSource miniGame_Error;
    private bool maze_started = false;

    private void OnEnable()
    {
        maze_started = false;
        miniGame_Error = GameObject.FindGameObjectWithTag("error_sound").GetComponent<AudioSource>();
        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
        StartCoroutine("reset_Cursor");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Pause.is_paused && maze_started)
        {
            if (eventData.pointerCurrentRaycast.gameObject.CompareTag("maze_wall"))
            {
                miniGame_Error.Play();
                StartCoroutine("reset_Cursor");
            }

            else if (eventData.pointerCurrentRaycast.gameObject.CompareTag("maze_goal"))
            {
                Cursor.SetCursor(null, hotspot, cursorMode);
                Cursor.lockState = CursorLockMode.Locked;
                gameObject.SetActive(false);
                minigame_Manager.closeMiniGame(3);
            }
        }
    }


    IEnumerator reset_Cursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yield return null;
        Cursor.lockState = CursorLockMode.None;
        maze_started = true;
    }
}
