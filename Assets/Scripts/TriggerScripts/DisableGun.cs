using UnityEngine;

public class DisableGun : MonoBehaviour
{

    public GameObject gun;
    public PlayerShoot shoot;
    public PlayerDash dash;

    public bool freeze;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other) {

        if (!other.CompareTag("Player") || !this.enabled) {
            return;
        }

        gun.SetActive(!freeze);
        shoot.frozen = freeze;
        dash.frozen = freeze;
    }
}
