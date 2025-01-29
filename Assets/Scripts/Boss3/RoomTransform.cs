using UnityEngine;

public class RoomTransform : MonoBehaviour
{
    public GameObject collapse; // Parent object containing child objects to modify
    public float pushForce = 10f; // Downward force magnitude

    // Start is called before the first frame update
    void Update()
    {
        Collapse();
        Destroy(this);
    }

    public void Collapse()
    {
        if (collapse != null)
        {
            foreach (Transform child in collapse.transform)
            {
                // Ensure the child has a Rigidbody
                Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = child.gameObject.AddComponent<Rigidbody>();
                }

                // Enable gravity
                rb.useGravity = true;

                // Apply a downward force
                rb.AddForce(Vector3.down * pushForce, ForceMode.Impulse);

                // Disable the collider if it exists
                // Collider collider = child.gameObject.GetComponent<Collider>();
                // if (collider != null)
                // {
                //     collider.enabled = false;
                // }

                // Destroy the object after 5 seconds
                // Destroy(child.gameObject, 5f);
            }
        }
        else
        {
            Debug.LogWarning("Collapse object is not assigned!");
        }
    }
}
