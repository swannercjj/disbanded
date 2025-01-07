using UnityEngine;
using UnityEngine.UI;

public class BossHealth : EnemyHealth
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    [SerializeField] private Slider healthSlider; // Reference to the health bar slider
    [SerializeField] private float lerpSpeed = 5f; // Speed of the health bar lerp animation
    [SerializeField] private Image bossImage; // Reference to the UI Image for the boss
    [SerializeField] private Color invulnerableColor = Color.blue; // Color when invulnerable
    [SerializeField] private Color vulnerableColor = Color.black; // Color when vulnerable

    private float targetHealthValue = 1f; // The target value for the slider (normalized)
    private bool isDead = false; // Flag to prevent redundant death logic

    private void Start()
    {
        MaxHealth = health;
        CurrentHealth = health;

        if (healthSlider != null)
        {
            healthSlider.value = 1f; // Full health on start
        }

        UpdateBossImageColor(); // Set initial color based on vulnerability state
    }

    public override void TakeDamage(int damage)
    {
        if (BossStateManager.Instance.IsVulnerable && !isDead)
        {
            base.TakeDamage(damage); // Apply damage
            CurrentHealth = health;

            // Update the target slider value
            targetHealthValue = Mathf.Clamp01((float)health / MaxHealth); // Normalize health (0 to 1)


            if (CurrentHealth <= 0)
            {
                HandleDeath();
            }
        }
    }

    private void Update()
    {
        if (healthSlider != null)
        {
            // Smoothly lerp the slider value towards the target health value
            healthSlider.value = Mathf.Lerp(healthSlider.value, 1 - targetHealthValue, lerpSpeed * Time.deltaTime);
        }

        UpdateBossImageColor(); // Update the color when damage is taken
    }

    private void UpdateBossImageColor()
    {
        if (bossImage != null)
        {
            Color targetColor = BossStateManager.Instance.IsVulnerable ? vulnerableColor : invulnerableColor;
            bossImage.color = targetColor;
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        Debug.Log("Boss has been defeated!");

        // Add a Rigidbody and enable gravity
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }

        
    }
}
