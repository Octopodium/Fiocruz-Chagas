using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(
    fileName = "New Audio Event",
    menuName = "Audio Tool/Audio Event")]
    
public class AudioEvent : ScriptableObject
{
    [Header("Clips")]
    [SerializeField] private AudioClip[] clips;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    [SerializeField] private Vector2 randomVolume = Vector2.one;

    [Header("Pitch")]
    [SerializeField] private Vector2 randomPitch = new Vector2(1f, 1f);

    [Header("Playback")]
    [SerializeField] private bool loop;

    [SerializeField] private AudioMixerGroup output;

    public AudioClip GetClip()
    {
        if (clips == null || clips.Length == 0)
            return null;

        return clips[Random.Range(0, clips.Length)];
    }

    public float GetVolume()
    {
        return volume * Random.Range(randomVolume.x, randomVolume.y);
    }

    public float GetPitch()
    {
        return Random.Range(randomPitch.x, randomPitch.y);
    }

    public bool Loop => loop;

    public AudioMixerGroup Output => output;
}