using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
    public bool loop;

    [Range(0f, 1f)]
    public float volume;

    [HideInInspector]
    public AudioSource source;

    public Sound(string name, AudioClip clip, AudioMixerGroup mixerGroup, bool loop, float volume) {
        this.name = name;
        this.clip = clip;
        this.mixerGroup = mixerGroup;
        this.loop = loop;
        this.volume = volume;
    }
}
