using UnityEngine;
using System.Collections.Generic;

public class BossStateManager2 : MonoBehaviour
{
    public static BossStateManager2 Instance { get; private set; }

    public bool IsVulnerable { get; private set; } = false;
    public float nextVulnerabilityThreshold; // The health threshold until the boss becomes invulnerable (public)
    private BossHealth2 bossHealth; // Reference to the BossHealth script
    public List<ToggleCrystal> crystals = new List<ToggleCrystal>(); // List of crystals

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
        bossHealth = GetComponent<BossHealth2>();

        if (bossHealth == null)
        {
            Debug.LogError("BossStateManager requires a BossHealth component on the same GameObject.");
        }
    }

    private void Update()
    {
        CheckCrystalsAndHealth();
    }

    // Check if all crystals are dead, and handle boss vulnerability and health restoration
    public void CheckCrystalsAndHealth()
    {
        bool allCrystalsDead = true;

        // Check if all crystals are dead
        foreach (var crystal in crystals)
        {
            if (crystal.alive)
            {
                allCrystalsDead = false;
                break;
            }
        }

        // If all crystals are dead, the boss becomes vulnerable at the next threshold
        if (allCrystalsDead && !IsVulnerable)
        {
            SetVulnerabilityThreshold();
            IsVulnerable = true;
        }

        // If the boss is vulnerable and reaches the vulnerability threshold, restore health to crystals and make the boss invulnerable again
        if (IsVulnerable && bossHealth != null && bossHealth.CurrentHealth <= nextVulnerabilityThreshold)
        {
            MakeBossInvulnerable();
            RestoreCrystals();
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
    }

    private void MakeBossInvulnerable()
    {
        IsVulnerable = false;
    }

    private void RestoreCrystals()
    {
        // Restore health to each crystal and set them back to alive
        foreach (var crystal in crystals)
        {
            crystal.RestoreHealth();
        }
    }
}
