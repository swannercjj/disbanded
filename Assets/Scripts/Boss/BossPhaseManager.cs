using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    public List<MonoBehaviour> scripts; // List of scripts to activate and deactivate
    public float gapBetweenPhases = 2f; // Time gap between each phase
    public float[] phaseDurations; // Duration for each phase
    public List<Transform> positionsAndRotations; // List of positions and rotations to lerp to

    public List<Animator> pillars; // List of pillar animators
    public int phasesBetweenPillars = 3; // Number of phases between pillar activations
    private int phasesSinceLastPillar = 0; // Phases since the last pillar activation
    private int num_destroyed_pillars = -1;

    private int currentScriptIndex = 0; // Index of the currently active script
    private float phaseTimer = 0f; // Timer to track phase duration
    private bool isLerping = false; // Flag to check if lerping is in progress
    private float lerpTimer = 0f; // Timer for lerping
    private Transform targetPosition; // The position and rotation the enemy is lerping to
    private bool hasStartedAttack = false; // Flag to check if the attack has already started
    public int totalPillars; // The total number of pillars in the scene
    [SerializeField] public GameObject boss_health_bar; // Reference to the health bar slider
    public Health bossHealth; // Reference to the boss health script
    private string state = "passive";
    private int pillar_index = 0;

    void Start()
    {
        // Ensure the scripts list and phaseDurations match
        if (scripts.Count == 0 || phaseDurations.Length != scripts.Count)
        {
            Debug.LogError("The scripts list or phaseDurations doesn't match correctly.");
            return;
        }

        // Initialize the number of pillars for the BossStateManager
        BossStateManager.Instance.InitializePillars(totalPillars);
        ShufflePillars();
        // Ensure we have the health script assigned
        if (bossHealth == null)
        {
            Debug.LogError("Boss health script not found.");
            return;
        }

        // Initially deactivate all scripts
        DeactivateAllScripts();

        // Activate the first script and call Initiate()
    }

    public void TriggerFight() {
        boss_health_bar.SetActive(true);
        state = "attack";

        ActivateScript(currentScriptIndex);
        CallInitiate(scripts[currentScriptIndex]);
    }

    void Update()
    {
        if (state == "passive") {
            return;
        }

        // Check if health is 0 or below and disable the ScriptManager if true
        if (bossHealth != null && bossHealth.health <= 0)
        {
            Debug.Log("Boss health is zero, disabling all scripts.");
            boss_health_bar.SetActive(false);
            DeactivateAllScripts(); // Disable all scripts in the list
            this.enabled = false; // Disable the ScriptManager
            return; // Exit the update method
        }

        // Increment phase timer
        phaseTimer += Time.deltaTime;

        // Handle lerping to the new position and rotation if lerping is in progress
        if (isLerping)
        {
            lerpTimer += Time.deltaTime / gapBetweenPhases;

            transform.position = Vector3.Lerp(transform.position, targetPosition.position, lerpTimer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetPosition.rotation, lerpTimer);

            if (lerpTimer >= 1f)
            {
                isLerping = false;
                phaseTimer = 0f;
                hasStartedAttack = false;

                ActivateScript(currentScriptIndex);
                CallInitiate(scripts[currentScriptIndex]);
            }
        }
        else
        {
            if (phaseTimer >= phaseDurations[currentScriptIndex] && !hasStartedAttack)
            {
                StartAttack();
            }

            if (phaseTimer >= phaseDurations[currentScriptIndex])
            {
                DeactivateAllScripts();

                currentScriptIndex = (currentScriptIndex + 1) % scripts.Count;

                targetPosition = positionsAndRotations[Random.Range(0, positionsAndRotations.Count)];

                isLerping = true;
                lerpTimer = 0f;

                HandlePillarLogic();
            }
        }
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

    private void StartAttack()
    {
        if (scripts[currentScriptIndex] != null && !hasStartedAttack)
        {
            hasStartedAttack = true;
        }
    }
private void ShufflePillars()
{
    System.Random rng = new System.Random();
    int n = pillars.Count;
    while (n > 1)
    {
        n--;
        int k = rng.Next(n + 1);
        var value = pillars[k];
        pillars[k] = pillars[n];
        pillars[n] = value;
    }
}
    private void HandlePillarLogic()
    {
        if (phasesSinceLastPillar >= phasesBetweenPillars)
        {
            if (pillars[pillar_index] != null)
            {
                phasesSinceLastPillar = 0;
                num_destroyed_pillars = BossStateManager.Instance.destroyedPillars;
                // Play the opening animation
                pillars[pillar_index].SetTrigger("Open");
                pillar_index += 1;
                return;
            }
        }
        // Increment the phase counter only if the boss is invulnerable
        if (!BossStateManager.Instance.IsVulnerable && BossStateManager.Instance.destroyedPillars > num_destroyed_pillars)
        {
            phasesSinceLastPillar++;
        }
    }
}
