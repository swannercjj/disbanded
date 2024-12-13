using UnityEngine;
using UnityEngine.UI; // For accessing UI elements like Image
using UnityEngine.SceneManagement; // Required for scene management
using System.Collections;

public class PlayerHealth : Health
{
    public GameObject gameOverScreen; // Reference to the game-over UI
    public Camera playerCamera; // Reference to the player's camera
    public float zoomOutHeight = 15f; // Base height for the camera to zoom out
    public float zoomOutSpeed = 2f; // Speed for the camera zoom-out
    public MonoBehaviour[] scriptsToDisable; // List of scripts to disable upon death
    public GameObject gunPrefab; // Reference to the gun prefab to hide on death
    public GameObject uiObjectToHide; // Reference to the UI object to hide on death
    public Image healthBarImage; // Reference to the health bar (Image UI element with fillAmount)

    private bool isDead = false; // Tracks if the player has already died
    private Transform cameraTransform;

    // Max HP for the player (this could also be set in the Inspector)
    public int maxHP = 100;

    // Track current health
    private float currentHealth;

    private void Start()
    {
        // Initialize the player's health
        currentHealth = maxHP;
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = 0f; // Initialize health bar at 0 (full health = 0 fill)
        }
    }

   public override void TakeDamage(int damage)
{
    if (isDead) return; // Prevent further actions if already dead

    base.TakeDamage(damage);

    currentHealth -= damage; // Decrease current health
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHP); // Ensure health doesn't go below 0

    // Update health bar (Image fill amount) over time using a coroutine
    if (healthBarImage != null)
    {
        // Calculate the target fillAmount based on the current health
        // Invert the scale so that max health = 0 fillAmount and 0 health = 0.5 fillAmount
        float targetFillAmount = 0.5f - ((float)currentHealth / maxHP) * 0.5f;
        StartCoroutine(SmoothHealthBarUpdate(targetFillAmount)); // Start the coroutine to update the health bar
    }

    if (currentHealth <= 0)
    {
        Die();
    }
}


    private IEnumerator SmoothHealthBarUpdate(float targetFillAmount)
    {
        float startFillAmount = healthBarImage.fillAmount; // Get the current fill amount
        float elapsedTime = 0f;
        float lerpDuration = 0.1f; // Duration for the smooth transition

        // Smoothly transition to the target fillAmount over the specified duration
        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime; // Increase elapsed time
            float currentFillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / lerpDuration); // Interpolate
            healthBarImage.fillAmount = currentFillAmount; // Update the health bar's fill amount
            yield return null; // Wait for the next frame
        }

        // Ensure the final value is exactly the target value
        healthBarImage.fillAmount = targetFillAmount;
    }

    private void Die()
    {
        isDead = true;

        // Show the game-over screen
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        // Disable player-related scripts
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        // Hide the gun and UI object
        if (gunPrefab != null)
        {
            gunPrefab.SetActive(false);
        }

        if (uiObjectToHide != null)
        {
            uiObjectToHide.SetActive(false);
        }

        // Detach the camera and zoom it out
        if (playerCamera != null)
        {
            cameraTransform = playerCamera.transform;

            // Detach the camera from the player
            cameraTransform.parent = null;

            // Start the zoom-out coroutine
            StartCoroutine(ZoomOutCamera(cameraTransform));
        }
    }

    private IEnumerator ZoomOutCamera(Transform cameraTransform)
    {
        // Randomize the direction of the camera zoom out
        float randomAngle = Random.Range(-45f, 45f); // Randomize the camera zoom-out angle

        // Calculate the target position with randomness in the upward direction
        Vector3 targetPosition = transform.position + Quaternion.Euler(randomAngle, 0f, 0f) * Vector3.up * zoomOutHeight;

        // Zoom out towards the random position while continuously looking at the player
        while (Vector3.Distance(cameraTransform.position, targetPosition) > 0.1f)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * zoomOutSpeed);

            // Keep looking at the player while zooming out
            cameraTransform.LookAt(transform.position);

            yield return null;
        }

        // After zoom-out is complete, the camera will remain still but always look at the player
        StayLookingAtPlayer(cameraTransform);
    }

    private void StayLookingAtPlayer(Transform cameraTransform)
    {
        // While the player is dead, keep the camera looking at the player
        StartCoroutine(LockCameraToPlayer(cameraTransform));
    }

    private IEnumerator LockCameraToPlayer(Transform cameraTransform)
    {
        // While the player is dead, keep the camera locked to the player and always looking at the player
        while (isDead)
        {
            cameraTransform.LookAt(transform.position);
            yield return null;
        }
    }

    void Update()
    {
        // If the player is dead and the "R" key is pressed, reload the scene
        if (isDead && Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
    }

    private void ReloadScene()
    {
        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
