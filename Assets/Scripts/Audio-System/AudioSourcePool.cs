using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool
{
    private readonly Transform parent;
    private readonly List<AudioSource> sources = new();

    public AudioSourcePool(Transform parent)
    {
        this.parent = parent;
    }

    public AudioSource Get()
    {
        foreach (AudioSource source in sources)
        {
            if (!source.isPlaying)
                return source;
        }

        return CreateSource();
    }

    private AudioSource CreateSource()
    {
        GameObject obj = new GameObject("Pooled Audio Source");

        obj.transform.SetParent(parent);

        AudioSource source = obj.AddComponent<AudioSource>();

        source.playOnAwake = false;

        sources.Add(source);

        return source;
    }
}