using UnityEngine.Audio;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioMixerGroup mixer;

    // The different arrays should represent the different 2D sound folders (sound categories in /Assets/Resources/Sounds/2D/<folders>)
    public Sound[] uiSounds;
    public Sound[] musicSounds;
    public Sound[] effectSounds;
    public enum SoundType {
        UI,
        MUSIC,
        EFFECT
    }

    public static SoundManager activeSoundManager;

    private void Awake() {
        if (activeSoundManager == null) activeSoundManager = this;
        else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Create audio sources for all registered sounds
        InitSounds(uiSounds);
        InitSounds(musicSounds);
        InitSounds(effectSounds);
    }

    private void InitSounds(Sound[] sounds) {
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = mixer;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
        }
    }

    /* CURRENTLY DISABLED / NOT NEEDED / MAYBE FOR MUSIC
    private void Start() {
        
    }*/

    public void Play(SoundType st, string name) {
        Sound foundSound = FindSound(st, name);
        if (foundSound != null) {
            foundSound.source.Play();
        }
    }

    public void Play(SoundType st, string name, float delayInSec) {
        Sound foundSound = FindSound(st, name);
        if (foundSound != null) {
            foundSound.source.PlayDelayed(delayInSec);
        }
    }

    /**
        Returns the found sound or null if not existing
     */
    private Sound FindSound(SoundType st, string name) {
        Sound foundSound = null;

        switch (st) {
            case SoundType.UI:
                foundSound = System.Array.Find(uiSounds, s => s.name == name);
                break;
            case SoundType.MUSIC:
                foundSound = System.Array.Find(musicSounds, s => s.name == name);
                break;
            case SoundType.EFFECT:
                foundSound = System.Array.Find(effectSounds, s => s.name == name);
                break;

            default:
                Debug.LogWarning("Invalid SoundType <" + st + ">: could not be found");
                break;
        }

        if (foundSound == null) Debug.LogWarning("Invalid sound name <" + name + "> for sound type <" + st + ">: could not be found");
        return foundSound;
    }
}
