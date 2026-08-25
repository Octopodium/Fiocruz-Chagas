using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable {
    public Collectable collectable;
    public bool destroyOnPicked = true;

    public string GetHoverText() {
        return "Pegar " + collectable.GetName();
    }
    
    public void HandleInteract() {
        GameManager.instance.player.inventory.AddCollectable(collectable);

        if (destroyOnPicked) {
            Destroy(gameObject);
        }
    }

}
