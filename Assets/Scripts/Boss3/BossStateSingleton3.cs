using UnityEngine;
using System.Collections.Generic;

public class BossStateManager3 : MonoBehaviour
{
    public static BossStateManager3 Instance { get; private set; }

    public bool IsVulnerable { get; private set; } = false;
    private float nextVulnerabilityThreshold; // The health threshold until the boss becomes invulnerable (public)
    private BossHealth3 bossHealth; // Reference to the BossHealth script
    private int destroyed_crystal_count = 0;
    public int weakpoints_left = 0;

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
        bossHealth = GetComponent<BossHealth3>();

        if (bossHealth == null)
        {
            Debug.LogError("BossStateManager requires a BossHealth component on the same GameObject.");
        }
    }

    private void Update()
    {
        CheckCrystalsAndHealth();
    }

    public void IncrementCrystalsDestroyed() {
        destroyed_crystal_count += 1;
        weakpoints_left -= 1;
    }

    // Check if all crystals are dead, and handle boss vulnerability and health restoration
    public void CheckCrystalsAndHealth()
    {
        // If all crystals are dead, the boss becomes vulnerable at the next threshold
        if (destroyed_crystal_count >= 5 && !IsVulnerable)
        {
            SetVulnerabilityThreshold();
            IsVulnerable = true;
            destroyed_crystal_count = 0;
        }

        // If the boss is vulnerable and reaches the vulnerability threshold, restore health to crystals and make the boss invulnerable again
        if (IsVulnerable && bossHealth != null && bossHealth.CurrentHealth <= nextVulnerabilityThreshold)
        {
            MakeBossInvulnerable();
        }
    }

    // Set the boss vulnerability threshold based on health fractions (3/4, 2/4, 1/4)
    private void SetVulnerabilityThreshold()
    {
        if (bossHealth != null)
        {
            if (bossHealth.CurrentHealth > bossHealth.MaxHealth * 0.75f) 
            {
                nextVulnerabilityThreshold = bossHealth.MaxHealth * 0.75f;
            }
            else if (bossHealth.CurrentHealth > bossHealth.MaxHealth * 0.50f)
            {
                nextVulnerabilityThreshold = bossHealth.MaxHealth * 0.50f;
            }
            else if (bossHealth.CurrentHealth > bossHealth.MaxHealth * 0.25f)
            {
                nextVulnerabilityThreshold = bossHealth.MaxHealth * 0.25f;
            }
            else
            {
                nextVulnerabilityThreshold = bossHealth.MaxHealth * 0f; // Or set another behavior for the lowest health
            }
        }
        Debug.Log(nextVulnerabilityThreshold);
    }

    private void MakeBossInvulnerable()
    {
        IsVulnerable = false;
    }
}
