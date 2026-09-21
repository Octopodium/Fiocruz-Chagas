using UnityEngine;

public class AudioPlayer
{
    public AudioHandle Play(
        PooledAudioSource pooledSource,
        AudioEvent audioEvent)
    {
        if (audioEvent == null)
            return null;

        AudioClip clip = audioEvent.GetClip();

        if (clip == null)
            return null;

        AudioSource source = pooledSource.Source;

        int playbackId = pooledSource.BeginPlayback();

        source.clip = clip;
        source.volume = audioEvent.GetVolume();
        source.pitch = audioEvent.GetPitch();
        source.loop = audioEvent.Loop;
        source.outputAudioMixerGroup = audioEvent.Output;

        source.Play();

        return new AudioHandle(
            pooledSource,
            playbackId);
    }
}