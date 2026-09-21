using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool
{
    private readonly Transform parent;

    private readonly List<PooledAudioSource> sources = new();

    public AudioSourcePool(Transform parent)
    {
        this.parent = parent;
    }

    public PooledAudioSource Get()
    {
        foreach (PooledAudioSource pooledSource in sources)
        {
            if (!pooledSource.Source.isPlaying)
                return pooledSource;
        }

        return CreateSource();
    }

    private PooledAudioSource CreateSource()
    {
        GameObject obj = new GameObject("Pooled Audio Source");

        obj.transform.SetParent(parent);

        AudioSource audioSource =
            obj.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        PooledAudioSource pooledSource =
            new PooledAudioSource(audioSource);

        sources.Add(pooledSource);

        return pooledSource;
    }
}