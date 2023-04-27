using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze_Picker : MonoBehaviour
{
    public GameObject[] layouts;
    public int maze_range; //to be used to limit which maze layouts can be chosen. used to alter difficulty
    private int maze_chosen;
    private int prev_chosen_maze = 100;

    private void OnEnable()
    {
        pick_maze();
    }

    public void pick_maze()
    {
        maze_chosen = Random.Range(0, maze_range);
        Debug.LogError("maze layout index = " + maze_chosen);

        if (maze_chosen != prev_chosen_maze)
        {
            prev_chosen_maze = maze_chosen;
            layouts[maze_chosen].SetActive(true);
            layouts[maze_chosen].GetComponent<Maze>().enabled = false;
            layouts[maze_chosen].GetComponent<Maze>().enabled = true;
        }

        else
        {
            pick_maze();
        }
    }
}
