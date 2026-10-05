using UnityEngine;

public class OptionsController : MonoBehaviour {
    public VolumeControl musicControl, sfxControl;

    void Start() {
        LoadAll();
    }

    public void LoadAll() {
        musicControl.Load();
        sfxControl.Load();
    }

    public void SaveAll() {
        musicControl.Save();
        sfxControl.Save();
    }
}
