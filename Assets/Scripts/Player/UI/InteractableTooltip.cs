using UnityEngine;
using UnityEngine.UI;

public class InteractableTooltip : MonoBehaviour {
    public Text label;

    void Awake() {
        GameManager.instance.player.onHoverTextChange += HandleHoverChange;
    }

    /// <summary>
    /// Called when the current interactable being hovered changed.
    /// Displays on screen a small text description of the possible action of interacting with the interactable.
    /// </summary>
    /// <param name="hoverText">The text to appear on the tooltip. If an empty string, hides the tooltip.</param>
    void HandleHoverChange(string hoverText) {
        label.text = hoverText;
    }


    void OnDestroy() {
        GameManager.instance.player.onHoverTextChange -= HandleHoverChange;
    }
}
