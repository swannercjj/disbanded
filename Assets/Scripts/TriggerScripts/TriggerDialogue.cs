using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TriggerDialogue : MonoBehaviour
{
    public DialogueBox dialogueBox; // Reference to the DialogueBox script
    public List<string> dialogueTexts; // List of dialogue strings
    public string characterName; // The name of the character
    public GameObject beats;
    public bool trigger_once = false; // If true, the dialogue triggers only once

    private int dialogueIndex = 0; // Tracks the current dialogue index
    private bool playerInRange = false; // Tracks if the player is in range
    private bool hasTriggered = false; // Tracks if the dialogue has already been triggered
    private GameObject player;
    private string trigger_key; // Unique key for saving trigger state

    // Static HashSet to store triggers that have been activated in the current session
    private static HashSet<string> triggeredInSession = new HashSet<string>();

    private void Start()
    {
        // Generate a unique key for this trigger using its name and position
        trigger_key = gameObject.name + "_" + transform.position.ToString();

        // Check if the trigger has been activated in the current session
        if (trigger_once && triggeredInSession.Contains(trigger_key))
        {
            hasTriggered = true;
            gameObject.SetActive(false); // Disable the trigger for this session
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the object entering the trigger has the PlayerController tag and hasn't triggered yet
        if (other.CompareTag("Player") && !hasTriggered)
        {
            playerInRange = true;
            hasTriggered = true; // Mark as triggered
            player = other.gameObject;

            if (trigger_once)
            {
                triggeredInSession.Add(trigger_key); // Store it in the session-only HashSet
            }

            StartCoroutine(WaitAndEnable());
        }
    }

    private IEnumerator WaitAndEnable()
    {
        // Wait for 0.1 seconds
        yield return new WaitForSeconds(0.1f);

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Set velocity to zero
            rb.angularVelocity = Vector3.zero; // Stop rotational movement
        }

        player.GetComponent<PlayerController>().frozen = true;
        player.GetComponent<PlayerDash>().frozen = true;
        player.GetComponent<PlayerShoot>().frozen = true;
        beats.SetActive(false);

        // Start the dialogue
        if (dialogueTexts.Count > 0)
        {
            dialogueIndex = 0;
            dialogueBox.Say(characterName, dialogueTexts[dialogueIndex]);
        }
    }

    private void EnableAllParentScripts()
    {
        // Get the parent GameObject
        GameObject parent = gameObject;

        // Get all MonoBehaviour components on the parent
        MonoBehaviour[] scripts = parent.GetComponents<MonoBehaviour>();

        // Enable each script, except this one
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this) // Avoid enabling this script
            {
                script.enabled = true;
            }
        }
    }

    private void Update()
    {
        // Check if the player is in range and left-clicks
        if (playerInRange && Input.GetMouseButtonDown(0))
        {
            dialogueIndex++;

            if (dialogueIndex < dialogueTexts.Count)
            {
                // Show the next dialogue
                dialogueBox.Say(characterName, dialogueTexts[dialogueIndex]);
            }
            else
            {
                // End the dialogue
                dialogueBox.Hide();
                dialogueIndex = 0;
                player.GetComponent<PlayerController>().frozen = false;
                player.GetComponent<PlayerDash>().frozen = false;
                player.GetComponent<PlayerShoot>().frozen = false;
                beats.SetActive(true);
                EnableAllParentScripts();

                    Destroy(this); // Destroy the script after use if trigger_once is false

            }
        }
    }
}
