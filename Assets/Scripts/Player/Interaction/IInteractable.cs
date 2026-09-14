
/// <summary>
/// Abstract class that defines an object as an Interactable (clickable by the player).
/// </summary>
public abstract class IInteractable: IUnderMouse {

    /// <summary>
    /// Called when an Interactable is interacted with (clicked). It will only be called if CanInteract return true.
    /// </summary>
    public abstract void HandleInteract();

}
