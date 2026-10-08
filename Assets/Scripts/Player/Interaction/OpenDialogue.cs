using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Interactable that triggers a dialogue when interacted.
/// </summary>
public class OpenDialogue : IInteractable {
    [SerializeField] private string NPCName = "Dona Neuza"; //temp
    [SerializeField] private string startNode = "TestAnaScript"; //temp
    [SerializeField] private bool startAutomatic = false;


    public ActionDescription[] onDialogueFinished;


    private void Start() {
        if(startAutomatic) StartDialogue();
    }

    public override string GetHoverText() {
        return "Conversar com " + NPCName;
    }
    
    public override void HandleInteract() => StartDialogue();

    public override bool CanBeFound() {
        return true;
    }

    void StartDialogue() {
        GameManager.instance.dialogue.onDialogueComplete.AddListener(HandleDialogueFinished);
        GameManager.instance.dialogue.StartDialogue(startNode);
    }

    void HandleDialogueFinished() {
        GameManager.instance.dialogue.onDialogueComplete.RemoveListener(HandleDialogueFinished);
        ActionDescription.RunAll(onDialogueFinished);
    }

    void OnDestroy() {
        if (GameManager.exists)
            GameManager.instance.dialogue.onDialogueComplete.RemoveListener(HandleDialogueFinished);
    }
}