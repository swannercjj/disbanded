using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BeatManager : MonoBehaviour
{
    public GameObject beatPrefab;                // Prefab of the beat
    public RectTransform canvasRectTransform;    // Reference to the RectTransform of the canvas
    public Transform ui;                         // Parent for spawned beats
    public float bpm = 50f;                      // Beats per minute
    public float beatSpeed = 100f;               // Speed at which the beat moves (adjust for your game's speed)
    public AudioSource beat_sound;

    public AudioSource musicSource;             // AudioSource for playing the music

    private float timeBetweenBeats;              // Time between spawns
    private float timeSinceLastSpawn;            // Timer for next spawn
    private List<GameObject> beats;              // List to keep track of spawned beats
    private bool isMusicPlaying = false;         // Tracks whether music is currently playing

    void Start()
    {
        // Initialize the beats list
        beats = new List<GameObject>();

        // Calculate spawn rate based on BPM
        timeBetweenBeats = 60f / bpm;  // 60 seconds / BPM gives time per beat
        timeSinceLastSpawn = timeBetweenBeats;  // Ensure the first beat spawns immediately

        // Set up the music source
        if (musicSource != null)
        {
            musicSource.loop = true; // Loop the music
        }
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
    beat.transform.SetParent(ui, false); // Set to false to prevent world position adjustments

    // Reset local scale to avoid unintended size changes
    beat.transform.localScale = Vector3.one / 3f;

    beats.Add(beat);

    BeatMovement beatMovement = beat.AddComponent<BeatMovement>();
    beatMovement.Initialize(beatSpeed, canvasWidth, this, beat_sound, Time.time);
}


    public void StartMusic()
    {
        if (!isMusicPlaying && musicSource != null)
        {
            musicSource.Play();
            isMusicPlaying = true;
        }
    }

    public bool IsMusicPlaying()
    {
        return isMusicPlaying;
    }

    public void RemoveBeat(GameObject beat)
    {
        if (beats.Contains(beat))
        {
            beats.Remove(beat);
        }
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

            if (beatMovement != null && beatPositionX < 0f)
            {
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
}

public class BeatMovement : MonoBehaviour
{
    private float speed;
    private float canvasWidth;
    private RectTransform beatRectTransform;
    private BeatManager beatManager;
    private AudioSource beat_sound;
    private bool beat_triggered;

    public bool IsUsed { get; set; } = false; // Tracks whether the beat has been used
    private float spawnTime; // Time when the beat was spawned

    void Start()
    {
        beatRectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(float moveSpeed, float width, BeatManager manager, AudioSource sound, float correctedSpawnTime)
{
    speed = moveSpeed;
    canvasWidth = width;
    beatManager = manager;
    beat_sound = sound;
    spawnTime = correctedSpawnTime; // Use the corrected spawn time
}


    void Update()
    {
        // Calculate the position based on the time since spawn
        float elapsedTime = Time.time - spawnTime;
        float newXPosition = canvasWidth / 2 - speed * elapsedTime;
        beatRectTransform.anchoredPosition = new Vector2(newXPosition, beatRectTransform.anchoredPosition.y);

        // Trigger music or sound logic when the beat crosses certain thresholds
        if (newXPosition <= 5 && !beatManager.IsMusicPlaying())
        {
            beatManager.StartMusic();
        }
        else if (newXPosition <= 5 && !beat_triggered)
        {
            beat_triggered = true;
            beat_sound.Play();
        }

        // Fade-in and fade-out logic
        if (newXPosition > 0f)
        {
            float fadeInStart = canvasWidth * 0.35f; // 3/4 on the right
            float fadeInFactor = Mathf.InverseLerp(fadeInStart, 0f, newXPosition);
            SetAlpha(fadeInFactor);
        }
        else
        {
            float fadeOutStart = -canvasWidth * 0.35f; // 1/4 on the left
            float fadeOutFactor = Mathf.InverseLerp(0f, fadeOutStart, newXPosition);
            SetAlpha(1 - fadeOutFactor); // Fade out as it moves past the left side
        }

        // Remove and destroy beat if it goes off-screen
        if (newXPosition < -canvasWidth)
        {
            beatManager.RemoveBeat(gameObject);
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

