using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BeatManager : MonoBehaviour
{
    public GameObject beatPrefab;                // Prefab of the beat
    public RectTransform canvasRectTransform;    // Reference to the RectTransform of the canvas
    public Transform ui;
    public float bpm = 50f;                      // Beats per minute
    public float beatSpeed = 100f;               // Speed at which the beat moves (adjust for your game's speed)

    private float timeBetweenBeats;              // Time between spawns
    private float timeSinceLastSpawn;            // Timer for next spawn
    private List<GameObject> beats;              // List to keep track of spawned beats

    void Start()
    {
        // Initialize the beats list
        beats = new List<GameObject>();

        // Calculate spawn rate based on BPM
        timeBetweenBeats = 60f / bpm;  // 60 seconds / BPM gives time per beat
        timeSinceLastSpawn = timeBetweenBeats;  // Ensure the first beat spawns immediately
    }

    void Update()
    {
        // Update the timer to spawn new beats at the correct rate
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= timeBetweenBeats)
        {
            SpawnBeat();
            timeSinceLastSpawn = 0f;  // Reset the spawn timer
        }
    }

    void SpawnBeat()
    {
        float canvasWidth = canvasRectTransform.rect.width;
        Vector3 spawnPosition = new Vector3(canvasWidth / 2, -133, 0f);

        GameObject beat = Instantiate(beatPrefab, spawnPosition, Quaternion.identity);
        beat.transform.SetParent(ui);
        beat.transform.localPosition = spawnPosition;

        beats.Add(beat);

        BeatMovement beatMovement = beat.AddComponent<BeatMovement>();
        beatMovement.Initialize(beatSpeed, canvasWidth);
    }

    public GameObject GetBeatInMiddle(float centerRange = 18f)
    {
        float canvasWidth = canvasRectTransform.rect.width;
        float centerX = 0f;

        foreach (GameObject beat in beats)
        {
            if (beat != null)
            {
                RectTransform beatRect = beat.GetComponent<RectTransform>();
                float beatPositionX = beatRect.anchoredPosition.x;

                if (Mathf.Abs(beatPositionX - centerX) <= centerRange)
                {
                    BeatMovement beatMovement = beat.GetComponent<BeatMovement>();
                    if (beatMovement != null && !beatMovement.IsUsed)
                    {
                        beatMovement.IsUsed = true; // Mark this beat as used
                        ChangeBeatColor(beat, Color.cyan); // Set color to red
                        return beat; // Return the next beat
                    }
                }
            }
        }

        // If no beat in the middle, mark the next beat as used and set color to red
        foreach (GameObject beat in beats)
        {

            BeatMovement beatMovement = beat.GetComponent<BeatMovement>();
            RectTransform beatRect = beat.GetComponent<RectTransform>();
            float beatPositionX = beatRect.anchoredPosition.x;


            if (beatMovement != null && beatPositionX < 0f) {
                beatMovement.IsUsed = true;
            } 
            else if (beatMovement != null)
            {
                beatMovement.IsUsed = true; // Mark this beat as used
                ChangeBeatColor(beat, Color.red); // Set color to red
                return null; // Return the next beat
            }
        }

        return null; // No beat found in the middle or available for use
    }

    public bool IsBeatColorWhite(GameObject beat)
    {
        Color beatColor = GetBeatColor(beat);
        // Check if RGB values are all 1 and allow alpha to vary
        return Mathf.Approximately(beatColor.r, 1f) && Mathf.Approximately(beatColor.g, 1f) && Mathf.Approximately(beatColor.b, 1f);
    }

    public GameObject GetNextBeat()
    {
        foreach (GameObject beat in beats)
        {
            if (beat != null)
            {
                return beat; // Return the first beat in the list
            }
        }

        return null;
    }

    public void ChangeBeatColor(GameObject beat, Color color)
    {
        Image beatImage = beat.GetComponent<Image>();
        if (beatImage != null)
        {
            beatImage.color = color;
        }
    }

    public Color GetBeatColor(GameObject beat)
    {
        Image beatImage = beat.GetComponent<Image>();
        if (beatImage != null)
        {
            return beatImage.color; // Return the current color of the beat
        }
        else
        {
            return Color.clear; // Return a default value if no Image component is found
        }
    }


    public void RemoveBeat(GameObject beat)
    {
        if (beats.Contains(beat))
        {
            beats.Remove(beat);
        }
    }
}

public class BeatMovement : MonoBehaviour
{
    private float speed;
    private float canvasWidth;
    private RectTransform beatRectTransform;

    public bool IsUsed { get; set; } = false; // Tracks whether the beat has been used

    void Start()
    {
        beatRectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(float moveSpeed, float width)
    {
        speed = moveSpeed;
        canvasWidth = width;
    }

    void Update()
{
    beatRectTransform.anchoredPosition += Vector2.left * speed * Time.deltaTime;

    // Modify the fade-in logic: start fading in from 3/4 of the canvas width
    if (beatRectTransform.anchoredPosition.x > 0f)
    {
        // The beat starts fading in at 3/4 of the screen width
        float fadeInStart = canvasWidth * 0.35f; // 3/4 on the right
        float fadeInFactor = Mathf.InverseLerp(fadeInStart, 0f, beatRectTransform.anchoredPosition.x);
        SetAlpha(fadeInFactor);
    }
    else
    {
        // Modify the fade-out logic: start fading out 1/4 of the way from the left edge
        float fadeOutStart = -canvasWidth * 0.35f; // 1/4 on the left
        float fadeOutFactor = Mathf.InverseLerp(0f, fadeOutStart, beatRectTransform.anchoredPosition.x);
        SetAlpha(1 - fadeOutFactor); // Fade out as it moves past the left side
    }

    // Remove and destroy beat if it goes off-screen
    if (beatRectTransform.anchoredPosition.x < -canvasWidth)
    {
        BeatManager beatManager = Object.FindFirstObjectByType<BeatManager>();
        if (beatManager != null)
        {
            beatManager.RemoveBeat(gameObject);
        }
        Destroy(gameObject);
    }
}


    private void SetAlpha(float alpha)
    {
        Image beatImage = beatRectTransform.GetComponent<Image>();
        if (beatImage != null)
        {
            Color currentColor = beatImage.color;
            beatImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }
    }
}
