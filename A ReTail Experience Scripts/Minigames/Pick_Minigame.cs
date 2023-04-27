using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Pick_Minigame : MonoBehaviour
{
    public GameObject[] minigames; //an array of all the minigames to play
    public GameObject player; //the player
    int seed; //a random int that will be used as the seed
    public int gameChoice; //the game that will be chosen
    public GameObject nonPP_Canvas; //the canvas without post processing
    public GameObject scanlines; //the scanlines effect
    public Volume vol; //the post processing volume
    public AudioSource miniGame_finish_SFX, miniGame_start_SFX;


    private Bloom bloom;
    private LensDistortion ld;
    private DepthOfField dof;

    private void Start()
    {
        StartCoroutine("wait_until_player_chosen");

        seed = (int)System.DateTime.Now.Ticks; //generates a unique random seed so the game chosen each time will not be consistent
        Random.InitState(seed);

        vol.profile.TryGet<Bloom>(out bloom);
        vol.profile.TryGet<DepthOfField>(out dof);
        vol.profile.TryGet<LensDistortion>(out ld);
    }
    public void start_minigame(int _game_index) //picks a minigame at random
    {
        Debug.LogWarning("Game index from start minigame: " + _game_index);
        player.GetComponent<PlayerMovement>().enabled = false; //disables player movement while the game is playing
        player.GetComponent<Player_Helper>().enabled = false;

        restartScript(_game_index);
        nonPP_Canvas.SetActive(false); //deactivates the canvas with no post processing
        scanlines.SetActive(true); //activates the scanlines effect
        toggle_PostProcessing(true); //changes the postprocessing effects for minigame.
        minigames[_game_index].SetActive(true); //activates the minigame
        miniGame_start_SFX.Play();
    }

    public int pick_minigame()
    {
        return gameChoice = Random.Range(0, minigames.Length); //picks an int value within the length of the array
    }

    public void closeMiniGame(int game_index)
    {
        Pause.need_cursor_after_pause = false;
        Pause.in_maze = false;
        player.GetComponent<PlayerMovement>().enabled = true; //disables player movement while the game is playing
        player.GetComponent<Player_Helper>().enabled = true;
        player.GetComponent<Player_Helper>().finishedHelping(); 
        nonPP_Canvas.SetActive(true); // activate canvas UI with no post processing attached
        scanlines.SetActive(false); // deactivates the scanlines
        toggle_PostProcessing(false);
        minigames[game_index].SetActive(false); //deactivates the minigame
        miniGame_finish_SFX.Play();
    }

    public void restartScript(int game_index)
    {
        Debug.LogWarning("game_index from restart script: " + game_index);
        if(game_index == 0) //restarts the Mash.cs script
        {
            minigames[game_index].GetComponent<Mash>().enabled = false;
            minigames[game_index].GetComponent<Mash>().enabled = true;
        }

        else if(game_index == 1) //restarts the Word_Unscramble.cs script
        {
            minigames[game_index].GetComponent<Word_Unscramble>().enabled = false;
            minigames[game_index].GetComponent<Word_Unscramble>().enabled = true;
        }

        else if(game_index == 2) //restarts the Button_Sequence.cs script
        {
            minigames[game_index].GetComponent<Button_Sequence>().enabled = false;
            minigames[game_index].GetComponent<Button_Sequence>().enabled = true;
        }

        else if (game_index == 3) //restarts the Maze_Picker.cs script
        {
            Pause.need_cursor_after_pause = true;
            Pause.in_maze = true;

            minigames[game_index].GetComponent<Maze_Picker>().enabled = false;
            minigames[game_index].GetComponent<Maze_Picker>().enabled = true;
        }

        else if (game_index == 4) //restarts the Item_Finder.cs script
        {
            Pause.need_cursor_after_pause = true;

            minigames[game_index].GetComponent<Item_Finder>().enabled = false;
            minigames[game_index].GetComponent<Item_Finder>().enabled = true;
        }
    }

    private void toggle_PostProcessing(bool game)
    {
        if (game)
        {
            dof.focusDistance.value = 0.1f;
        }

        else
        {
            dof.focusDistance.value = 1.83f;
        }

        ld.active = game;
        bloom.active = !game;
    }

    IEnumerator wait_until_player_chosen()
    {
        yield return null;
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
