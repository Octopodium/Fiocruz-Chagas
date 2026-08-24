using UnityEngine;

public class TravelToAmbient : MonoBehaviour, IInteractable {
    public AmbientInfo ambient;

    public string GetHoverText() {
        return "Ir para " + ambient.ambientName;
    }
    
    public void HandleInteract() {
        GameManager.instance.navigation.GoTo(ambient);
    }

}
