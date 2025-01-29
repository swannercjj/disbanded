using UnityEngine;

public class Launch : MonoBehaviour
{
    public float launchSpeed = 10f; // Speed at which the object moves in the negative Z direction
    private float lifetime = 6f;    // Time before the object is destroyed

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start a coroutine to destroy the object after a set time
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        // Move the object in the negative Z direction at a constant speed
        transform.Translate(Vector3.back * launchSpeed * Time.deltaTime);
    }
}
