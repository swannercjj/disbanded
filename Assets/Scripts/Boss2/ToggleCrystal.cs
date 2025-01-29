using UnityEngine;

public class ToggleCrystal : Health
{
    public bool alive = true; // The alive state of the crystal
    public Material aliveMaterial; // Material for the crystal when it's alive
    public Material deadMaterial;  // Material for the crystal when it's dead
    private Renderer crystalRenderer; // The Renderer component to change materials
    private int max_health;

    public ScriptManagerBoss2 boss_state;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Renderer component of the object
        crystalRenderer = GetComponent<Renderer>();

        if (crystalRenderer == null)
        {
            Debug.LogError("Renderer component not found!");
        }
        max_health = health;
        // Set the initial material based on the alive state
        UpdateMaterial();
    }

    // Override the TakeDamage method from the base class
    public override void TakeDamage(int damage, bool cause)
    {
        if (!cause || boss_state == null)
        {
            return;
        }

        if (boss_state.state == "passive") {
            return;
        }
        
        base.TakeDamage(damage, cause); // Call the base class implementation

        if (health <= 0)
        {
            alive = false;
            UpdateMaterial(); // Update material when the crystal dies
        }
    }

    // Restore health and set the object to be alive
    public void RestoreHealth()
    {
        health = max_health;
        alive = true;
        UpdateMaterial(); // Update material when the crystal is restored
    }

    // Method to update the material based on the alive state
    private void UpdateMaterial()
    {
        if (alive)
        {
            // Set the alive material
            crystalRenderer.material = aliveMaterial;
        }
        else
        {
            // Set the dead material
            crystalRenderer.material = deadMaterial;
        }
    }
}
