using UnityEngine;
using UnityEngine.Events;

public class GenericUseCollectable : MonoBehaviour, IUseCollectable {
    public Collectable collectable;
    public bool consumeOnUse = true;
    public UnityEvent onCollectableUsed;


    private void Start()
    {
        DestroyIfInInventory();
    }

    public void HandleCollectable(Collectable collectableHover) {
        print("Usando " + collectableHover.GetName());
        if (consumeOnUse) {
            GameManager.instance.player.inventory.RemoveCollectable(collectable);
        }

        onCollectableUsed?.Invoke();
    }

    public string GetHoverText() {
        return "Usar " + GameManager.instance.player.collectableHeld.GetName();
    }

    public bool CanBeFound() {
        return collectable == GameManager.instance.player.collectableHeld;
    }

    /// <summary>
    /// If linked collectable is already in inventory, destroy collectable.
    /// </summary>
    private void DestroyIfInInventory()
    {
        if (InventoryManager.Instance.InventoryContainsCollectable(collectable))
        {
            Destroy(gameObject);
        }
    }
}
