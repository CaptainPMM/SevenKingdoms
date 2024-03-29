﻿using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System.IO;

public class SoundManager : MonoBehaviour {
    public AudioMixer mixer;

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

    [HideInInspector] public float defaultMasterVol;
    [HideInInspector] public float defaultUIVol;
    [HideInInspector] public float defaultMusicVol;
    [HideInInspector] public float defaultEffectsVol;
    [HideInInspector] public float defaultAmbientVol;

    [SerializeField]
    private List<Sound> loopingSounds;
    private SoundState currSoundState;

    private void Awake() {
        if (activeSoundManager == null) activeSoundManager = this;
        else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        loopingSounds = new List<Sound>();

        // Create audio sources for all registered sounds
        InitSounds(uiSounds);
        InitSounds(musicSounds);
        InitSounds(effectSounds);
    }

    private void InitSounds(Sound[] sounds) {
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixerGroup;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
        }
    }

    private void Start() {
        // Setup sound states for the music
        SoundState st1 = new SoundState(FindSound(SoundType.MUSIC, "ost_lvl1_loop"), 15, float.MinValue, null);

        SoundState st2trans = new SoundState(FindSound(SoundType.MUSIC, "ost_lvl2_transition"), float.MinValue, float.MinValue, null);
        st1.SetNextSoundState(st2trans);

        SoundState st2 = new SoundState(FindSound(SoundType.MUSIC, "ost_lvl2_loop"), 22, 15, st1);
        st2trans.SetNextSoundState(st2);

        SoundState st3trans = new SoundState(FindSound(SoundType.MUSIC, "ost_lvl3_transition"), float.MinValue, float.MinValue, null);
        st2.SetNextSoundState(st3trans);

        SoundState st3 = new SoundState(FindSound(SoundType.MUSIC, "ost_lvl3_loop"), 38, 22, st2);
        st3trans.SetNextSoundState(st3);

        SoundState st4trans = new SoundState(FindSound(SoundType.MUSIC, "ost_lvl4_transition"), float.MinValue, float.MinValue, null);
        st3.SetNextSoundState(st4trans);

        SoundState st4 = new SoundState(FindSound(SoundType.MUSIC, "ost_lvl4_loop"), float.MaxValue, null, 38, st3);
        st4trans.SetNextSoundState(st4);

        // Init first sound state
        currSoundState = st1;
        currSoundState.Play();

        // Load default mixer vol values
        mixer.GetFloat("MasterVol", out defaultMasterVol);
        mixer.GetFloat("UIVol", out defaultUIVol);
        mixer.GetFloat("MusicVol", out defaultMusicVol);
        mixer.GetFloat("EffectsVol", out defaultEffectsVol);
        mixer.GetFloat("AmbientVol", out defaultAmbientVol);

        // Load settings file (also settings not concerning sound -> but sound manager is always present)
        if (File.Exists(Global.PATH_SETTINGS)) {
            foreach (string line in File.ReadLines(Global.PATH_SETTINGS)) {
                string key = line.Split(':')[0];
                int val = int.Parse(line.Split(':')[1]);
                switch (key) {
                    case "MASTER_VOL":
                        if (val <= 0) {
                            mixer.SetFloat("MasterVol", -80);
                        } else {
                            mixer.SetFloat("MasterVol", defaultMasterVol + (Mathf.Log10((float)val / 10f) * 15)); // HARD CODED SLIDER MAX VALUE OF 10!!!
                        }
                        break;
                    case "UI_VOL":
                        if (val <= 0) {
                            mixer.SetFloat("UIVol", -80);
                        } else {
                            mixer.SetFloat("UIVol", defaultUIVol + (Mathf.Log10((float)val / 10f) * 15)); // HARD CODED SLIDER MAX VALUE OF 10!!!
                        }
                        break;
                    case "MUSIC_VOL":
                        if (val <= 0) {
                            mixer.SetFloat("MusicVol", -80);
                        } else {
                            mixer.SetFloat("MusicVol", defaultMusicVol + (Mathf.Log10((float)val / 10f) * 15)); // HARD CODED SLIDER MAX VALUE OF 10!!!
                        }
                        break;
                    case "EFFECTS_VOL":
                        if (val <= 0) {
                            mixer.SetFloat("EffectsVol", -80);
                        } else {
                            mixer.SetFloat("EffectsVol", defaultEffectsVol + (Mathf.Log10((float)val / 10f) * 15)); // HARD CODED SLIDER MAX VALUE OF 10!!!
                        }
                        break;
                    case "AMBIENT_VOL":
                        if (val <= 0) {
                            mixer.SetFloat("AmbientVol", -80);
                        } else {
                            mixer.SetFloat("AmbientVol", defaultAmbientVol + (Mathf.Log10((float)val / 10f) * 15)); // HARD CODED SLIDER MAX VALUE OF 10!!!
                        }
                        break;
                    case "CAMERA_SPEED":
                        Global.CAMERA_SPEED = val;
                        break;
                    case "CAMERA_ZOOM_SPEED":
                        Global.CAMERA_ZOOM_SPEED = val;
                        break;

                    default:
                        Debug.LogWarning("Could not recognize key <" + key + "> from settings file");
                        break;
                }
            }
        }
    }

    private void Update() {
        float currVal = 0;
        if (GameController.activeGameController != null) currVal = GameController.activeGameController.locationsHeldByPlayer;

        int soundStateTransitionAnswer = currSoundState.TransitionPossible(currVal);
        if (soundStateTransitionAnswer != 0) {
            if (soundStateTransitionAnswer > 0) {
                currSoundState = currSoundState.NextState();
            } else {
                currSoundState = currSoundState.BackState();
            }
            currSoundState.Play();
        }
        // No transition possible -> skip
    }

    public static void Play(SoundType st, string name) { activeSoundManager.PlaySound(st, name); }
    private void PlaySound(SoundType st, string name) {
        Sound foundSound = FindSound(st, name);
        if (foundSound != null) {
            foundSound.source.Play();
            if (foundSound.loop && loopingSounds.Find(s => s.name == name) == null) loopingSounds.Add(foundSound);
        }
    }

    public static void Play(SoundType st, string name, float delayInSec) { activeSoundManager.PlaySound(st, name, delayInSec); }
    private void PlaySound(SoundType st, string name, float delayInSec) {
        Sound foundSound = FindSound(st, name);
        if (foundSound != null) {
            foundSound.source.PlayDelayed(delayInSec);
            if (foundSound.loop && loopingSounds.Find(s => s.name == name) == null) loopingSounds.Add(foundSound);
        }
    }

    public static bool IsCurrentlyLooping(SoundType st, string name) { return activeSoundManager.SoundIsCurrentlyLooping(st, name); }
    private bool SoundIsCurrentlyLooping(SoundType st, string name) {
        Sound foundSound = FindSound(st, name);
        if (foundSound != null) {
            if (loopingSounds.Find(s => s.name == name) != null) return true;
        }
        return false;
    }

    public static bool IsCurrentlyLooping(Sound sound) { return activeSoundManager.SoundIsCurrentlyLooping(sound); }
    private bool SoundIsCurrentlyLooping(Sound sound) {
        if (sound != null) {
            if (loopingSounds.Find(s => s.name == sound.name) != null) return true;
        }
        return false;
    }

    /** 
        Returns false on error and true on success
     */
    public static bool StopLoop(SoundType st, string name) { return activeSoundManager.StopLoopingSound(st, name); }
    private bool StopLoopingSound(SoundType st, string name) {
        Sound foundSound = FindSound(st, name);
        if (foundSound != null && SoundIsCurrentlyLooping(foundSound)) {
            foundSound.source.Stop();
            loopingSounds.Remove(foundSound);
            return true;
        }
        return false;
    }

    /**
        Used to directly play sounds from the button on click handlers in the inspector.
        Provide following parameters in the parameters string (seperated by comma (no spaces)):
            1st-> operation: "play"/"playdelay"/stop"
            2nd-> soundType: <name of the sound type (UI, MUSIC, etc.)>
            3rd-> soundName: <name of the sound>
           [4th-> playDelay: <delay as float> (!only needed if operation = "playdelay"!)]
     */
    public void InspectorPlay(string parameters) {
        string[] p = parameters.Split(',');
        if (p.Length < 3) {
            Debug.LogWarning("SoundManager.InspectorPlay parameters missing");
            return;
        }

        string operation = p[0];
        string soundTypeString = p[1];
        SoundType soundType = SoundType.UI; // Default
        string soundName = p[2];

        bool foundSoundType = false;
        foreach (SoundType st in System.Enum.GetValues(typeof(SoundType))) {
            if (st.ToString() == soundTypeString) {
                soundType = st;
                foundSoundType = true;
                break;
            }
        }
        if (!foundSoundType) {
            Debug.LogWarning("SoundManager.InspectorPlay could not evaluate parameter[1] (soundType) <" + p[1] + ">");
            return;
        }

        switch (operation) {
            case "play":
                Play(soundType, soundName);
                return;
            case "playdelay":
                if (p.Length < 4) {
                    Debug.LogWarning("SoundManager.InspectorPlay parameter[3] (delay) missing");
                    return;
                }
                Play(soundType, soundName, float.Parse(p[3]));
                return;
            case "stop":
                if (!StopLoop(soundType, soundName)) {
                    Debug.LogWarning("SoundManager.InspectorPlay an error occured while stopping a loop with name <" + soundName + ">");
                }
                return;

            default:
                Debug.LogWarning("SoundManager.InspectorPlay could not evaluate parameter[0] (operation) <" + p[0] + ">");
                return;
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
