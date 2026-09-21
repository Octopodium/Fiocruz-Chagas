using UnityEngine;

public class PooledAudioSource
{
    public AudioSource Source { get; }

    public int PlaybackId { get; private set; }

    public PooledAudioSource(AudioSource source)
    {
        Source = source;
    }

    public int BeginPlayback()
    {
        PlaybackId++;

        return PlaybackId;
    }

    public bool IsPlaybackValid(int id)
    {
        return PlaybackId == id;
    }
}