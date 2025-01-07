using UnityEngine;

public class BossStateManager : MonoBehaviour
{
    public static BossStateManager Instance { get; private set; }

    public bool IsVulnerable { get; private set; } = false;
    private int totalPillars; // Total number of pillars
    public int destroyedPillars { get; private set; } = 0; // Number of destroyed pillars
    private float nextVulnerabilityThreshold; // The health threshold until the boss becomes invulnerable
    private BossHealth bossHealth; // Reference to the BossHealth script

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        bossHealth = GetComponent<BossHealth>();

        if (bossHealth == null)
        {
            Debug.LogError("BossStateManager requires a BossHealth component on the same GameObject.");
        }
    }

    private void Update()
    {
        CheckBossHealth();
    }

    public void InitializePillars(int totalPillars)
    {
        this.totalPillars = totalPillars;
        destroyedPillars = 0;
        nextVulnerabilityThreshold = bossHealth != null ? bossHealth.CurrentHealth : 0f;
        IsVulnerable = false;
    }

    public void NotifyPillarDestroyed()
    {
        destroyedPillars++;

        if (destroyedPillars > totalPillars)
        {
            Debug.LogWarning("More pillars destroyed than initialized. Check your logic.");
            return;
        }

        // Update vulnerability threshold
        float healthFraction = (float)(totalPillars - destroyedPillars) / totalPillars;
        nextVulnerabilityThreshold = bossHealth.MaxHealth * healthFraction;

        MakeBossVulnerable();
    }

    private void MakeBossVulnerable()
    {
        IsVulnerable = true;
    }

    public void CheckBossHealth()
    {
        if (bossHealth != null && bossHealth.CurrentHealth <= nextVulnerabilityThreshold)
        {
            IsVulnerable = false;
        }
    }
}
