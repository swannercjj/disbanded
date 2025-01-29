using UnityEngine;
using System.Collections.Generic;

public class TriggerFightBoss3 : MonoBehaviour
{
    public GameObject boss;
    private bool triggered = false;
    public List<Animator> doors; // List of animators for doors
    public BeatManager beat_manager;

    private void OnTriggerStay(Collider other)
    {
        if (triggered || this.enabled == false)
        {
            return;
        }

        // Check if the object passing through has a PlayerController script
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            triggered = true;

            // Set the "Close" trigger on all doors in the list
            foreach (Animator door in doors)
            {
                if (door != null)
                {
                    door.SetTrigger("Open");
                }
            }

            boss.GetComponent<ScriptManagerBoss3>().TriggerFight();
            beat_manager.PlayBossMusic();
        }
    }
}
