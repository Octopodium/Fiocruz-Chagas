using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

/// <summary>
/// Controls screen fade to black. Used by AmbientNavigation GoToCoroutine.
/// </summary>
public class FadeController : MonoBehaviour {
    public CanvasGroup fadeGroup;
    public float fadeTime = 0.5f;


    /// <summary>
    /// Fades from nothing to black. This function only starts the coroutine FadeToBlackCoroutine. For more control over when it finishes, call the coroutine directly.
    /// </summary>
    public void FadeToBlack() {
        StartCoroutine(FadeToBlackCoroutine());
    }

    /// <summary>
    /// Fades from nothing to black. Called mostly by AmbientNavigation GoToCoroutine to fade to black while loading an ambient.
    /// </summary>
    /// <returns>Returns an Coroutine that will end when it's totally faded</returns>
    public IEnumerator FadeToBlackCoroutine() {
        Tween tween = fadeGroup.DOFade(1, fadeTime);
        yield return tween.WaitForCompletion();
    }

    /// <summary>
    /// Fades from black to nothing. This function only starts the coroutine FadeFromBlackCoroutine. For more control over when it finishes, call the coroutine directly.
    /// </summary>
    public void FadeFromBlack() {
        StartCoroutine(FadeFromBlackCoroutine());
    }

    /// <summary>
    /// Fades from black to nothing. Called mostly by AmbientNavigation GoToCoroutine to fade from black after loading an ambient.
    /// </summary>
    /// <returns>Returns an Coroutine that will end when it's totally faded</returns>
    public IEnumerator FadeFromBlackCoroutine() {
        Tween tween = fadeGroup.DOFade(0, fadeTime);
        yield return tween.WaitForCompletion();
    }
}
