using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the loading percentage of AmbientNavigation GoToCoroutine
/// </summary>
public class LoadingOverlayUI : MonoBehaviour {
    public Text label;

    void Awake() {
        GameManager.instance.navigation.onAmbientLoadingProgress += HandleLoadingProgress;
        // gameObject.SetActive(false);
    }

    void HandleLoadingProgress(float progress) {
        // label.gameObject.SetActive(progress != 1.0f);
        label.text = Mathf.Floor(progress * 100) + "%";
    }

    void OnDestroy() {
        GameManager.instance.navigation.onAmbientLoadingProgress -= HandleLoadingProgress;
    }
}
