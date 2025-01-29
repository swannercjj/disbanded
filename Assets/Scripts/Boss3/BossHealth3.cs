using UnityEngine;
using UnityEngine.UI;

public class BossHealth3 : EnemyHealth
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
    public Animator door;

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

    public override void TakeDamage(int damage, bool cause)
    {
        if (BossStateManager3.Instance != null && BossStateManager3.Instance.IsVulnerable && !isDead)
        {
            base.TakeDamage(damage, cause); // Apply damage
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
        if (bossImage != null && BossStateManager3.Instance != null)
        {
            Color targetColor = BossStateManager3.Instance.IsVulnerable ? vulnerableColor : invulnerableColor;
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

        door.SetTrigger("PlayAnimation");
    }
}
