using UnityEngine;
using UnityEngine.SceneManagement;

public class Buton_Manager : MonoBehaviour
{
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotspot = new Vector2(16, 0);
    public int tut_page = 0;
    public int annoyed_manager = 0;
    public GameObject handbook;
    public GameObject phone; //to be used only in the main menu version of the game
    public GameObject ee_button; //the easter egg button. only visible on the main menu
    public GameObject[] tut_pages;
    public GameObject manager_speech; //only in the game levels. to be altered when the player opens the handbook or closes the entire thing.
    public Intro_Dialogue intro_dialogue; //only used in the game levels if the player has the intro dialogue enabled
    public GameObject[] levels; //the three different difficulty levels
    public int level_index; //the level index to parse through the levels array
    public Pause pause_manager; //to tell the game to unpause if the resume button is played. Only in the main levels in the game

    private void Start()
    {
        Cursor.SetCursor(null, hotspot, cursorMode);
        if (easter_egg_manager.easter_egg_button && SceneManager.GetActiveScene().name == "MainMenu")
        {
            ee_button.SetActive(true);

        }

        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            Intro_Dialogue.tut_on = true;

            if (ee_button.activeInHierarchy)
            {
                easter_egg_manager.easter_egg = true;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        level_index = 0;
    }
    public void button_Clicked(string name)
    {
        if(name == "Level_1")
        {
            SceneManager.LoadScene("Level_1");
        }

        else if(name == "Level_2")
        {
            SceneManager.LoadScene("Level_2");
        }

        else if(name == "Level_3")
        {
            SceneManager.LoadScene("Level_3");
        }

        else if(name == "Credits_Button")
        {
            SceneManager.LoadScene("Credits");
        }

        else if(name == "Quit_Button")
        {
            Application.Quit();
        }

        else if(name == "mainMenu_Button")
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        else if(name == "Handbook")
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                phone.SetActive(false);
            }
            else
            {
                manager_speech.SetActive(false);
            }
            handbook.SetActive(true);
        }

        else if(name == "nextPage")
        {
            if (tut_page == 0)
            {
                tut_pages[tut_page].SetActive(false);
                tut_pages[++tut_page].SetActive(true);
            }

            else if(tut_page == 1) //only available in the main game version of this menu
            {
                annoyed_manager++;

                if (annoyed_manager < 3)
                {
                    tut_pages[tut_page].SetActive(false);
                    tut_pages[tut_page + annoyed_manager].SetActive(true);
                }

                else //the player has been fired and unlocked the easter egg
                {
                    easter_egg_manager.easter_egg = true;
                    easter_egg_manager.easter_egg_button = true;
                    tut_pages[tut_page].SetActive(false);
                    tut_pages[tut_page + annoyed_manager].SetActive(true);
                }
            }
        }

        else if(name == "prevPage")
        {
            if (annoyed_manager == 0)
            {
                tut_pages[tut_page].SetActive(false);
                tut_pages[--tut_page].SetActive(true);
            }

            else
            {
                if (tut_pages[tut_page + annoyed_manager].activeInHierarchy) //we are in annoyed manager dialogue
                {
                    tut_pages[tut_page + annoyed_manager].SetActive(false);
                    tut_pages[tut_page].SetActive(true);
                }

                else
                {
                    tut_pages[tut_page].SetActive(false);
                    tut_pages[--tut_page].SetActive(true);
                }
            }
        }

        else if(name == "closeTut")
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                phone.SetActive(true);
            }

            else
            {
                manager_speech.SetActive(true);
            }

            tut_page = 0;
            tut_pages[0].SetActive(true);
            tut_pages[1].SetActive(false);
            handbook.SetActive(false);
                
        }

        else if(name == "easter_egg")
        {
            easter_egg_manager.easter_egg = !easter_egg_manager.easter_egg;
        }

        else if(name == "end_intro")
        {
            intro_dialogue.end_dialogue();
        }

        else if (name == "toggle_tut")
        {
            Intro_Dialogue.tut_on = !Intro_Dialogue.tut_on;
        }

        else if(name == "next_level")
        {
            if(level_index == levels.Length - 1)
            {
                levels[level_index].SetActive(false);
                level_index = 0;
                levels[level_index].SetActive(true);
            }

            else
            {
                levels[level_index].SetActive(false);
                levels[++level_index].SetActive(true);
            }
        }

        else if(name == "unpause")
        {
            pause_manager.resume_game();
        }
    }
}
