using UnityEngine;

public class AudioEmitter : MonoBehaviour
{
    [SerializeField] private AudioEvent audioEvent;

    [SerializeField] private bool playOnStart = true;

    private AudioHandle handle;

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        handle = AudioService.Instance.Play(audioEvent);
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