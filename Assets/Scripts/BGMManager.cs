using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioSource audioSource;    // The AudioSource component to play the audio

    // First set of BGM (Intro and Loop)
    public AudioClip bgmIntro;         // The BGM intro clip
    public AudioClip bgmLoop;          // The looping BGM clip

    // Second set of BGM (Intro and Loop)
    public AudioClip bgmIntro2;        // The second BGM intro clip
    public AudioClip bgmLoop2;         // The second looping BGM clip

    private bool isPlayingLoop = false;   // A flag to check if the loop has started

    void Start()
    {
        // Start playing the first set of BGM intro at the start of the scene
        PlayBGM(bgmIntro, bgmLoop);
    }

    void Update()
    {
        // Check if the intro has finished playing and the loop hasn't started yet
        if (!audioSource.isPlaying && !isPlayingLoop)
        {
            PlayLoopBGM();
        }
    }

    // Function to play the BGM intro and set up the loop
    public void PlayBGM(AudioClip intro, AudioClip loop)
    {
        bgmIntro = intro;
        bgmLoop = loop;
        isPlayingLoop = false;

        // Start playing the BGM intro
        audioSource.clip = bgmIntro;
        audioSource.loop = false;  // Ensure intro doesn't loop
        audioSource.Play();
    }

    // Function to start playing the looping BGM after the intro
    void PlayLoopBGM()
    {
        // Set the clip to the looping BGM and enable looping
        audioSource.clip = bgmLoop;
        audioSource.loop = true;
        audioSource.Play();
        isPlayingLoop = true; // Set the flag to true so the loop doesn't trigger again
    }

    // Function to switch to the second set of intro and looping BGM
    public void SwitchToSecondBGM()
    {
        // Stop the current audio
        audioSource.Stop();

        // Play the second set of intro and loop
        PlayBGM(bgmIntro2, bgmLoop2);
    }

    // Function to stop the currently playing BGM
    public void StopBGM()
    {
        // Stop the audio
        audioSource.Stop();

        // Reset the isPlayingLoop flag to handle further BGM play if needed
        isPlayingLoop = false;
    }
}



