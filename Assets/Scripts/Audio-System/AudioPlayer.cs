using UnityEngine;

public class AudioPlayer
{
    public void Play(
        AudioSource source,
        AudioEvent audioEvent)
    {
        if (audioEvent == null)
            return;

        AudioClip clip = audioEvent.GetClip();

        if (clip == null)
            return;

        source.clip = clip;
        source.volume = audioEvent.GetVolume();
        source.pitch = audioEvent.GetPitch();
        source.loop = audioEvent.Loop;
        source.outputAudioMixerGroup = audioEvent.Output;

        source.Play();
    }
}