using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;

/// <summary>
/// Controls screen fade to black. Used by AmbientNavigation GoToCoroutine.
/// </summary>
public class FadeController : MonoBehaviour {
    public CanvasGroup fadeGroup;
    public float fadeTime = 0.5f;

    /// <summary>
    /// Fades from nothing to black. This function only starts the coroutine FadeToBlackCoroutine. For more control over when it finishes, call the coroutine directly.
    /// </summary>
    /// <param name="onFaded">Optional callback, triggers when done.</param>
    public void FadeToBlack(Action onFaded = null) {
        StartCoroutine(FadeToBlackCoroutine(onFaded));
    }

    /// <summary>
    /// Fades from nothing to black. Called mostly by AmbientNavigation GoToCoroutine to fade to black while loading an ambient.
    /// </summary>
    /// <returns>Returns an Coroutine that will end when it's totally faded</returns>
    /// <param name="onFaded">Optional callback, triggers when done.</param>
    public IEnumerator FadeToBlackCoroutine(Action onFaded = null) {
        Tween tween = fadeGroup.DOFade(1, fadeTime);
        yield return tween.WaitForCompletion();
        onFaded?.Invoke();
    }

    /// <summary>
    /// Fades from black to nothing. This function only starts the coroutine FadeFromBlackCoroutine. For more control over when it finishes, call the coroutine directly.
    /// </summary>
    /// <param name="onFaded">Optional callback, triggers when done.</param>
    public void FadeFromBlack(Action onFaded = null) {
        StartCoroutine(FadeFromBlackCoroutine(onFaded));
    }

    /// <summary>
    /// Fades from black to nothing. Called mostly by AmbientNavigation GoToCoroutine to fade from black after loading an ambient.
    /// </summary>
    /// <returns>Returns an Coroutine that will end when it's totally faded</returns>
    /// <param name="onFaded">Optional callback, triggers when done.</param>
    public IEnumerator FadeFromBlackCoroutine(Action onFaded = null) {
        Tween tween = fadeGroup.DOFade(0, fadeTime);
        yield return tween.WaitForCompletion();
        onFaded?.Invoke();
    }
}
