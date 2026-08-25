
/// <summary>
/// Interface that defines an object as an Interactable (clickable by the player).
/// </summary>
public interface IInteractable {

    /// <summary>
    /// Called when an Interactable is interacted with (clicked).
    /// </summary>
    void HandleInteract();

    /// <summary>
    /// Defines which is the text shown when hovering this interactable.
    /// </summary>
    /// <returns>The text to be displayed</returns>
    string GetHoverText();
}
