using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;

/// <summary>
/// Controls screen fade to black. Used by AmbientNavigation GoToCoroutine.
/// </summary>
public class FadeController : MonoBehaviour {
    /// For external use only. Never used in any method of this class.
    public enum FadeOptions { FadeInOut, FadeInOnly, FadeOutOnly, DontFade }

    // Configurable options
    [SerializeField] private CanvasGroup fadeGroup;
    public float fadeTime = 0.5f;

    // Usefull properties
    public bool isOnBlack => fadeGroup.alpha == 1;
    public bool isOnClear => fadeGroup.alpha == 0;

    // Internal fields
    bool isFadingToBlack = false;
    bool isFadingFromBlack = false;
    Action onFadedToBlack;
    Action onFadedFromBlack;



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
        if (isOnBlack) {
            onFaded?.Invoke();
            yield break;
        }

        if (onFaded != null) onFadedToBlack += onFaded;
        if (isFadingToBlack) yield break;

        Tween tween = fadeGroup.DOFade(1, fadeTime);

        isFadingToBlack = true;
        yield return tween.WaitForCompletion();
        isFadingToBlack = false;

        onFadedToBlack?.Invoke();
        onFadedToBlack = null;
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
        if (isOnClear) {
            onFaded?.Invoke();
            yield break;
        }

        if (onFaded != null) onFadedFromBlack += onFaded;
        if (isFadingFromBlack) yield break;

        Tween tween = fadeGroup.DOFade(0, fadeTime);

        isFadingFromBlack = true;
        yield return tween.WaitForCompletion();
        isFadingFromBlack = false;

        onFadedFromBlack?.Invoke();
        onFadedFromBlack = null;
    }

    public void SetOnBlack(bool isOn = true) {
        if (isOn) fadeGroup.alpha = 1;
        else fadeGroup.alpha = 0;
    }
}
