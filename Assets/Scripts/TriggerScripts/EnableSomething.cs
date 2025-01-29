using UnityEngine;
using System.Collections;
public class EnableSomething : MonoBehaviour
{
    private bool hasTriggered = false; // Tracks if the dialogue has already been triggered

    public GameObject something;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(WaitAndEnable());
        }
    }

    private IEnumerator WaitAndEnable()
    {
        // Wait for 0.1 seconds
        yield return new WaitForSeconds(0.1f);

        // Enable the GameObject
        something.SetActive(true);
    }
}
