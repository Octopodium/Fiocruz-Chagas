
/// <summary>
/// Interface that defines an object as an UseCollectable (interacts with a collectable over it).
/// </summary>
public interface IUseCollectable {

    /// <summary>
    /// Called when an valid Collectable is dropped over it.
    /// </summary>
    void HandleCollectable(Collectable collectableHover);

    /// <summary>
    /// Defines which is the text shown when hovering a Collectable over it.
    /// </summary>
    /// <returns>The text to be displayed</returns>
    string GetHoverText(Collectable collectableHover);

    /// <summary>
    /// Defines which is the text shown when hovering a Collectable over it.
    /// </summary>
    /// <returns>The text to be displayed</returns>
    bool CanUse(Collectable collectableHover);
}
