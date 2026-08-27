
/// <summary>
/// Interface that defines an object as an Interactable (clickable by the player).
/// </summary>
public interface IInteractable: IUnderMouse {

    /// <summary>
    /// Called when an Interactable is interacted with (clicked). It will only be called if CanInteract return true.
    void HandleInteract();

    /// <summary>
    /// Defines which is the text shown when hovering this interactable. It will only be called if CanInteract return true.
    /// </summary>
    /// <returns>The text to be displayed</returns>
    string GetHoverText();
}
