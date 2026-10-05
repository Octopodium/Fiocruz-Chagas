using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour {
    
    public string track;
    public string playerPrefName;
    [Range(0, 1)] public float defaultValue;

    [Header("References")]
    public AudioMixer mixer;
    public Slider slider;

    void Start() {
        if (string.IsNullOrEmpty(playerPrefName)) playerPrefName = track;
        Load();
    }

    public void OnSliderChanged(float value) {
        mixer.SetFloat(track, NormalizedToVolume(slider.value));
    }

    public void RefreshSlider() {
        mixer.GetFloat(track, out float volume);
        slider.value = VolumeToNormalized(volume);
    }

    public void Load() {
        float normalized = PlayerPrefs.HasKey(playerPrefName) ? PlayerPrefs.GetFloat(playerPrefName) : defaultValue;
        mixer.SetFloat(track, NormalizedToVolume(normalized));
        RefreshSlider();
    }

    public void Save() {
        mixer.GetFloat(track, out float volume);
        PlayerPrefs.SetFloat(playerPrefName, VolumeToNormalized(volume));
    }


    float NormalizedToVolume(float normalized) => Mathf.Log10(normalized) * 20f;
    float VolumeToNormalized(float volume) => Mathf.Pow(10, volume / 20f);
}
