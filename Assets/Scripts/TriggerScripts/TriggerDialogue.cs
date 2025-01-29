using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class TriggerDialogue : MonoBehaviour
{
    public DialogueBox dialogueBox; // Reference to the DialogueBox script
    public List<string> dialogueTexts; // List of dialogue strings
    public string characterName; // The name of the character
    public GameObject beats;
    private int dialogueIndex = 0; // Tracks the current dialogue index
    private bool playerInRange = false; // Tracks if the player is in range
    private bool hasTriggered = false; // Tracks if the dialogue has already been triggered
    private GameObject player;


    private void OnTriggerStay(Collider other)
    {
        // Check if the object entering the trigger has the PlayerController tag and hasn't triggered yet
        if (other.CompareTag("Player") && !hasTriggered)
        {
            playerInRange = true;
            hasTriggered = true; // Mark as triggered
            player = other.gameObject;
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

    // private void OnTriggerExit(Collider other)
    // {
    //     // Check if the object exiting the trigger has the PlayerController tag
    //     if (other.CompareTag("Player"))
    //     {
    //         playerInRange = false;

    //         // Close the dialogue when the player leaves
    //         dialogueBox.Hide();
    //     }
    // }

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
                Destroy(this);
            }
        }
    }
}
