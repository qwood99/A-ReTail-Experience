using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class GameManager : MonoBehaviour
{
    public GameObject[] writeUps; //all the write up UI components
    public GameObject player, easter_egg_player, normal_player;
    public GameObject minigame_canvas, non_pp_canvas, schedule_canvas, help_text_canvas;
    public GameObject win_text, lose_text;
    public Transform digital_clock;
    public Transform write_up_parent;
    public AudioSource gameOver_Sound, victory_Sound, background_BGM;
    public Pause pause; //script to pause the game.
    public Intro_Dialogue intro_dialogue; //the script that controls all the intro dialogue 
    public Volume vol; //the post processing volume
    public Vector2 hotspot = new Vector2(16, 0);

    private Bloom bloom;
    private LensDistortion ld;
    private DepthOfField dof;


    public int counter_to_failure = 0; //the amount of write-ups the player has


    public string[] times; //to be used in the time_text
    public int time_index = 0; //the index for the times[]
    public float level_timer = 180f; //the total time the level will play
    public float schedule_time = 9f; //indicates what time it is in the shift. TO BE USED FOR THE SCHEDULE MANAGER
    public float new_half_hour = 11.25f; //the intervals between updating the time_text
    public TMP_Text time_text; //the time that will be displayed on the clock
    public bool did_player_win = false;
    public bool in_dialogue = false;

    private void Start()
    {

        vol.profile.TryGet<Bloom>(out bloom);
        vol.profile.TryGet<DepthOfField>(out dof);
        vol.profile.TryGet<LensDistortion>(out ld);

        ld.active = false;
        dof.focusDistance.value = 1.83f;
        bloom.active = true;

        Pause.need_cursor_after_pause = false;
        Pause.in_maze = false;
        Pause.is_paused = false;


        if (easter_egg_manager.easter_egg)
        {
            Debug.LogError("setting easter egg on");
            easter_egg_player.SetActive(true);
        }

        else
        {
            normal_player.SetActive(true);
        }


        StartCoroutine("choose_player_and_lock_cursor");
    }
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape) && !in_dialogue)
        {
            if (!Pause.is_paused)
            {
                pause.pause_game();
            }
        }
        

        if(level_timer - Time.timeSinceLevelLoad <= 0 && !did_player_win)
        {
            did_player_win = true;
            StartCoroutine("game_Over");
        }

        else if (new_half_hour - Time.timeSinceLevelLoad <= 0f)
        {
            schedule_time += .5f;
            new_half_hour += 11.25f;
            time_text.text = times[++time_index];
        }
    }

    public void get_writtenUp()
    {
        if(counter_to_failure == 2)
        {
            writeUps[counter_to_failure].SetActive(true);
            ++counter_to_failure;
            StartCoroutine("game_Over");
        }

        else
        {
            background_BGM.pitch += 0.25f;
            writeUps[counter_to_failure].SetActive(true);
            counter_to_failure++;
        }
    }

    IEnumerator game_Over()
    {
        dof.focusDistance.value = 1.83f;
        bloom.active = true;
        ld.active = false;

        minigame_canvas.SetActive(false);
        schedule_canvas.SetActive(false);
        help_text_canvas.SetActive(false);
        non_pp_canvas.SetActive(true);

        Cursor.SetCursor(null, hotspot, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.None;

        if (did_player_win)
        {
            victory_Sound.Play();
            digital_clock.localPosition = new Vector3(-834f, -446f, 0f);
            digital_clock.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            win_text.SetActive(true);
        }

        else
        {
            gameOver_Sound.Play();
            write_up_parent.localPosition = new Vector3(-443f, -446f, 0f);
            write_up_parent.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            lose_text.SetActive(true);
        }

        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<Player_Helper>().enabled = false;

        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 0f;
    }

    IEnumerator choose_player_and_lock_cursor()
    {
        yield return new WaitForSeconds(0.0001f);
        player = GameObject.FindGameObjectWithTag("Player");
        Cursor.lockState = CursorLockMode.Locked;
        digital_clock.localPosition = new Vector3(0f, 0f, 0f);
        digital_clock.localScale = new Vector3(1f, 1f, 1f);
        write_up_parent.localPosition = new Vector3(0f, 0f, 0f);
        write_up_parent.localScale = new Vector3(1f, 1f, 1f);
    }
}
