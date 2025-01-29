using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManagerBoss3 : MonoBehaviour
{
    [System.Serializable]
    public class PhaseData
    {
        public List<Transform> positions; // List of positions for this phase
        public List<float> durations;    // List of durations for moving to the corresponding positions
    }

    public List<MonoBehaviour> scripts; // List of scripts to activate and deactivate
    public List<PhaseData> phaseDataList; // List of phases with positions and durations
    public float gapBetweenPhases = 2f; // Time gap between phases
    public string state = "passive";    // Current state of the boss
    public GameObject boss_health_bar; // Reference to the health bar slider
    public Health bossHealth;          // Reference to the boss health script

    public GameObject prefab;          // Prefab to shoot
    public Transform prefab_spawnpoint;
    public int shootEveryPhases = 3;   // Shoot every n phases
    public float shootForce = 10f;     // Force applied to the prefab
    public int numberOfProjectiles = 5; // Number of projectiles to shoot

    private int currentScriptIndex = 0;  // Index of the currently active script
    private int phase_counter = 0;

    void Start()
    {
        // Ensure scripts and phase data are properly set
        if (scripts.Count == 0 || phaseDataList.Count != scripts.Count)
        {
            Debug.LogError("Mismatch between scripts and phase data.");
            return;
        }

        if (bossHealth == null)
        {
            Debug.LogError("Boss health script not assigned.");
            return;
        }

        DeactivateAllScripts();
    }

    public void TriggerFight()
    {
        boss_health_bar.SetActive(true);
        state = "attack";

        ActivateScript(currentScriptIndex);
        CallInitiate(scripts[currentScriptIndex]);
        StartCoroutine(StartPhase(currentScriptIndex));
    }

    void Update()
    {
        if (state == "passive") return;

        // Check if health is 0 or below and disable everything
        if (bossHealth != null && bossHealth.health <= 0)
        {
            Debug.Log("Boss defeated. Disabling all scripts.");
            boss_health_bar.SetActive(false);
            DeactivateAllScripts();
            this.enabled = false;
            return;
        }
    }

    private IEnumerator StartPhase(int phaseIndex)
    {
        PhaseData currentPhase = phaseDataList[phaseIndex];

        // Move through each position in the current phase
        for (int i = 0; i < currentPhase.positions.Count; i++)
        {
            Transform target = currentPhase.positions[i];
            float duration = currentPhase.durations[i];
            yield return MoveToPosition(target, duration);
        }

        // Check if it's time to shoot projectiles
        if (phase_counter >= shootEveryPhases && !BossStateManager3.Instance.IsVulnerable)
        {
            ShootProjectiles();
            phase_counter = 0;

        } else if (!BossStateManager3.Instance.IsVulnerable && BossStateManager3.Instance.weakpoints_left == 0) {
            phase_counter ++;
        }

        // Deactivate current script and move to the next phase
        DeactivateScript(currentScriptIndex);

        // Wait before starting the next phase
        yield return new WaitForSeconds(gapBetweenPhases);
        currentScriptIndex = (currentScriptIndex + 1) % scripts.Count;

        ActivateScript(currentScriptIndex);
        CallInitiate(scripts[currentScriptIndex]);

        StartCoroutine(StartPhase(currentScriptIndex));
    }

    private void ShootProjectiles()
    {
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            // Instantiate the prefab
            GameObject projectile = Instantiate(prefab, prefab_spawnpoint.position, Quaternion.identity);

            // Calculate a random direction (positive y with random x and z)
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0, 1f), Random.Range(-1f, 1f)).normalized;

            // Apply force to the projectile
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(randomDirection * shootForce, ForceMode.Impulse);
            }
        }
        BossStateManager3.Instance.weakpoints_left = numberOfProjectiles;
    }

    private IEnumerator MoveToPosition(Transform target, float duration)
    {

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        Vector3 targetPosition = target.position;
        Quaternion targetRotation = target.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Smoothly move and rotate toward the target
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        // Ensure final position and rotation match exactly
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    private void ActivateScript(int index)
    {
        if (scripts[index] != null)
        {
            scripts[index].enabled = true;
        }
    }

    private void DeactivateScript(int index)
    {
        if (scripts[index] != null)
        {
            scripts[index].enabled = false;
        }
    }

    private void DeactivateAllScripts()
    {
        foreach (var script in scripts)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }
    }

    private void CallInitiate(MonoBehaviour script)
    {
        if (script != null)
        {
            var method = script.GetType().GetMethod("Initiate", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(script, null);
            }
        }
    }
}
