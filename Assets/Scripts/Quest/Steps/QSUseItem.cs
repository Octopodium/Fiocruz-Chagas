using System;
using UnityEngine;

/// <summary>
/// Quest Step that finishes when the item disappears of the inventory. If not on, finishes instantly.
/// </summary>
[CreateAssetMenu(fileName = "QSUseItem", menuName = "Scriptable Objects/Quest Steps/Use Item Step"), Serializable]
public class QSUseItem : QuestStep{
    public Collectable item;


    public override void Start() {
        if (!GameManager.instance.player.inventory.InventoryContainsCollectable(item)) {
            Finish();
            return;
        }

        GameManager.instance.player.inventory.OnRemoveFromInventory += InventoryChanged;
    }

    public override void Stop() {
        GameManager.instance.player.inventory.OnRemoveFromInventory -= InventoryChanged;
    }

    void InventoryChanged(Collectable collectable) {
        if (collectable == item) Finish();
    }
}
