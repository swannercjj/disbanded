using UnityEngine;
using System.Collections;  // Required for coroutines

public class HideOnStart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DisableAfterDelay(0.1f)); // Start the coroutine with a 1-second delay
    }

    // Coroutine to delay disabling the GameObject
    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        gameObject.SetActive(false); // Disable the GameObject after the delay
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
