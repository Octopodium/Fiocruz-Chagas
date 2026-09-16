using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A generic UseCollectable that triggers an event when player drags and drop a specific collectable card over it.
/// </summary>
public class GenericUseCollectable : IUseCollectable {
    public Collectable collectable;
    public bool consumeOnUse = true;
    public UnityEvent onCollectableUsed;


    public override void HandleCollectable(Collectable collectableHover) {
        if (consumeOnUse) {
            GameManager.instance.player.inventory.RemoveCollectable(collectable);
        }

        onCollectableUsed?.Invoke();
    }

    public override string GetHoverText() {
        return "Usar " + GameManager.instance.player.collectableHeld.GetName();
    }

    public override bool CanBeFound() {
        return collectable == GameManager.instance.player.collectableHeld;
    }


    public void CheckFlag(string flag) => GameManager.instance.flags.CheckFlag(flag);
    public void UncheckFlag(string flag) => GameManager.instance.flags.UncheckFlag(flag);
}
