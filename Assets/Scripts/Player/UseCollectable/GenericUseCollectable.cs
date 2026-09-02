using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A generic UseCollectable that triggers an event when player drags and drop a specific collectable card over it.
/// </summary>
public class GenericUseCollectable : MonoBehaviour, IUseCollectable {
    public Collectable collectable;
    public bool consumeOnUse = true;
    public UnityEvent onCollectableUsed;


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
}
