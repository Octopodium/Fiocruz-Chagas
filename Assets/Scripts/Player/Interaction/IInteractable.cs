
/// <summary>
/// Interface that defines an object as an Interactable (clickable by the player).
/// </summary>
public interface IInteractable: IUnderMouse {

    /// <summary>
    /// Called when an Interactable is interacted with (clicked). It will only be called if CanInteract return true.
    /// </summary>
    void HandleInteract();

}
