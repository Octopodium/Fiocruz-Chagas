using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inspectable : MonoBehaviour, IInteractable {
    public String itemName = "item";
    public bool beingInspected = false;

    public string GetHoverText() {
        return "Inspecionar " + itemName;
    }
    
    public void HandleInteract() {
        GameManager.instance.inspectator.Inspect(this);
        beingInspected = true;
    }

    public bool CanBeFound() {
        return !beingInspected;
    }

}
