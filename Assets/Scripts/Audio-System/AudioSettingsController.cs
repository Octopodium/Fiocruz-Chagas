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
        float volume = Mathf.Log10(
            Mathf.Max(value, 0.0001f)) * 20f;

        audioMixer.SetFloat(
            MusicVolume,
            volume);

        PlayerPrefs.SetFloat(
            MusicVolume,
            value);
    }

    public void SetSFXVolume(float value)
    {
        float volume = Mathf.Log10(
            Mathf.Max(value, 0.0001f)) * 20f;

        audioMixer.SetFloat(
            SFXVolume,
            volume);

        PlayerPrefs.SetFloat(
            SFXVolume,
            value);
    }

    private void LoadSettings()
    {
        float musicVolume =
            PlayerPrefs.GetFloat(
                MusicVolume,
                1f);

        float sfxVolume =
            PlayerPrefs.GetFloat(
                SFXVolume,
                1f);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }
}