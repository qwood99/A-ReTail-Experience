using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item_Finder : MonoBehaviour
{
    public List<GameObject> layouts = new List<GameObject>(); //the layouts themselves
    public GameObject[] not_Goals; //the prefabs of the non-goal images (SAME ORDER AS THE Goals[])
    public GameObject[] goals; //the prefabs of the goal tagged images (SAME ORDER AS THE not_Goals[])

    public int goal_index = 0; //the index to be chosen for the goal image
    public AudioSource error_SFX; //to play when the player clicks on NOT the goal
    public int layout_index_range; //the layout index that can be adjusted in inspector depending on the difficulty of the scene
    public Pick_Minigame minigame_Manager;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotspot = new Vector2(16, 0);
    private List<int> chosen_points = new List<int>();
    public bool mouse_over_goal = false;
    public Transform goal_image; //the image to show the player what they need to find


    private int layout_index; //the actual value for the layout index

    private void Awake()
    {
        populate_dictionary(); //function to populate the dictionary with indexes and the layout gameobjects
    }

    void OnEnable()
    {
        mouse_over_goal = false;
        Cursor.lockState = CursorLockMode.None;
        choose_layout(); //function to choose the layout to be played
        choose_goal(); //function to randomly choose what obj the goal will be
        spawn_objects(); //function to spawn the objs in the spawn points
    }


    public void choose_goal()
    {
        goal_index = Random.Range(0, goals.Length);
        Instantiate(not_Goals[goal_index], goal_image.position, goal_image.rotation, goal_image);
    }

    public void spawn_objects()
    {

        int goal_spawn_point = Random.Range(0, layouts[layout_index].transform.childCount); //choose a random spawn point for the goal prefab

        int not_goal_index; //the index to choose the prefab for the not_goals
        int children_amount = layouts[layout_index].transform.childCount;

        for (int i = 0; i  < children_amount; i++)
        {

            if (i == 0)
            {
                Debug.LogError("spawning goal");

                Instantiate(goals[goal_index], layouts[layout_index].transform.GetChild(goal_spawn_point).transform.position, layouts[layout_index].transform.rotation, layouts[layout_index].transform);
            }

            else if(i == goal_spawn_point) //ignore the spawn point index that the goal was spawned at
            {

            }

            else
            {
                not_goal_index = Random.Range(0, not_Goals.Length);

                if(not_goal_index == goal_index) //redo the not_goal_index if it is the same as the goal index so that there are no duplicate prefab objects
                {
                    i--;
                }


                else
                {
                    Instantiate(not_Goals[not_goal_index], layouts[layout_index].transform.GetChild(i).transform.position, layouts[layout_index].transform.rotation, layouts[layout_index].transform);
                }
            }
        }
    }

    public void choose_layout()
    {
        layout_index = Random.Range(0, layout_index_range);
        layouts[layout_index].SetActive(true);
    }

    public void populate_dictionary()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            layouts.Add(transform.GetChild(i).gameObject);
            Debug.LogError(layouts[i].name);
        }
    }

    public bool slot_used(int not_goal_point)
    {
        for(int i = 0; i < chosen_points.Count; i++)
        {
            if(chosen_points[i] == not_goal_point)
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !Pause.is_paused)
        {
            if (mouse_over_goal)
            {
                Cursor.lockState = CursorLockMode.Locked;

                GameObject[] to_be_destroyed = GameObject.FindGameObjectsWithTag("not_goal");

                for (int i = 0; i < to_be_destroyed.Length; i++)
                {
                    Destroy(to_be_destroyed[i]);
                }

                Destroy(GameObject.FindGameObjectWithTag("item_goal"));

                layouts[layout_index].SetActive(false);
                minigame_Manager.closeMiniGame(4);
            }

            else
            {
                error_SFX.Play();
            }
        }
    }
}
