using System;
using UnityEngine;

/// <summary>
/// Quest Step that finishes when the item appears on the inventory. If already on, finishes instantly.
/// </summary>
[CreateAssetMenu(fileName = "QSGetItem", menuName = "Scriptable Objects/Quest Steps/Get Item Step")]
public class QSGetItem : QuestStep{
    public Collectable item;


    public override void Start() {
        if (GameManager.instance.player.inventory.InventoryContainsCollectable(item)) {
            Finish();
            return;
        }

        GameManager.instance.player.inventory.OnAddToInventory += InventoryChanged;
    }

    public override void Stop() {
        GameManager.instance.player.inventory.OnAddToInventory -= InventoryChanged;
    }

    void InventoryChanged(Collectable collectable) {
        if (collectable == item) Finish();
    }
}
