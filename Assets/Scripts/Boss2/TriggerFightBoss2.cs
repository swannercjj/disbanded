using UnityEngine;

public class TriggerFightBoss2 : MonoBehaviour
{
    public GameObject boss;
    private bool triggered = false;
    public Animator door; 
    public BeatManager manager;

    private void OnTriggerStay(Collider other)
    {
        if (triggered || this.enabled == false) {
            return;
        }

        // Check if the object passing through has a PlayerController script
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            triggered = true;
            door.SetTrigger("Close");
            manager.PlayBossMusic();
            boss.GetComponent<ScriptManagerBoss2>().TriggerFight();
        }
    }
}
