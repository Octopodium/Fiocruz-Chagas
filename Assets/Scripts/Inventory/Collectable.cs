using UnityEngine;
// Base class for anything that can be collected / stored in the inventory
public abstract class Collectable : ScriptableObject
{
    public int collectableID;
    [SerializeField] protected string collectableName, description;
    [SerializeField] protected Sprite sprite;
    [SerializeField, Tooltip("Optional, sets flag when enter inventory, unsets when leaves inventory.")] protected string relatedFlag;

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
    /// <summary>
    /// Returns the related flag of the collectable.
    /// </summary>
    /// <returns></returns>
    public string GetRelatedFlag() => relatedFlag;
}
