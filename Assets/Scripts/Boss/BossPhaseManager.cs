using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour
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

    void Start()
    {
        // Ensure that the scripts list and positions list are not empty
        if (scripts.Count == 0 || phaseDurations.Length != scripts.Count)
        {
            Debug.LogError("The scripts list or phaseDurations doesn't match correctly.");
            return;
        }

        // Initially, deactivate all scripts
        DeactivateAllScripts();

        // Activate the first script and call Initiate()
        ActivateScript(currentScriptIndex);
        CallInitiate(scripts[currentScriptIndex]);
    }

    void Update()
    {
        // Increment phase timer
        phaseTimer += Time.deltaTime;

        // Handle lerping to the new position and rotation if lerping is in progress
        if (isLerping)
        {
            // We need to normalize the lerp timer to ensure it completes in the desired time (gapBetweenPhases seconds)
            lerpTimer += Time.deltaTime / gapBetweenPhases;

            // Lerp to the new position
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, lerpTimer);
            // Lerp to the new rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetPosition.rotation, lerpTimer);

            // If lerping is complete, stop the lerping and start the next phase
            if (lerpTimer >= 1f)
            {
                isLerping = false;
                phaseTimer = 0f; // Reset the phase timer to start the next phase
                hasStartedAttack = false; // Reset the attack flag after lerping

                // After lerping, reactivate the current script
                ActivateScript(currentScriptIndex);
                CallInitiate(scripts[currentScriptIndex]);
            }
        }
        else
        {
            // If no lerping, check if phase duration has passed
            if (phaseTimer >= phaseDurations[currentScriptIndex] && !hasStartedAttack)
            {
                // After lerping, start attack behavior (or any action tied to the script)
                StartAttack();
            }

            // If no lerping is in progress and phase duration has passed, handle the transition to the next phase
            if (phaseTimer >= phaseDurations[currentScriptIndex])
            {
                // Deactivate all scripts during the transition
                DeactivateAllScripts();

                // Move to the next script in the list (loop back to the first if necessary)
                currentScriptIndex = (currentScriptIndex + 1) % scripts.Count;

                // Pick a random position from the list of positions and rotations
                targetPosition = positionsAndRotations[Random.Range(0, positionsAndRotations.Count)];

                // Start lerping to the new position and rotation
                isLerping = true;
                lerpTimer = 0f; // Reset lerp timer
            }
        }
    }

    // Activate a script by its index
    private void ActivateScript(int index)
    {
        if (scripts[index] != null)
        {
            scripts[index].enabled = true;
        }
    }

    // Deactivate a script by its index
    private void DeactivateScript(int index)
    {
        if (scripts[index] != null)
        {
            scripts[index].enabled = false;
        }
    }

    // Deactivate all scripts in the list
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

    // Call the Initiate() method on the script if it exists
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

    // Start the attack or any other action after lerping
    private void StartAttack()
    {
        if (scripts[currentScriptIndex] != null && !hasStartedAttack)
        {
            // Trigger the attack or action tied to the script here.
            // For example:
            Debug.Log($"Attack initiated for script: {scripts[currentScriptIndex].name}");

            // Mark that the attack has been triggered for this phase
            hasStartedAttack = true;
        }
    }
}
