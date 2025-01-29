using UnityEngine;

public class WeakPointHealth: Health
{
    public GameObject explosionEffect; // Reference to the explosion effect prefab
    private Renderer objectRenderer;   // To control visibility

    private void Start()
    {
        // Cache the Renderer component
        objectRenderer = GetComponent<Renderer>();
        health = 200;
    }

    public override void TakeDamage(int damage, bool cause)
    {
        if (!cause) {
            return;
        }

        base.TakeDamage(damage, cause); // Call the base class implementation

        if (health <= 0)
        {
            TriggerExplosion();
            NotifyBossManager();
            MakeInvisible();
        }
    }

    private void TriggerExplosion()
    {
        if (explosionEffect != null)
        {
            // Instantiate the explosion effect at the object's position and rotation
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }
    }

    private void NotifyBossManager()
    {
        if (BossStateManager.Instance != null)
        {
            BossStateManager.Instance.NotifyPillarDestroyed();
        }
        else
        {
            Debug.LogWarning("BossStateManager instance is not found!");
        }
    }

    private void MakeInvisible()
    {
        if (objectRenderer != null)
        {
            objectRenderer.enabled = false; // Make the object invisible
            gameObject.SetActive(false);   // Disable the entire GameObject if necessary
        }
    }
}
