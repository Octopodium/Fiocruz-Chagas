using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A generic Interactable that triggers an event when clicked over it.
/// </summary>
public class GenericInteractable : IInteractable {
    public string genericName = "objeto";
    public UnityEvent onInteracted;

    public override string GetHoverText() {
        return "Interagir com " + genericName;
    }
    
    public override void HandleInteract() {
        onInteracted?.Invoke();
    }

    public override bool CanBeFound() {
        return true;
    }

}
