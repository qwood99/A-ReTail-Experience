using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


public class Upset_NPC : MonoBehaviour
{
    public GameObject player; // player gameobject
    private GameManager gm; // the gameManager
    [SerializeField] private Transform selfCheckout_location; // the location the NPC must move to for the SCO machine
    [SerializeField] private Transform exit_location; // the location the NPC must move to to exit the building
    private List<GameObject> all_SCOs = new List<GameObject>(); //all the SCO machines
    private List<GameObject> SCO_location = new List<GameObject>(); //all the walk to spots for the SCO machines
    private List<GameObject> SCO_look_spots = new List<GameObject>();


    public NavMeshAgent agent; // the nav mesh agent for pathfinding
    public Texture green_Light, red_Light; // to change the light above the self checkout when the NPC needs help.

    private List<GameObject> SCO_Lights = new List<GameObject>(); //The SCO Light
    private GameObject SCO_Light; //The SCO Light
    private SCO_List _SCO_List; 


    public float selfCheckout_timer = 3f; // the amount of time the NPC will stay at the SCO machine
    public float unhelped_timer = 30f; // the amount of time the NPC will stay at the SCO machine without being helped
    public float speed = 1f; // speed of NPC
    public float time_unhelped = 0f; // the length of time the NPC is unhelped
    public int spot_in_line = 0; // NPC place in line
    public int sco_index;


    int rand; // the random chance number for if the NPC needs help earlier than the full amount of time at self checkout

    public bool being_helped = false; // if the NPC is being helped by the player
    public bool helped = false; // if the player has finished helping the NPC
    public bool needs_help = false; // if the NPC needs help

    public Vector3 destination; // the target destination for the agent to move toward

    [SerializeField] private bool walking_to_selfCheckout = false; // if the NPC's destination is the self checkout machine
    [SerializeField] private bool walking_to_exit = false; // if the NPC's destination is the exit
    [SerializeField] private bool is_at_selfCheckout = false; // if the NPC is at the self checkout machine
    [SerializeField] private bool is_at_exit = false; // if the NPC is at the exit
    [SerializeField] private bool flash_light = false;
    [SerializeField] private float time_at_selfCheckout = 0f; // the length of time the NPC is at the self checkout machine
    [SerializeField] private float stuck_time = 20000f; //the amount of time the NPC has been in scene after they have been helped. This is to prevent them from stockpiling and never exiting.

    private void Start()
    {
        _SCO_List = GameObject.FindGameObjectWithTag("SCO_List").GetComponent<SCO_List>();

        for(int i = 0; i < _SCO_List.self_checkouts.Length; i++)
        {
            all_SCOs.Add(_SCO_List.self_checkouts[i]);
            SCO_Lights.Add(_SCO_List.self_checkouts[i]);
            SCO_location.Add(_SCO_List.self_checkouts[i].transform.Find("SCO walk location (1)").gameObject);
            SCO_look_spots.Add(_SCO_List.self_checkouts[i].transform.Find("look_spot").gameObject);
        }

        if(SceneManager.GetActiveScene().name == "Level_1")
        {
            unhelped_timer = 45f;
        }

        if (SceneManager.GetActiveScene().name == "Level_2")
        {
            unhelped_timer = 30f;
        }

        if (SceneManager.GetActiveScene().name == "Level_3")
        {
            unhelped_timer = 25f;
        }

        starting_spot(); // determine which place in line the NPC is in
        Debug.Log(spot_in_line);

        exit_location = GameObject.FindGameObjectWithTag("Exit_Location").transform;

        GetComponent<NavMeshAgent>().enabled = true;
        agent.updateRotation = false;

        gm = GameObject.FindGameObjectWithTag("game_manager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (walking_to_selfCheckout && !is_at_selfCheckout) // checks if NPC is walking to self checkout but is not there yet
        {
            gameObject.transform.LookAt(SCO_look_spots[sco_index].transform);
            is_at_selfCheckout = is_at_destination(destination);

        }

        else if (walking_to_exit && !is_at_exit) // checks if NPC is walking to the exit but is not there yet
        {
            is_at_exit = is_at_destination(destination);
        }

        else if (is_at_selfCheckout)
        {
            while (!needs_help) // the NPC does not need help yet
            {
                if (time_at_selfCheckout == 0) // starts the timer of NPC at self checkout
                {
                    agent.updateRotation = true;
                    SCO_Light.GetComponent<SCO_Manager>().activateBox(this.gameObject);
                    time_at_selfCheckout = Time.timeSinceLevelLoad;
                }

                else
                {
                    if (Time.timeSinceLevelLoad - time_at_selfCheckout >= selfCheckout_timer) // when NPC has been at the self checkout machine longer than the self checkout timer
                    {
                        needs_help = true;
                    }

                    else
                    {
                        rand = Random.Range(0, 1000); // random chance the NPC will need help earlier than the full time at self checkout timer

                        if (rand == 0)
                        {
                            needs_help = true;
                        }
                    }
                }
            }

            if (!flash_light)
            {
                StartCoroutine("flash_SCO_light_red");
            }

            if (time_unhelped == 0) // starts the unhelped timer for the NPC
            {
                time_unhelped = Time.timeSinceLevelLoad;
            }

            else
            {

                if (being_helped) // if the NPC is being helped, don't do anything
                {
                    StopCoroutine("flash_SCO_light_red");
                    StopCoroutine("flash_SCO_light_gray");
                }

                else if ((Time.timeSinceLevelLoad - time_unhelped >= unhelped_timer) || helped) // when the NPC has been unhelped too long or the player has completed helping them, the NPC will begin walking to the exit
                {
                    if (!helped)
                    {
                        gm.get_writtenUp();
                    }


                    SCO_Light.GetComponent<Renderer>().material.SetTexture("_BaseMap", green_Light); //change the SCO light to green
                    SCO_Light.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);



                    needs_help = false; //the NPC no longer needs help
                    SCO_Light.GetComponent<SCO_Manager>().deactivateBox(); //deactivate the help box trigger for the SCO


                    walking_to_exit = true; // NPC will start walking to exit
                    walking_to_selfCheckout = false;
                    is_at_selfCheckout = false; // NPC will no longer be at self checkout
                    destination = new Vector3(exit_location.position.x, 0.12f, exit_location.position.z); // NPC will now want to walk toward exit
                    agent.SetDestination(destination); //set the NPC to walk to the exit
                    SCO_Light.GetComponent<freeSCO>().free = true; //sets the SCO machine the NPC was at to be considered free for other NPCs to head to.
                    stuck_time = Time.timeSinceLevelLoad;
                    StartCoroutine("prevent_Stuck");
                }
            }
        }

        else if (is_at_exit && helped) // if the NPC reaches the exit and has been helped
        {
            Destroy(this.gameObject);
        }

        else if (is_at_exit && !helped) // if the NPC reaches the exit and has not been helped. Results in a penalty
        {
            // give player a write up
            Destroy(this.gameObject);
        }


        else if (spot_in_line == 0)
        {
            Debug.Log("next in line. Finding SCOs");
            findSCOs(); //find a free SCO since you are next in line
        }

        else
        {
            update_spot();
        }
    }

    private bool is_at_destination(Vector3 destination) // checks if the NPC is at the desired location
    {

        if (agent.remainingDistance == 0)
        {
            return true;
        }
        return false;
    }

    private void findSCOs()
    {
        for (int i = 0; i < all_SCOs.Count; i++)
        {
            if (all_SCOs[i].GetComponent<freeSCO>().is_free())
            {
                sco_index = i;
                all_SCOs[sco_index].GetComponent<freeSCO>().free = false;
                destination = new Vector3(SCO_location[sco_index].transform.position.x, 0.12f, SCO_location[sco_index].transform.position.z);
                agent.SetDestination(destination); // sets the NPC to move to the SCO machine
                agent.updateRotation = false;
                walking_to_selfCheckout = true;
                SCO_Light = SCO_Lights[sco_index];
                break;
            }
        }
    }

    private void starting_spot()
    {
        if (GameObject.FindGameObjectsWithTag("Normal_NPC").Length == 1)
        {
            spot_in_line = 0;
        }

        else
        {
            Debug.Log((GameObject.FindGameObjectsWithTag("Normal_NPC").Length));
            int taken_SCOs = 0;

            for (int i = 0; i < all_SCOs.Count; i++)
            {
                if (!all_SCOs[i].GetComponent<freeSCO>().is_free())
                {
                    taken_SCOs++;
                }
            }
            spot_in_line = GameObject.FindGameObjectsWithTag("Normal_NPC").Length - taken_SCOs;
        }
    }

    private void update_spot()
    {
        for (int i = 0; i < all_SCOs.Count; i++)
        {
            if (all_SCOs[i].GetComponent<freeSCO>().is_free())
            {
                if (spot_in_line == 0)
                {

                }
                else
                {
                    spot_in_line--;
                }
            }
        }
    }


    IEnumerator prevent_Stuck()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

    IEnumerator flash_SCO_light_red()
    {
        Debug.LogError("flashing red");
        flash_light = true;
        SCO_Light.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
        SCO_Light.GetComponent<Renderer>().material.SetTexture("_BaseMap", red_Light);

        yield return new WaitForSeconds(1f);
        StartCoroutine("flash_SCO_light_gray");
    }

    IEnumerator flash_SCO_light_gray()
    {
        Debug.LogError("flashing gray");
        SCO_Light.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.gray);

        yield return new WaitForSeconds(1f);
        StartCoroutine("flash_SCO_light_red");
    }
}
