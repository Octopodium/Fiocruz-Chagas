using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Interactable that triggers a dialogue when interacted.
/// </summary>
public class OpenDialogue : IInteractable
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string NPCName = "Dona Neuza"; //temp
    [SerializeField] private string startNode = "TestAnaScript"; //temp
    
    public override string GetHoverText() 
    {
        return "Conversar com " + NPCName;
    }
    
    public override void HandleInteract() 
    {
        dialogueRunner.StartDialogue(startNode); 
    }

    public override bool CanBeFound() 
    {
        return true;
    }
}