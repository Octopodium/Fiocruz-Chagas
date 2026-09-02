using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Used on itens that can be brought to the front of the camera and rotated.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Inspectable : MonoBehaviour, IInteractable {
    public String itemName = "item";
    public bool beingInspected {get; private set;} = false;

    public System.Action<bool> onInspectingChanged;

    List<IUnderMouse> childrenUnderMouse = new List<IUnderMouse>();

    void Awake() {
        GetComponentsInChildren<IUnderMouse>(childrenUnderMouse);
    }

    void Start() {
        SetChildrenUnderMouseState(false);
    }

    public string GetHoverText() {
        return "Inspecionar " + itemName;
    }

    public bool CanBeFound() {
        return !beingInspected;
    }

    public void HandleInteract() => Inspect();
    public void Inspect() => SetBeingInspected(true);
    public void StopInspecting() => SetBeingInspected(false);

    /// <summary>
    /// Sets if the Inspectable is being inspected.
    /// </summary>
    /// <param name="is_it"></param>
    public void SetBeingInspected(bool is_it) {
        if (is_it == beingInspected) return;

        beingInspected = is_it;

        if (beingInspected)
            GameManager.instance.inspectator.Inspect(this);

        SetChildrenUnderMouseState(beingInspected);
        onInspectingChanged?.Invoke(beingInspected);
    }

    /// <summary>
    /// Internal use only. Updates the children IUnderMouse enabled state, so they are only enabled while inspecting.
    /// </summary>
    /// <param name="active">Value to set every IUnderMouse enabled state to.</param>
    void SetChildrenUnderMouseState(bool active) {
        for (int i = childrenUnderMouse.Count - 1; i > 0; i--) {
            IUnderMouse child = childrenUnderMouse[i];
            if (child is MonoBehaviour) {
                MonoBehaviour childComponent = (MonoBehaviour) child;

                if (childComponent == null) childrenUnderMouse.Remove(child);
                else childComponent.enabled = active;
            }
        }
    }

}
