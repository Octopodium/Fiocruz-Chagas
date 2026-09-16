using UnityEngine;

/// <summary>
/// UseCollectable that triggers that triggers a dialogue when player drags and drop a specific collectable card over it.
/// </summary>
public class OpenCollectableDialogue : IUseCollectable {
    public Collectable collectable;
    public bool consumeOnUse = true;


    public string NPCName = "Dona Neuza";
    public string startNode = "TestAnaScript";


    public override void HandleCollectable(Collectable collectableHover) {
        if (consumeOnUse) {
            GameManager.instance.player.inventory.RemoveCollectable(collectable);
        }

        GameManager.instance.dialogue.StartDialogue(startNode); 
    }

    public override string GetHoverText() {
        return "Conversar com " + NPCName + " sobre " + GameManager.instance.player.collectableHeld.GetName();
    }

    public override bool CanBeFound() {
        return collectable == GameManager.instance.player.collectableHeld;
    }


    
    

}
