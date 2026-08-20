using UnityEngine;
using UnityEngine.SceneManagement;

public class TravelToAmbient : MonoBehaviour, IInteractable {
    public AmbientInfo ambient;

    void Start() {
        GameManager.instance.navigation.PreloadAmbient(ambient);
    }

    public string GetHoverText() {
        return "Ir para " + ambient.ambientName;
    }
    
    public void HandleInteract() {
        GameManager.instance.navigation.GoTo(ambient);
    }

}
