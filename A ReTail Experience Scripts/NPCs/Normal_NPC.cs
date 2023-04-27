using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Normal_NPC : MonoBehaviour
{
    public GameObject player; // player gameobject
    public GameObject SCO;
    private List<GameObject> all_SCOs = new List<GameObject>(); //all the SCO machines
    private SCO_List _SCO_List;
    private List<GameObject> SCO_location = new List<GameObject>(); //all the walk to spots for the SCO machines
    private List<GameObject> SCO_look_spots = new List<GameObject>();
    public int sco_index;

    public Transform selfCheckout_location; // the location the NPC must move to for the SCO machine
    [SerializeField] private Transform exit_location; // the location the NPC must move to to exit the building
    public NavMeshAgent agent; // the nav mesh agent for pathfinding

    public float selfCheckout_timer = 3f; // the amount of time the NPC will stay at the SCO machine
    public float speed = 1f; // speed of NPC
    public int spot_in_line = 0; // NPC place in line
    public Vector3 destination; // the target destination for the agent to move toward
    [SerializeField] private float stuck_time = 20000f; //the amount of time the NPC has been in scene after they have been helped. This is to prevent them from stockpiling and never exiting.

    [SerializeField] private bool walking_to_selfCheckout = false; // if the NPC's destination is the self checkout machine
    [SerializeField] private bool walking_to_exit = false; // if the NPC's destination is the exit
    [SerializeField] private bool is_at_selfCheckout = false; // if the NPC is at the self checkout machine
    [SerializeField] private bool is_at_exit = false; // if the NPC is at the exit
    [SerializeField] private float time_at_selfCheckout = 0f; // the length of time the NPC is at the self checkout machine

    private void Start()
    {
        _SCO_List = GameObject.FindGameObjectWithTag("SCO_List").GetComponent<SCO_List>();

        for (int i = 0; i < _SCO_List.self_checkouts.Length; i++)
        {
            all_SCOs.Add(_SCO_List.self_checkouts[i]);
            SCO_location.Add(_SCO_List.self_checkouts[i].transform.Find("SCO walk location (1)").gameObject);
            SCO_look_spots.Add(_SCO_List.self_checkouts[i].transform.Find("look_spot").gameObject);
        }

        starting_spot(); // determine which place in line the NPC is in
        Debug.Log(spot_in_line);

        exit_location = GameObject.FindGameObjectWithTag("Exit_Location").transform; // determines the exit location

        GetComponent<NavMeshAgent>().enabled = true;
    }

    private void Update()
    {

        if (walking_to_selfCheckout && !is_at_selfCheckout)
        {
            gameObject.transform.LookAt(SCO_look_spots[sco_index].transform);
            is_at_selfCheckout = is_at_destination(destination);
        }

        else if (walking_to_exit && !is_at_exit)
        {
            is_at_exit = is_at_destination(destination);
        }

        else if (is_at_selfCheckout)
        {
            if (time_at_selfCheckout == 0) // starts the timer of NPC at self checkout
            {
                time_at_selfCheckout = Time.timeSinceLevelLoad;
            }
            else
            {
                if(Time.timeSinceLevelLoad - time_at_selfCheckout >= selfCheckout_timer) // when NPC has been at the self checkout machine longer than the self checkout timer
                {
                    walking_to_exit = true; // NPC will start walking to exit
                    agent.updateRotation = true;
                    walking_to_selfCheckout = false;
                    is_at_selfCheckout = false; // NPC will no longer be at self checkout
                    destination = new Vector3(exit_location.position.x, 0.12f, exit_location.position.z); // NPC will now want to walk toward exit
                    agent.SetDestination(destination);
                    SCO.GetComponent<freeSCO>().free = true;
                    stuck_time = Time.timeSinceLevelLoad;
                    StartCoroutine("prevent_Stuck");
                }
            }
        }

        else if (is_at_exit)
        {
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
        //Debug.Log("Destination: " + destination + " transform.position: " + transform.position);
        if (agent.remainingDistance == 0)
        {
           // Debug.Log("true");
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
                SCO = all_SCOs[sco_index];
                SCO.GetComponent<freeSCO>().free = false;
                destination = new Vector3(SCO_location[sco_index].transform.position.x, 0.12f, SCO_location[sco_index].transform.position.z);
                agent.SetDestination(destination); // sets the NPC to move to the SCO machine
                agent.updateRotation = false;
                walking_to_selfCheckout = true;
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
        for(int i = 0; i < all_SCOs.Count; i++)
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
}
