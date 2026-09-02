
/// <summary>
/// Interface that defines an object as able to be found by the camera raycast.
/// </summary>
public interface IUnderMouse {
    /// <summary>
    /// Defines the text shown when hovering. It will only be called if CanBeFound return true.
    /// </summary>
    /// <returns>A small description of the object or the action you can perform with it.</returns>
    string GetHoverText();

    /// <summary>
    /// Defines if this object can be found by the camera rasycast. In certain cases, the object may want to stay hidden for a time.
    /// </summary>
    /// <returns>True if will be detected by the camera raycast.</returns>
    bool CanBeFound();
}
