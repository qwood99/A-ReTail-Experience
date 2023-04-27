using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class Word_Unscramble : MonoBehaviour
{ 
    public TMP_InputField tmp_user_word; // used to take in user input of their guess toward the unscrambled word
    public TMP_Text tmp_scrambled_word; // displays the scrambled word in the canvas
    public GameObject minigame_manager;
    public AudioSource miniGame_error;

    int seed; // used to make sure that each time the game is loaded it will be randomly chosen 
    string word; // the chosen word to be scrambled
    string scrambled_word; // the scrambled word itself
    public int word_range; //to be used to limit the available words. to alter the dificulty

    public List<string> word_bank;
    public List<Sprite> images;

    public Image image_renderer;
    private Sprite image;
    private int prev_chosen_word = 100;
    private int chosen_word;

    private void OnEnable()
    {
        word_Picker();
        scrambler();
    }

    private void Update()
    {
        if (!tmp_user_word.isFocused && !Pause.is_paused) //this checks if the input field is focused, and if not, focuses it
        {
            StartCoroutine("focus_Input");
        }

        else if (Pause.is_paused && tmp_user_word.isFocused)
        {
            tmp_user_word.DeactivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.Return)) // when the user presses enter after guessing the word
        {
            Unscramble_The_Word();
        }
    }

    private void word_Picker() // picks a word from the word list to use
    {
        chosen_word = Random.Range(0, word_range); //choose a random word TO be scrambled

        if (chosen_word != prev_chosen_word)
        {
            word = word_bank[chosen_word];
            image = images[chosen_word];
            image_renderer.sprite = image;
        }

        else
        {
            word_Picker();
        }
    }

    private void scrambler() //scrambles the originally chosen word
    {
        StringBuilder scrambled_word_B = new StringBuilder(); //a way to gradually build the new string of the scrambled word
        List<int> slots_used = new List<int>(); //a list containing the values of all the slots already chosen for the word that is TO be scrambled;
        int letter_slot_chosen; //the letter that is chosen from the word TO be scrambled

        for(int i = 0; i < word.Length; i++)
        {
            if (i == 0)
            {
                letter_slot_chosen = Random.Range(1, word.Length); //ensures that the first letter chosen is never the first letter of the actual word to prevent the word from not being scrambled by chance
            }

            else
            {
                letter_slot_chosen = Random.Range(0, word.Length); //choose a random letter from the word TO be scrambled
            }

            if(slots_used == null) //if this is the first letter chosen
            {
                scrambled_word_B.Append(word[letter_slot_chosen]);
                slots_used.Add(letter_slot_chosen);
            }

            else //any other letter chosen iteration
            {
                if(has_slot_been_used(slots_used, letter_slot_chosen)) //checks if the slot has already been used to ensure that the scrambled word does NOT have any incorrectly repeated letters
                {
                    i--;
                }

                else
                {
                    scrambled_word_B.Append(word[letter_slot_chosen]);
                    slots_used.Add(letter_slot_chosen);
                }
            }
        }
        scrambled_word = scrambled_word_B.ToString();
        tmp_scrambled_word.text = scrambled_word;
    }

    private bool has_slot_been_used(List<int> slots_used, int letter_slot_chosen) //returns a bool determining if the slot has already been used to ensure that the scrambled word does NOT have any incorrectly repeated letters
    {
        for (int j = 0; j < slots_used.Count; j++)
        {
            if (slots_used[j] == letter_slot_chosen)
            {
                return true;
            }
        }
        return false;
    }

    private void Unscramble_The_Word() // checks if the guessed word is correct
    {
        if(tmp_user_word.text == word) // if the user is correct with the unscrambled word
        {
            tmp_user_word.text = "";
            minigame_manager.GetComponent<Pick_Minigame>().closeMiniGame(1);
        }

        else
        {
            tmp_user_word.text = "";
            miniGame_error.Play(); //play error noise
        }
    }

    IEnumerator focus_Input()
    {
        yield return null;
        tmp_user_word.ActivateInputField();
    }
}
