using UnityEngine;

public class AudioService : MonoBehaviour
{
    public static AudioService Instance { get; private set; }

    private AudioSourcePool pool;
    private AudioPlayer player;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        pool = new AudioSourcePool(transform);
        player = new AudioPlayer();
    }

    public void Play(AudioEvent audioEvent)
    {
        AudioSource source = pool.Get();

        source.spatialBlend = 0f;

        player.Play(source, audioEvent);
    }

    public void Play(
        AudioEvent audioEvent,
        Vector3 position)
    {
        AudioSource source = pool.Get();

        source.transform.position = position;
        source.spatialBlend = 1f;

        player.Play(source, audioEvent);
    }

}