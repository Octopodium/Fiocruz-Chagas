using UnityEngine;
using UnityEngine.Events;

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

    public string GetHoverText(Collectable collectableHover) {
        return "Usar " + collectableHover.GetName();
    }

    public bool CanUse(Collectable collectableHover) {
        return collectable == collectableHover;
    }
}
