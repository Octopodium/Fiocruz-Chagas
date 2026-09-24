
using UnityEngine;

/// <summary>
/// Abstract class that defines an object as an Interactable (clickable by the player).
/// </summary>
public abstract class IInteractable: IUnderMouse {

    /// <summary>
    /// Called when an Interactable is interacted with (clicked). It will only be called if CanInteract return true.
    /// </summary>
    public abstract void HandleInteract();

    #if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        Gizmos.color = CanBeFound() ? Color.cyan : Color.red;
        Gizmos.color *= new Color(1, 1, 1, 0.2f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center , collider.size * 1.05f);
    }
    #endif

}
