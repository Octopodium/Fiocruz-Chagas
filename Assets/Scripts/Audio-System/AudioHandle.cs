using UnityEngine;

public class AudioHandle
{
    private readonly PooledAudioSource pooledSource;
    private readonly int playbackId;

    public AudioHandle(
        PooledAudioSource pooledSource,
        int playbackId)
    {
        this.pooledSource = pooledSource;
        this.playbackId = playbackId;
    }

    public bool IsValid =>
        pooledSource != null &&
        pooledSource.IsPlaybackValid(playbackId);

    public bool IsPlaying =>
        IsValid &&
        pooledSource.Source.isPlaying;

    public bool IsPaused { get; private set; }

    public void Pause()
    {
        if (!IsValid || !IsPlaying)
            return;

        pooledSource.Source.Pause();

        IsPaused = true;
    }

    public void Resume()
    {
        if (!IsValid || !IsPaused)
            return;

        pooledSource.Source.UnPause();

        IsPaused = false;
    }

    public void Stop()
    {
        if (!IsValid)
            return;

        pooledSource.Source.Stop();

        IsPaused = false;
    }

    public void SetVolume(float volume)
    {
        if (!IsValid)
            return;

        pooledSource.Source.volume =
            Mathf.Clamp01(volume);
    }

    public void SetPitch(float pitch)
    {
        if (!IsValid)
            return;

        pooledSource.Source.pitch = pitch;
    }
}