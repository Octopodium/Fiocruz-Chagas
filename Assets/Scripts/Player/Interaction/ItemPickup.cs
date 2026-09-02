using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable {
    public Collectable collectable;
    public bool destroyOnPicked = true;

    private void Start()
    {
        DestroyIfInInventory();
    }

    public string GetHoverText() {
        return "Pegar " + collectable.GetName();
    }
    
    public void HandleInteract() {
        GameManager.instance.player.inventory.AddCollectable(collectable);

        if (destroyOnPicked) {
            Destroy(gameObject);
        }
    }

    public bool CanBeFound() {
        return true;
    }

    /// <summary>
    /// If linked collectable is already in inventory, destroy collectable.
    /// </summary>
    private void DestroyIfInInventory()
    {
        if (InventoryManager.Instance.InventoryContainsCollectable(collectable))
        {
            Debug.Log("Inseto no inventário");
            Destroy(gameObject);
        }
    }

}
