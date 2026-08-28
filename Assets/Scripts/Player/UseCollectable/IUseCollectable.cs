
/// <summary>
/// Interface that defines an object as an UseCollectable (interacts with a collectable over it).
/// </summary>
public interface IUseCollectable: IUnderMouse {

    /// <summary>
    /// Called when an valid Collectable is dropped over it. It will only be called if the collectable passes CanUse with true.
    /// </summary>
    /// <param name="collectableHover">The collectable that was dropped over it.</param>
    void HandleCollectable(Collectable collectableHover);

}
