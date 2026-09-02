using UnityEngine;

/// <summary>
/// Interactable used to switch ambients on click. Normally used at doors.
/// </summary>
public class TravelToAmbient : MonoBehaviour, IInteractable {
    public AmbientInfo ambient;

    public string GetHoverText() {
        return "Ir para " + ambient.ambientName;
    }
    
    public void HandleInteract() {
        GameManager.instance.navigation.GoTo(ambient);
    }

    public bool CanBeFound() {
        return true;
    }

}
