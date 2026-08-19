using UnityEngine;
using UnityEngine.SceneManagement;

public class TravelToAmbient : MonoBehaviour, IInteractable {
    public AmbientInfo ambient;

    public string GetHoverText() {
        return "Ir para " + ambient.ambientName;
    }
    
    public void HandleInteract() {
        SceneManager.LoadScene(ambient.sceneName);
    }

}
