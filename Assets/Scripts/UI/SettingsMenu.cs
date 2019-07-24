using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingsMenu : MonoBehaviour {
    public Slider volMaster;
    public Slider volUI;
    public Slider volMusic;
    public Slider volEffects;
    public Slider volAmbient;

    public Slider ctrlMoveSpeed;
    public Slider ctrlZoomSpeed;

    private void Start() {
        // Load settings and set sliders
        if (File.Exists(Global.PATH_SETTINGS)) {
            foreach (string line in File.ReadLines(Global.PATH_SETTINGS)) {
                string key = line.Split(':')[0];
                int val = int.Parse(line.Split(':')[1]);
                switch (key) {
                    case "MASTER_VOL":
                        volMaster.value = val;
                        break;
                    case "UI_VOL":
                        volUI.value = val;
                        break;
                    case "MUSIC_VOL":
                        volMusic.value = val;
                        break;
                    case "EFFECTS_VOL":
                        volEffects.value = val;
                        break;
                    case "AMBIENT_VOL":
                        volAmbient.value = val;
                        break;
                    case "CAMERA_SPEED":
                        ctrlMoveSpeed.value = val - (ctrlMoveSpeed.maxValue / 2);
                        break;
                    case "CAMERA_ZOOM_SPEED":
                        ctrlZoomSpeed.value = val - (ctrlZoomSpeed.maxValue / 2);
                        break;

                    default:
                        Debug.LogWarning("Could not recognize key <" + key + "> from settings file");
                        break;
                }
            }
        }
    }

    public void Save() {
        // VOLUMES
        // Set mixer values and save to file
        string path = Global.PATH_SETTINGS;
        File.WriteAllText(path, ""); // Reset file

        if (volMaster.value <= 0) {
            SoundManager.activeSoundManager.mixer.SetFloat("MasterVol", -80);
        } else {
            SoundManager.activeSoundManager.mixer.SetFloat("MasterVol", SoundManager.activeSoundManager.defaultMasterVol + (Mathf.Log10((volMaster.value) / volMaster.maxValue) * 15));
        }
        File.AppendAllText(path, "MASTER_VOL:" + volMaster.value + "\n");

        if (volUI.value <= 0) {
            SoundManager.activeSoundManager.mixer.SetFloat("UIVol", -80);
        } else {
            SoundManager.activeSoundManager.mixer.SetFloat("UIVol", SoundManager.activeSoundManager.defaultUIVol + (Mathf.Log10((volUI.value) / volUI.maxValue) * 15));
        }
        File.AppendAllText(path, "UI_VOL:" + volUI.value + "\n");

        if (volMusic.value <= 0) {
            SoundManager.activeSoundManager.mixer.SetFloat("MusicVol", -80);
        } else {
            SoundManager.activeSoundManager.mixer.SetFloat("MusicVol", SoundManager.activeSoundManager.defaultMusicVol + (Mathf.Log10((volMusic.value) / volMusic.maxValue) * 15));
        }
        File.AppendAllText(path, "MUSIC_VOL:" + volMusic.value + "\n");

        if (volEffects.value <= 0) {
            SoundManager.activeSoundManager.mixer.SetFloat("EffectsVol", -80);
        } else {
            SoundManager.activeSoundManager.mixer.SetFloat("EffectsVol", SoundManager.activeSoundManager.defaultEffectsVol + (Mathf.Log10((volEffects.value) / volEffects.maxValue) * 15));
        }
        File.AppendAllText(path, "EFFECTS_VOL:" + volEffects.value + "\n");

        if (volAmbient.value <= 0) {
            SoundManager.activeSoundManager.mixer.SetFloat("AmbientVol", -80);
        } else {
            SoundManager.activeSoundManager.mixer.SetFloat("AmbientVol", SoundManager.activeSoundManager.defaultAmbientVol + (Mathf.Log10((volAmbient.value) / volAmbient.maxValue) * 15));
        }
        File.AppendAllText(path, "AMBIENT_VOL:" + volAmbient.value + "\n");

        // CONTROLS
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            // Mobile
            // Currently nothing happens
        } else {
            // Desktop
            Global.CAMERA_SPEED = Global.CAMERA_DEFAULT_SPEED + (ctrlMoveSpeed.value - (ctrlMoveSpeed.maxValue / 2));
            File.AppendAllText(path, "CAMERA_SPEED:" + Global.CAMERA_SPEED + "\n");

            Global.CAMERA_ZOOM_SPEED = Global.CAMERA_DEFAULT_ZOOM_SPEED + (ctrlZoomSpeed.value - (ctrlZoomSpeed.maxValue / 2));
            File.AppendAllText(path, "CAMERA_ZOOM_SPEED:" + Global.CAMERA_ZOOM_SPEED + "\n");
        }
    }
}