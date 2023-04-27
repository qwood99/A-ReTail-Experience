using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Helper : MonoBehaviour
{
    public GameObject minigame_manager; //the minigame picker
    public Transform schedule_desk;
    public Schedule_Manager sched_man; //the schedule manager
    private GameObject SCO; // the sco the player is interacting with
    [SerializeField] bool in_Help_Range = false; //the player is in range to help an NPC
    [SerializeField] bool in_sched_range = false; //the player is in range to view the schedule

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Help Box"))
        {
            Debug.Log(other.gameObject.tag);
            in_Help_Range = true; //The player is close enough to help
            SCO = other.gameObject.transform.parent.gameObject;
            SCO.transform.Find("Interactable_Light").gameObject.SetActive(true);
        }

        else if(other.gameObject.CompareTag("Schedule Box"))
        {
            schedule_desk.Find("desk_light").gameObject.SetActive(true);
            in_sched_range = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Help Box")) //The player left the range in which they can help NPC
        {
            in_Help_Range = false;
            SCO.transform.Find("Interactable_Light").gameObject.SetActive(false);
            SCO = null;
        }

        else if (other.gameObject.CompareTag("Schedule Box"))
        {
            schedule_desk.Find("desk_light").gameObject.SetActive(false);
            in_sched_range = false;
        }
    }

    private void toHelp()  //while the player is in the collider for the NPC to be helped
    {
        SCO.GetComponent<SCO_Manager>().helping_NPC();
    }



    public void finishedHelping() // The player has finished helping the NPC
    {
        SCO.GetComponent<SCO_Manager>().done_Helping();
        in_Help_Range = false;
    }

    public void check_schedule()
    {
        sched_man.view_schedule();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (in_Help_Range)
            {
                toHelp();
            }

            else if (in_sched_range)
            {
                check_schedule();
            }
        }
    }
}
