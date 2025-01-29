using UnityEngine;

public class TransparencySetter : MonoBehaviour
{
    public GameObject prefabToSpawn;  // The prefab to spawn before destroying the object
    public Transform spawnPoint;      // The position at which to spawn the prefab
    public float fadeSpeed = 0.5f;    // The speed at which the transparency changes

    private Material materialInstance; // The unique material instance for this object
    private Color materialColor;      // To store the current color of the material
    private bool fadingIn = true;     // Tracks whether the object is becoming more visible or more transparent

    void Start()
    {
        // Get the Renderer component and assign a unique material instance
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            materialInstance = renderer.material; // This creates a unique material instance
            materialColor = materialInstance.color;
            materialColor.a = 0f; // Start fully transparent
            materialInstance.color = materialColor;
        }
        else
        {
            Debug.LogError("Renderer not found. Ensure the GameObject has a Renderer component.");
        }
    }

    void Update()
    {
        if (materialInstance != null)
        {
            // Adjust the alpha value
            if (fadingIn)
            {
                materialColor.a += fadeSpeed * Time.deltaTime; // Increase transparency
                if (materialColor.a >= 0.75f) // Stop when alpha reaches 0.75 (75% visible)
                {
                    materialColor.a = 0.75f;
                    fadingIn = false;
                }
            }
            else
            {
                materialColor.a -= fadeSpeed * Time.deltaTime; // Decrease transparency
                if (materialColor.a <= 0f) // Stop when alpha reaches 0 (fully transparent)
                {
                    materialColor.a = 0f;
                    if (prefabToSpawn != null && spawnPoint != null)
                    {
                        // Spawn the prefab at the specified spawn point
                        Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
                    }
                    Destroy(gameObject); // Destroy the GameObject after spawning the prefab
                }
            }

            // Apply the updated color to the material
            materialInstance.color = materialColor;
        }
    }
}
