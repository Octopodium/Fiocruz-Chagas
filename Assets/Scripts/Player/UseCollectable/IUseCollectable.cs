
/// <summary>
/// Interface that defines an object as an UseCollectable (interacts with a collectable over it).
/// </summary>
public interface IUseCollectable: IUnderMouse {

    /// <summary>
    /// Called when an valid Collectable is dropped over it. It will only be called if the collectable passes CanUse with true.
    /// </summary>
    /// <param name="collectableHover">The collectable that was dropped over it.</param>
    void HandleCollectable(Collectable collectableHover);

    /// <summary>
    /// Defines which is the text shown when hovering a Collectable over it. It will only show if the collectable passes CanUse with true.
    /// </summary>
    /// <param name="collectableHover">The collectable that is being currently held.</param>
    /// <returns>The text to be displayed</returns>
    string GetHoverText(Collectable collectableHover);
}
