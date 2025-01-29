using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    private GameObject dialogueBox;      // Reference to the parent object (dialogue box)
    public TextMeshProUGUI nameText;     // Reference to the TextMeshProUGUI for the name
    public TextMeshProUGUI dialogueText; // Reference to the TextMeshProUGUI for the dialogue

    private Coroutine typingCoroutine;   // To keep track of the typing coroutine

    private void Awake()
    {
        dialogueBox = gameObject; // Set the dialogue box to this script's parent object
    }

    // Function to start the dialogue
    public void Say(string name, string dialogue)
    {
        // Enable the dialogue box
        dialogueBox.SetActive(true);

        // Set the name text immediately
        nameText.text = name;

        // Stop any ongoing typing coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Start a new typing coroutine
        typingCoroutine = StartCoroutine(TypeDialogue(dialogue));
    }

    // Coroutine to type out the dialogue letter by letter
    private IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = ""; // Clear existing text

        foreach (char letter in dialogue)
        {
            dialogueText.text += letter; // Add the next letter
            yield return new WaitForSeconds(0.02f); // Wait for a short time before adding the next letter
        }
    }

    // Function to hide the dialogue box
    public void Hide()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueBox.SetActive(false);
        dialogueText.text = "";
        nameText.text = "";
    }
}
