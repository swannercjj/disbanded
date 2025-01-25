using UnityEngine;

public class CrateDestroyer : MonoBehaviour
{
    public int id = 1;

    private void OnTriggerEnter(Collider other)
{
    // Check if the other object is on the "Crate" layer
    if (other.gameObject.layer == LayerMask.NameToLayer("Crate"))
    {   
        if (other.gameObject.GetComponent<Crate>()) {
            if (other.gameObject.GetComponent<Crate>().id == id) {
                return;
            }

        }

        Destroy(other.gameObject, 1f);
    }
}
}
