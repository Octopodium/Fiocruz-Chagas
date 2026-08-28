using UnityEngine;
// Base class for anything that can be collected / stored in the inventory
public abstract class Collectable : ScriptableObject
{
    public int collectableID;
    [SerializeField] protected string collectableName, description;
    [SerializeField] protected Sprite sprite;

    /// <summary>
    /// Returns the name of the collectable.
    /// </summary>
    /// <returns></returns>
    public string GetName() => collectableName;
    /// <summary>
    /// Returns the description of the collectable.
    /// </summary>
    /// <returns></returns>
    public string GetDescription() => description;
    /// <summary>
    /// Returns the sprite of the collectable.
    /// </summary>
    /// <returns></returns>
    public Sprite GetSprite() => sprite;
}
