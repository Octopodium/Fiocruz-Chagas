using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MusicVolume = "MusicVolume";
    private const string SFXVolume = "SFXVolume";

    private void Start()
    {
        LoadSettings();
    }

    public void SetMusicVolume(float value)
    {
        SetVolume(MusicVolume, value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume(SFXVolume, value);
    }

    private void SetVolume(string parameter, float value)
    {
        float volumeDB = value <= 0.0001f
            ? -80f
            : Mathf.Log10(value) * 20f;

        bool success = audioMixer.SetFloat(parameter, volumeDB);

        if (!success)
        {
            Debug.LogError(
                $"AudioMixer parameter '{parameter}' not found!"
            );
            return;
        }

        PlayerPrefs.SetFloat(parameter, value);

        Debug.Log(
            $"{parameter}: {value:F2} | {volumeDB:F2} dB"
        );
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolume, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolume, 1f);

        musicSlider.SetValueWithoutNotify(musicVolume);
        sfxSlider.SetValueWithoutNotify(sfxVolume);

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}