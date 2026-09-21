using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField] private AudioEvent music;

    private AudioHandle musicHandle;

    private void Start()
    {
        musicHandle =
            AudioService.Instance.Play(music);
    }
}