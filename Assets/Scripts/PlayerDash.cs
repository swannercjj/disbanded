using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    public float dashSpeed = 20f; // Speed of the dash
    public float dashDuration = 0.2f; // Duration of the dash
    public float dashCooldown = 1f; // Cooldown time between dashes
    public float trailEffectDuration = 0.5f; // Duration the trail effect stays active after the dash
    public GameObject trailEffect; // Assign the trail effect prefab in the Inspector
    public RectTransform canvas; // Reference to the Canvas containing the BeatManager

    private BeatManager beatManager; // Reference to the BeatManager
    private Rigidbody rb;
    private Vector3 dashDirection;
    private float dashTimeRemaining;
    private bool isDashing;
    private float dashCooldownRemaining;
    private float trailEffectTimeRemaining;
    private VisualEffect vfx;
    private Coroutine fadeOutCoroutine; // To keep track of the fade-out coroutine
    public bool frozen;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("PlayerDash requires a Rigidbody component.");
        }

        if (trailEffect != null)
        {
            trailEffect.SetActive(false); // Ensure the trail effect is initially disabled
            vfx = trailEffect.GetComponent<VisualEffect>();
            if (vfx == null)
            {
                Debug.LogError("Trail effect is missing a VisualEffect component.");
            }
        }

        if (canvas != null)
        {
            // Find the BeatManager as a component of the canvas
            beatManager = canvas.GetComponentInChildren<BeatManager>();
            if (beatManager == null)
            {
                Debug.LogError("No BeatManager found on the Canvas.");
            }
        }
        else
        {
            Debug.LogError("Canvas reference is required for PlayerDash.");
        }
    }

    void Update()
    {
        HandleDashInput();
        UpdateDash();
        UpdateTrailEffect();
    }

    private void HandleDashInput()
    {
        // Update cooldown timer
        if (dashCooldownRemaining > 0)
        {
            dashCooldownRemaining -= Time.deltaTime;
        }

        // Dash input logic (LeftShift or right mouse button)
        if (!isDashing && dashCooldownRemaining <= 0 && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)))
        {
            if (beatManager != null)
            {
                GameObject beatInMiddle = beatManager.GetBeatInMiddle();

                if (beatInMiddle != null)
                {
                    // Start the dash
                    StartDash();
                }
                else
                {
                    // No beat in the middle, dash blocked
                    // Debug.Log("No beat in the middle. Dash blocked.");
                }
            }
        }
    }

    private void StartDash()
{
    if (frozen) {
        return;
    }

    Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

    if (inputDirection.magnitude > 0.1f)
    {
        dashDirection = transform.TransformDirection(inputDirection.normalized);
    }
    else
    {
        // Default to forward direction if no input is provided
        dashDirection = transform.forward;
    }

    dashTimeRemaining = dashDuration;
    isDashing = true;
    dashCooldownRemaining = dashCooldown;

    if (trailEffect != null)
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutCoroutine = null;
        }

        trailEffect.SetActive(true);
        vfx.SetFloat("Alpha", 1.0f);
        trailEffectTimeRemaining = trailEffectDuration;
    }
}


    private void UpdateDash()
    {
        if (isDashing)
        {
            if (dashTimeRemaining > 0)
            {
                rb.useGravity = false;
                rb.linearVelocity = new Vector3(dashDirection.x * dashSpeed, rb.linearVelocity.y, dashDirection.z * dashSpeed);
                dashTimeRemaining -= Time.deltaTime;
            }
            else
            {
                isDashing = false;
                rb.useGravity = true;
            }
        }
    }

    private void UpdateTrailEffect()
    {
        if (trailEffect != null && trailEffectTimeRemaining > 0)
        {
            trailEffectTimeRemaining -= Time.deltaTime;
            if (trailEffectTimeRemaining <= 0)
            {
                if (fadeOutCoroutine == null)
                {
                    fadeOutCoroutine = StartCoroutine(FadeOutTrailEffect());
                }
            }
        }
    }

    private IEnumerator FadeOutTrailEffect()
    {
        float fadeDuration = 1f;
        float startAlpha = vfx.GetFloat("Alpha");
        float fadeStep = Time.deltaTime / fadeDuration;

        for (float t = 0; t < 1; t += fadeStep)
        {
            float alpha = Mathf.Lerp(startAlpha, 0, t);
            vfx.SetFloat("Alpha", alpha);
            yield return null;
        }

        vfx.SetFloat("Alpha", 0);
        trailEffect.SetActive(false);
    }
}
