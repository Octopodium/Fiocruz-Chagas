
/// <summary>
/// Interface that defines an object as able to be found by the camera raycast.
/// </summary>
public interface IUnderMouse {
    /// <summary>
    /// Defines the text shown when hovering. It will only be called if CanBeFound return true.
    /// </summary>
    /// <returns></returns>
    string GetHoverText();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    bool CanBeFound();
}
