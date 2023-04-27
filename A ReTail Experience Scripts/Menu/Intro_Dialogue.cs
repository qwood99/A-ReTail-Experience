using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_Dialogue : MonoBehaviour
{
    public GameManager gm;
    public GameObject tut_dialogue; //the gmaeobject with the intro dialogue UI
    public static bool tut_on = true; //the option in the main menu for the player to decide if they want to see this information


    private void Start()
    {
        if (tut_on)
        {
            Debug.LogError("tut_on");
            start_dialogue();
        }
    }
    public void start_dialogue()
    {
        Cursor.lockState = CursorLockMode.None;
        tut_dialogue.SetActive(true);
        gm.in_dialogue = true;
        Time.timeScale = 0f;
    }

    public void end_dialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        tut_dialogue.SetActive(false);
        gm.in_dialogue = false;
        Time.timeScale = 1f;
    }
}
