using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SCO_Manager : MonoBehaviour
{
    public GameObject help_box; //the box around the SCO machine where the player can interact
    public GameObject NPC; //the NPC that is on the SCO machine
    public GameObject help_Text; //the text that appears when an NPC needs help
    public GameObject help_Text_Box;
    public Pick_Minigame minigame_manager; //the minigame mananger
    public float time_unhelped;
    public int game_index;
    public bool is_NPC_at_SCO = false;


    public void activateBox(GameObject _NPC)
    {
        NPC = _NPC;
        is_NPC_at_SCO = true;
        game_index = minigame_manager.pick_minigame();
        set_helpText();
        help_box.SetActive(true);
        help_Text_Box.SetActive(true);
    }

    public void deactivateBox()
    {
        NPC = null;
        is_NPC_at_SCO = false;
        transform.Find("Interactable_Light").gameObject.SetActive(false);
        help_box.SetActive(false);
        help_Text_Box.GetComponent<Image>().color = Color.white;
        help_Text_Box.SetActive(false);
    }

    public void helping_NPC()
    {
        if (NPC.GetComponent<Upset_NPC>() != null)
        {
            NPC.GetComponent<Upset_NPC>().being_helped = true;
        }

        else
        {
            NPC.GetComponent<Karen>().being_helped = true;
        }

        minigame_manager.GetComponent<Pick_Minigame>().start_minigame(game_index);
    }

    public void done_Helping()
    {
        if (NPC.GetComponent<Upset_NPC>() != null)
        {
            NPC.GetComponent<Upset_NPC>().helped = true;
            NPC.GetComponent<Upset_NPC>().being_helped = false;
        }

        else
        {
            NPC.GetComponent<Karen>().helped = true;
            NPC.GetComponent<Karen>().being_helped = false;
        }
    }

    public void set_helpText()
    {
        if (game_index == 0)
        {
            help_Text.GetComponent<TMP_Text>().text = "MASH";
        }

        else if (game_index == 1)
        {
            help_Text.GetComponent<TMP_Text>().text = "Unscramble";
        }

        else if (game_index == 2)
        {
            help_Text.GetComponent<TMP_Text>().text = "Password";
        }

        else if (game_index == 3)
        {
            help_Text.GetComponent<TMP_Text>().text = "Maze";
        }

        else if(game_index == 4)
        {
            help_Text.GetComponent<TMP_Text>().text = "Item Check";

        }
    }

    private void Update()
    {
        if(is_NPC_at_SCO && NPC.GetComponent<Upset_NPC>() != null && !NPC.GetComponent<Upset_NPC>().being_helped && NPC.GetComponent<Upset_NPC>().needs_help)
        {
            time_unhelped = Time.timeSinceLevelLoad - NPC.GetComponent<Upset_NPC>().time_unhelped;

            if (time_unhelped >= NPC.GetComponent<Upset_NPC>().unhelped_timer * 0.6)
            {
                help_Text_Box.GetComponent<Image>().color = Color.red;
            }

            else if(time_unhelped >= NPC.GetComponent<Upset_NPC>().unhelped_timer * 0.3)
            {
                help_Text_Box.GetComponent<Image>().color = Color.yellow;
            }
        }

        else if(is_NPC_at_SCO && NPC.GetComponent<Karen>() != null && !NPC.GetComponent<Karen>().being_helped && NPC.GetComponent<Karen>().needs_help)
        {
            time_unhelped = Time.timeSinceLevelLoad - NPC.GetComponent<Karen>().time_unhelped;

            if (time_unhelped >= NPC.GetComponent<Karen>().unhelped_timer * 0.6)
            {
                help_Text_Box.GetComponent<Image>().color = Color.red;
            }

            else if (time_unhelped >= NPC.GetComponent<Karen>().unhelped_timer * 0.3)
            {
                help_Text_Box.GetComponent<Image>().color = Color.yellow;
            }
        }
    }

    public void pick_new_game()
    {
        game_index = minigame_manager.pick_minigame();
    }
}
