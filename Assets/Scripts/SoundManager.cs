using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;

        [HideInInspector]
        public AudioSource source;
    }

    public Sound[] sounds;
    public Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 初始化音频源
        InitializeSounds();
    }

    private void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            soundDictionary[s.name] = s;
        }
    }

    public void Play(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            s.source.volume = Mathf.Clamp(s.volume * 2f, 0f, 1f);  // 将音量增加一倍，但不超过1
            s.source.pitch = Mathf.Clamp(s.pitch * 1.5f, 0.1f, 3f);  // 将音高增加50%，但保持在合理范围内
            s.source.Play();
            Debug.Log($"SoundManager: Playing {name}. Volume: {s.source.volume}, Pitch: {s.source.pitch}");
        }
    }

    public void Stop(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            s.source.Stop();
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }
}
