using UnityEngine;

/// <summary>
/// Interactable used to switch ambients on click. Normally used at doors.
/// </summary>
public class TravelToAmbient : IInteractable {
    public AmbientInfo ambient;

    public override string GetHoverText() {
        return "Ir para " + ambient.ambientName;
    }
    
    public override void HandleInteract() {
        GameManager.instance.navigation.GoTo(ambient);
    }

    public override bool CanBeFound() {
        return true;
    }

}
