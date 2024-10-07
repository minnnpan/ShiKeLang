using UnityEngine;

public class PlayBGMOnActive : MonoBehaviour
{
    public AudioSource bgmAudioSource;   // AudioSource for BGM
    public AudioSource sfxAudioSource;   // AudioSource for SFX
    public AudioClip bgmIntro;           // BGM intro clip (can be null)
    public AudioClip bgmLoop;            // Looping BGM clip
    public AudioClip sfxClip;            // SFX clip (can be null)

    private bool isPlayingLoop = false;  // A flag to check if the loop has started

    // This is called when the GameObject or script becomes active
    void OnEnable()
    {
        // Play the BGM intro (if provided) and the SFX simultaneously
        PlayBGMAndSFX(bgmIntro, bgmLoop, sfxClip);
    }

    void Update()
    {
        // Check if the intro has finished playing and the loop hasn't started yet
        if (!bgmAudioSource.isPlaying && !isPlayingLoop && bgmIntro != null)
        {
            PlayLoopBGM();
        }
    }

    // Function to play the BGM intro, loop, and SFX (if provided)
    public void PlayBGMAndSFX(AudioClip intro, AudioClip loop, AudioClip sfx)
    {
        isPlayingLoop = false;  // Reset the loop flag

        // Play the SFX if it is assigned
        if (sfx != null)
        {
            sfxAudioSource.clip = sfx;
            sfxAudioSource.loop = false;  // Ensure SFX doesn't loop
            sfxAudioSource.Play();
        }

        // If the intro is assigned, play it; otherwise, play the loop immediately
        if (intro != null)
        {
            bgmAudioSource.clip = intro;
            bgmAudioSource.loop = false;  // Ensure the intro doesn't loop
            bgmAudioSource.Play();
        }
        else
        {
            // No intro, just start the loop immediately
            PlayLoopBGM();
        }
    }

    // Function to start playing the looping BGM
    void PlayLoopBGM()
    {
        // Set the clip to the looping BGM and enable looping
        bgmAudioSource.clip = bgmLoop;
        bgmAudioSource.loop = true;  // Enable looping for the BGM
        bgmAudioSource.Play();
        isPlayingLoop = true;  // Mark that the loop has started
    }
}
