using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Schedule_Manager : MonoBehaviour
{
    public Button[] button_times; //the buttons that the player can press to send people on break
    public TMP_Text[] button_text; //the text attached to the buttons
    public string[] time_text; //the text to be displayed on the button text
    public GameManager gm; //to allow the player to get write-ups and to access the shift time
    public GameObject sched; //the UI group with the schedule
    public float[] times; //the possible times that could be chosen. all in 30 minute increments to ensure none are TOO close together
    public List<float> chosen_times; //the chosen times from the text_times
    public GameObject player;
    public AudioSource paper_SFX;
    public bool allowed_to_deactivate_sched = false;
    public int time_range; //used to limit which times can be chosen so that shorter levels don't have a time that is out of bounds
    public Transform cam;


    private void Start()
    {
        StartCoroutine("wait_until_player_chosen");
        choose_breaks();
    }

    private void Update()
    {
        check_if_late();

        if(sched.activeInHierarchy && Input.GetKeyDown(KeyCode.E) && allowed_to_deactivate_sched)
        {
            allowed_to_deactivate_sched = false;
            deactivate_schedule();
        }
    }

    public void check_break(int button_num)
    {
        if (!Pause.is_paused)
        {
            if (chosen_times.Count == 1)
            {
                if (0 >= chosen_times[0] - gm.schedule_time)
                {
                    button_times[button_num].interactable = false;
                }
            }

            else if (0 >= chosen_times[button_num] - gm.schedule_time)
            {
                button_times[button_num].interactable = false;
            }
        }
    }

    public void check_if_late()
    {
        for(int i = 0; i < chosen_times.Count; i++)
        {
            if (chosen_times[i] - gm.schedule_time <= -0.5f && button_times[i].interactable)
            {
                button_times[i].interactable = false;               
                gm.get_writtenUp();
                
            }
        }
    }

    public void choose_breaks()
    {
        int rand;
        for (int i = 0; i < button_times.Length; i++)
        {
            if (i == 0)
            {
                rand = Random.Range(0, time_range);
                chosen_times.Add(times[rand]);
                button_text[i].text = time_text[rand];
            }

            else
            {
                rand = Random.Range(0, time_range);

                if (hasTimeBeenUsed(rand))
                {
                    --i;
                }

                else
                {
                    chosen_times.Add(times[rand]);
                    button_text[i].text = time_text[rand];
                }
            }
        }
    }

    public void view_schedule()
    {
        Pause.need_cursor_after_pause = true;

        cam.position = new Vector3(0.46f, -0.55f, 0.38f);
        cam.rotation = Quaternion.Euler(new Vector3(42.1f, 0, 0));
        Cursor.lockState = CursorLockMode.None;
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<Player_Helper>().enabled = false;
        paper_SFX.Play();
        sched.SetActive(true);
        StartCoroutine("opening_sched");
    }

    public void deactivate_schedule()
    {
        Pause.need_cursor_after_pause = false;

        cam.position = new Vector3(0.6f, 0.82f, -1.72f);
        cam.rotation = Quaternion.Euler(new Vector3(30f, 0, 0));
        Cursor.lockState = CursorLockMode.Locked;
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<Animator>().enabled = true;
        player.GetComponent<Player_Helper>().enabled = true;
        sched.SetActive(false);
    }

    public bool hasTimeBeenUsed(int rand)
    {
        for(int i = 0; i < chosen_times.Count; i++)
        {
            if(times[rand] == chosen_times[i])
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator opening_sched()
    {
        yield return new WaitForSeconds(.1f);
        allowed_to_deactivate_sched = true;
    }

    IEnumerator wait_until_player_chosen()
    {
        yield return null;
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
