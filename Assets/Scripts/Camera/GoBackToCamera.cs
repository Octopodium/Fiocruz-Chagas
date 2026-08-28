using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Interactable used to go back in the cameraStack.
/// </summary>
public class GoBackToCamera : MonoBehaviour, IInteractable {

    public string GetHoverText() {
        return "Voltar";
    }

    public bool CanBeFound() {
        return true;
    }


    public void HandleInteract() {
        GameManager.instance.cam.GoBack();
    }
}
