using System.Collections.Generic;
using UnityEngine;

public class AudioEmitter : MonoBehaviour
{
    [SerializeField] private AudioEvent audioEvent;

    [SerializeField] private bool playOnStart = true;

    [SerializeField] private bool playOnlyOnce = false;
    // Temp fix
    static HashSet<AudioEvent> alreadyPlayed = new HashSet<AudioEvent>();

    private AudioHandle handle;

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        if (playOnlyOnce && alreadyPlayed.Contains(audioEvent)) return;
        handle = AudioService.Instance.Play(audioEvent);
        if (playOnlyOnce) alreadyPlayed.Add(audioEvent);
    }

    public void Pause()
    {
        handle?.Pause();
    }

    public void Resume()
    {
        handle?.Resume();
    }

    public void Stop()
    {
        handle?.Stop();
    }
}