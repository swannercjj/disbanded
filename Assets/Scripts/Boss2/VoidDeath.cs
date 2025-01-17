using UnityEngine;

public class VoidDeath : MonoBehaviour
{
    public float growthRate = 1f;      // Amount to grow per second
    public float maxSize = 5f;        // Maximum size before fading out
    public float fadeSpeed = 1f;      // Speed of fading out

    private Vector3 targetScale;      // The next target scale
    private Renderer objectRenderer;  // Renderer for fading out
    private Color initialColor;       // Original color of the object
    private bool fadingOut = false;   // Whether the object is fading out

    void Start()
    {
        // Initialize the target scale and the renderer
        targetScale = transform.localScale;
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            initialColor = objectRenderer.material.color;
        }
        else
        {
            Debug.LogError("No Renderer attached to the object.");
        }
    }

    void Update()
    {
        if (!fadingOut)
        {
            // Grow the object if it's not fading out
            targetScale += Vector3.one * (growthRate * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime);

            // Check if the object has reached the maximum size
            if (transform.localScale.x >= maxSize)
            {
                fadingOut = true;
            }
        }
        else
        {
            // Fade out the object
            if (objectRenderer != null)
            {
                Color currentColor = objectRenderer.material.color;
                float newAlpha = Mathf.Max(currentColor.a - (fadeSpeed * Time.deltaTime), 0f);
                objectRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

                // Destroy the object when fully faded
                if (newAlpha <= 0f)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
        }
    }
}
