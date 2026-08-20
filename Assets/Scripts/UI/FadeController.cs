using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeController : MonoBehaviour {
    public Image fadeImg;

    /// <summary>
    /// Fades from nothing to black
    /// </summary>
    public void FadeToBlack() {
        fadeImg.DOFade(1, 0.5f);
    }

    /// <summary>
    /// Fades from black to nothing
    /// </summary>
    public void FadeFromBlack() {
        fadeImg.DOFade(1, 0.5f);
    }
}
