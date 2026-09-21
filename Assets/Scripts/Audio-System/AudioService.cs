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

    public AudioHandle Play(AudioEvent audioEvent)
    {
        PooledAudioSource pooledSource = pool.Get();

        pooledSource.Source.spatialBlend = 0f;

        return player.Play(
            pooledSource,
            audioEvent);
    }

    public AudioHandle Play(
        AudioEvent audioEvent,
        Vector3 position)
    {
        PooledAudioSource pooledSource = pool.Get();

        pooledSource.Source.transform.position = position;
        pooledSource.Source.spatialBlend = 1f;

        return player.Play(
            pooledSource,
            audioEvent);
    }
}