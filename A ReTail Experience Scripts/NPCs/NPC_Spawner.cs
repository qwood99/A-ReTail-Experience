using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC_Spawner : MonoBehaviour
{
    public Transform NPC_Parent;
    public GameObject normal_NPC; // the generic normal NPC
    public GameObject Karen_NPC;
    public GameObject[] upset_NPC;
    //Karen game object

    public float spawn_Timer = 5f; // determines the amount of time in between spawns
    public float last_spawned_time = 0f; // the time when the most recent NPC was spawned

    int upset_NPC_type;
    int NPC_behaviour;

    public const int NPC_LIMIT = 5; // limits the amount of NPCs to be in the scene at any one point

    private void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Normal_NPC") == null || GameObject.FindGameObjectsWithTag("Normal_NPC").Length < NPC_LIMIT)
        {
            if ((Time.timeSinceLevelLoad - last_spawned_time) >= spawn_Timer) // if it has been longer than the spawn timer since the time the last NPC was spawned
            {
                if (SceneManager.GetActiveScene().name == "Level_3")
                {
                    NPC_behaviour = Random.Range(0, 6); // randomly chooses if the NPC spawned will need help or not
                }

                else if(SceneManager.GetActiveScene().name == "Level_2")
                {
                    NPC_behaviour = Random.Range(0, 4); // randomly chooses if the NPC spawned will need help or not
                }

                else
                {
                    NPC_behaviour = Random.Range(0, 3); // randomly chooses if the NPC spawned will need help or not
                }

                if (NPC_behaviour == 0 || NPC_behaviour == 1) // NPC will need help
                {
                    Debug.Log("Upset NPC");
                    upset_NPC_type = Random.Range(0,3);
                    Instantiate(upset_NPC[upset_NPC_type], transform.position, transform.rotation, NPC_Parent);
                    last_spawned_time = Time.timeSinceLevelLoad;

                }

                else if(NPC_behaviour == 2) // NPC will NOT need help
                {
                    Debug.Log("Normal npc");
                    Instantiate(normal_NPC, transform.position, transform.rotation, NPC_Parent);
                    last_spawned_time = Time.timeSinceLevelLoad;

                }

                else
                {
                    Debug.Log("Normal npc");
                    Instantiate(Karen_NPC, transform.position, transform.rotation, NPC_Parent);
                    last_spawned_time = Time.timeSinceLevelLoad;
                }
            }
        }
    }
}
