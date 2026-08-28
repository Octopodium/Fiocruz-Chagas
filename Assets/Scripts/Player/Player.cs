using System;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Works like a sub GameManager, for things related to the player (can store references)
/// </summary>
public class Player : MonoBehaviour {
    // References
    public InventoryManager inventory;


    // Internal
    [HideInInspector] public IInteractable currentInteractable { get; private set; }
    [HideInInspector] public IUseCollectable currentUseCollectable { get; private set; }
    public Collectable collectableHeld;

    public Action<string> onHoverTextChange;
    public Action<Collectable> onCollectableHeldChanged;

    void Awake() {
        UIManager.instance.inventoryDeck.OnHeldCardChanged += HandleHeldCardChanged;
    }


    void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            HandleMouseClicked();
        }
    }

    void FixedUpdate() {
        CheckUnderMouse();
    }


    /// <summary>
    /// Called internally when a mouse clicks.
    /// </summary>
    void HandleMouseClicked() {
        if (currentInteractable != null) {
            currentInteractable.HandleInteract();
        }
    }

    /// <summary>
    /// Called internally when a card is being released.
    /// </summary>
    void HandleHeldCardReleaded() {
        if (currentUseCollectable != null && collectableHeld != null) {
            currentUseCollectable.HandleCollectable(collectableHeld);
        }
    }


    #region Inventory

    /// <summary>
    /// Handles InventoryDeckDisplay's OnHeldCardChanged.
    /// Set collectableHeld as the current CollectableCard being held by the player. If none, then null.
    /// </summary>
    /// <param name="nameId">The current held Collectable nameId, or an empty string if none</param>
    void HandleHeldCardChanged(string nameId) {
        Collectable collectable = nameId == "" ? null : inventory.GetCollectableById(nameId);
        if (collectableHeld == collectable)
            return;
        
        if (collectable == null)
            HandleHeldCardReleaded();

        
        collectableHeld = collectable;
        onCollectableHeldChanged?.Invoke(collectableHeld);
    }

    #endregion


    #region Interact

    /// <summary>
    /// This function is called every FixedUpdate to check for Interactables under the current mouse position.
    /// Calls SetCurrentInteractable and SetCurrentUseColletable with the value if found or null if didn't.
    /// </summary>
    void CheckUnderMouse() {
        GameObject under = GameManager.instance.cam.CheckUnderMouse(out IUnderMouse underMouseInterface);

        IInteractable interactable = null;
        IUseCollectable useCollectable = null;

        bool isUsingCard = collectableHeld != null;

        if (underMouseInterface != null) {
            if ((underMouseInterface is IInteractable) && !isUsingCard) {
                interactable = (IInteractable) underMouseInterface;
            } else if ((underMouseInterface is IUseCollectable) && isUsingCard) {
                useCollectable = (IUseCollectable) underMouseInterface;
            }
        }

        SetCurrentInteractable(interactable, !isUsingCard);
        SetCurrentUseColletable(useCollectable, isUsingCard); 

    }

    /// <summary>
    /// Sets the current hovered interactable, which will be interacted with when the players performs a click.
    /// Also triggers the event 'onHoverTextChange' with the Interactable's GetHoverText result, or with a empty string if null. 
    /// </summary>
    /// <param name="interactable">The interactable being set. Won't do a thing if equals to currentInteractable.</param>
    /// <param name="updateHoverText">Defines if 'onHoverTextChange' should be called with the Interactable's GetHoverText result. True by default.</param>
    void SetCurrentInteractable(IInteractable interactable, bool updateHoverText = true) {
        if (interactable == currentInteractable)
            return;
        
        currentInteractable = interactable;

        string hoverText = currentInteractable != null ? currentInteractable.GetHoverText() : "";
        if (updateHoverText) onHoverTextChange?.Invoke(hoverText);
    }

    /// <summary>
    /// Sets the current hovered UseCollectable, which will be used with when the players let's go of the click.
    /// Also triggers the event 'onHoverTextChange' with the Interactable's GetHoverText result, or with a empty string if null. 
    /// </summary>
    /// <param name="useCollectable">The UseCollectable being set. Won't do a thing if equals to currentUseCollectable.</param>
    /// <param name="updateHoverText">Defines if 'onHoverTextChange' should be called with the UseCollectable's GetHoverText result. True by default.</param>
    void SetCurrentUseColletable(IUseCollectable useCollectable, bool updateHoverText = true) {
        if (useCollectable == currentUseCollectable)
            return;
        
        currentUseCollectable = useCollectable;

        string hoverText = currentUseCollectable != null ? currentUseCollectable.GetHoverText() : "";
        if (updateHoverText) onHoverTextChange?.Invoke(hoverText);
    }

    #endregion
}
