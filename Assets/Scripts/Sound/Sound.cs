using UnityEngine;

[System.Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
    public bool loop;

    [Range(0f, 1f)]
    public float volume;

    [HideInInspector]
    public AudioSource source;

    public Sound(string name, AudioClip clip, bool loop, float volume) {
        this.name = name;
        this.clip = clip;
        this.loop = loop;
        this.volume = volume;
    }
}
