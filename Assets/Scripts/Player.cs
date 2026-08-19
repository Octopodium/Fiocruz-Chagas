using System;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Works like a sub GameManager, for things related to the player (can store references)
/// </summary>
public class Player : MonoBehaviour {
    [HideInInspector] public IInteractable currentInteractable { get; private set; }
    public Action<string> onHoverTextChange;
    

    void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            HandleMouseClicked();
        }
    }

    void FixedUpdate() {
        CheckInteractables();
    }


    /// <summary>
    /// Called internally when a mouse clicks
    /// </summary>
    void HandleMouseClicked() {
        if (currentInteractable != null) {
            currentInteractable.HandleInteract();
        }
    }



    #region Interact

    /// <summary>
    /// This function is called every FixedUpdate to check for Interactables under the current mouse position.
    /// If found, calls SetCurrentHoveredInteractable with the Interactable.
    /// </summary>
    void CheckInteractables() {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        IInteractable interactable = null;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.transform.gameObject.TryGetComponent<IInteractable>(out interactable)) {
                SetCurrentInteractable(interactable);
            }
        }

        SetCurrentInteractable(interactable);
    }

    /// <summary>
    /// Sets the current hovered interactable, which will be interacted with when the players performs a click.
    /// Also calls 
    /// </summary>
    /// <param name="interactable"></param>
    void SetCurrentInteractable(IInteractable interactable) {
        if (interactable == currentInteractable)
            return;
        
        currentInteractable = interactable;

        string hoverText = currentInteractable != null ? currentInteractable.GetHoverText() : "";
        onHoverTextChange?.Invoke(hoverText);
    }

    #endregion
}
