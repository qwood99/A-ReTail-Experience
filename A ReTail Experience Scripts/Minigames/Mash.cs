using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mash : MonoBehaviour
{
    public GameObject minigame_manager;
    public int amount_of_times_to_press_space; // amount of times player needs to press spacebar
    [SerializeField] private int hp;
    public AudioSource _as; // audio source that will play the sound effects
    public GameObject mash_text;
    public static bool display_mash_text = true;

    private void OnEnable()
    {
        this.transform.localPosition = new Vector3(0, 0, 0);
        hp = amount_of_times_to_press_space;

        if (mash_text.activeInHierarchy && !display_mash_text)
        {
            display_mash_text = true;
        }
    }

    void Update()
    {
        if (!Pause.is_paused)
        {
            if (hp <= 0) //when the minigame is completed
            {
                minigame_manager.GetComponent<Pick_Minigame>().closeMiniGame(0);
            }

            else if (Input.GetKeyDown(KeyCode.Space))
            { //when spacebar is pressed
                if (display_mash_text)
                {
                    display_mash_text = false;
                    mash_text.SetActive(false);
                }

                tickDown();
                this.transform.localPosition += new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-5, -2));
                _as.Play();
            }
        }
    }

    private void tickDown() // handles the game actions that occur when player presses the spacebar
    {
        hp--;
        _as.Stop();
        _as.Play();
    }
}
