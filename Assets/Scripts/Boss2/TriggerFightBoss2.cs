using UnityEngine;

public class TriggerFightBoss2 : MonoBehaviour
{
    public GameObject boss;
    private bool triggered = false;
    public Animator door; 

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) {
            return;
        }

        // Check if the object passing through has a PlayerController script
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            triggered = true;
            door.SetTrigger("Close");
            boss.GetComponent<ScriptManagerBoss2>().TriggerFight();
        }
    }
}
