using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public float pitchRange = 0.1f; // Range for random pitch variation

    private AudioSource currentAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            // Stop the currently playing sound
            if (currentAudioSource != null && currentAudioSource.isPlaying)
            {
                Destroy(currentAudioSource); // Destroy current AudioSource
            }

            // Create a new AudioSource for the new sound
            currentAudioSource = gameObject.AddComponent<AudioSource>();
            currentAudioSource.pitch = Random.Range(1.0f - pitchRange, 1.0f + pitchRange);
            currentAudioSource.PlayOneShot(clip);

            // Destroy the AudioSource after the clip finishes playing
            Destroy(currentAudioSource, clip.length);
        }
    }
}
