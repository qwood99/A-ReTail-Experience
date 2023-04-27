using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Button_Sequence : MonoBehaviour
{
    public Image[] button_sequence; //the image sequence shown to player
    public Sprite[] button_sprites; // the images that will be used
    public GameObject minigame_manager; 
    public List<int> keys_to_press; // the int values assigned to each key needed to be pressed.
    public int i = 0; // an indexer


    private Dictionary<KeyCode, int> dic = new Dictionary<KeyCode, int>() // dictionary to quickly grab the correct key value if given a keycode
    {
        {KeyCode.Alpha0, 0 }, {KeyCode.Keypad0, 0},
        {KeyCode.Alpha1, 1 }, {KeyCode.Keypad1, 1},
        {KeyCode.Alpha2, 2 }, {KeyCode.Keypad2, 2},
        {KeyCode.Alpha3, 3}, {KeyCode.Keypad3, 3},
        {KeyCode.Alpha4, 4 }, {KeyCode.Keypad4, 4},
        {KeyCode.Alpha5, 5 }, {KeyCode.Keypad5, 5},
        {KeyCode.Alpha6, 6 }, {KeyCode.Keypad6, 6},
        {KeyCode.Alpha7, 7 }, {KeyCode.Keypad7, 7},
        {KeyCode.Alpha8, 8 }, {KeyCode.Keypad8, 8},
        {KeyCode.Alpha9, 9 }, {KeyCode.Keypad9, 9},
        {KeyCode.UpArrow, 10},
        {KeyCode.RightArrow, 11},
        {KeyCode.DownArrow, 12},
        {KeyCode.LeftArrow, 13}
    };

    private void OnEnable()
    {
        chooseSequence();
    }

    private void Update()
    {
        if (!Pause.is_paused)
        {
            if (checkInput(i) && i == 5) // if the key pressed is correct and it is the final in the sequence
            {
                keys_to_press.Clear();
                minigame_manager.GetComponent<Pick_Minigame>().closeMiniGame(2);
            }

            else if (checkInput(i)) // the key pressed is correct
            {
                button_sequence[i].gameObject.GetComponent<Image>().color = Color.green;
                i++;
            }
        }
    }

    private void chooseSequence() // randomly generates the sequence
    {
        int rand;
        i = 0;

        for (int j = 0; j < 6; j++)
        {
            rand = UnityEngine.Random.Range(0, 14);
            button_sequence[j].sprite = button_sprites[rand];
            keys_to_press.Add(rand);
            button_sequence[j].GetComponent<Image>().color = Color.white;
        }
    }

    private bool checkInput(int i) // checks if the keycode inputted is the correct keycode for the sequence.
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode))) // iterates through all possible keycodes
        {
            if(Input.GetKeyDown(kcode) && dic.ContainsKey(kcode) && dic[kcode] == keys_to_press[i]) // if a key was pressed AND if the dictionary contains that keycode as a key AND it is the correct keycode to press in for the sequence
            {
                return true;
            }
        }
        return false;
    }

}
