using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SaturationTransition : MonoBehaviour
{
    [SerializeField] private Volume volume;
    private ColorAdjustments colorAjust;
    private float initialSaturation;
    [SerializeField] private float fadeDuration = 3.0f;

    private void Awake()
    {
        volume = GetComponent<Volume>();
        if(volume.profile.TryGet<ColorAdjustments>(out colorAjust))
        {
            initialSaturation = colorAjust.saturation.value;
            Debug.Log("Encontrado.");
        }
    }

    public void SetSaturation(float value)
    {
        colorAjust.saturation.value = value;
    }

    public async void FadeColor()
    {
        float timer = 0;
        float saturation;
        float timeScale;
        while(timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            saturation = Mathf.Lerp(-100, initialSaturation, timer / fadeDuration);
            timeScale = Mathf.Lerp(0, 1, timer / fadeDuration);
            colorAjust.saturation.value = saturation;
            Time.timeScale = timeScale;
            await Awaitable.EndOfFrameAsync();
        }
        colorAjust.saturation.value = initialSaturation;
        Time.timeScale = 1.0f;
    }

}
