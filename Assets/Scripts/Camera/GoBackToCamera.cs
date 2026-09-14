using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Interactable used to go back in the cameraStack.
/// </summary>
public class GoBackToCamera : IInteractable {

    public override string GetHoverText() {
        return "Voltar";
    }

    public override bool CanBeFound() {
        return true;
    }


    public override void HandleInteract() {
        GameManager.instance.cam.GoBack();
    }
}
