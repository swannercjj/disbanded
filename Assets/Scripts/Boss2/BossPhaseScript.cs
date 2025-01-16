using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManagerBoss2 : MonoBehaviour
{
    public List<MonoBehaviour> scripts; // List of scripts to activate and deactivate
    public float gapBetweenPhases = 2f; // Time gap between each phase
    public float[] phaseDurations; // Duration for each phase
    public List<Transform> positionsAndRotations; // List of positions and rotations to lerp to

    private int currentScriptIndex = 0; // Index of the currently active script
    private float phaseTimer = 0f; // Timer to track phase duration
    private bool isLerping = false; // Flag to check if lerping is in progress
    private float lerpTimer = 0f; // Timer for lerping
    private Transform targetPosition; // The position and rotation the enemy is lerping to
    private bool hasStartedAttack = false; // Flag to check if the attack has already started
    private string state = "passive"; // Current state of the boss

    [SerializeField] public GameObject boss_health_bar; // Reference to the health bar slider
    public Health bossHealth; // Reference to the boss health script

    void Start()
    {
        // Ensure the scripts list and phaseDurations match
        if (scripts.Count == 0 || phaseDurations.Length != scripts.Count)
        {
            Debug.LogError("The scripts list or phaseDurations doesn't match correctly.");
            return;
        }

        // Ensure we have the health script assigned
        if (bossHealth == null)
        {
            Debug.LogError("Boss health script not found.");
            return;
        }

        // Initially deactivate all scripts
        DeactivateAllScripts();
    }

    public void TriggerFight()
    {
        boss_health_bar.SetActive(true);
        state = "attack";

        ActivateScript(currentScriptIndex);
        CallInitiate(scripts[currentScriptIndex]);
    }

    void Update()
    {
        if (state == "passive") return;

        // Check if health is 0 or below and disable the ScriptManager if true
        if (bossHealth != null && bossHealth.health <= 0)
        {
            Debug.Log("Boss health is zero, disabling all scripts.");
            boss_health_bar.SetActive(false);
            DeactivateAllScripts();
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
}
