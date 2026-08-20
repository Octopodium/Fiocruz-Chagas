using UnityEngine;
// Base class for anything that can be collected / stored in the inventory
public abstract class Collectable : ScriptableObject
{
    public int collectableID;
    [SerializeField] protected string collectableName, description;
    [SerializeField] protected Sprite sprite;

    public string GetName() => collectableName;
    public string GetDescription() => description;
    public Sprite GetSprite() => sprite;
}
