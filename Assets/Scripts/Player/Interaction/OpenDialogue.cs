using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Interactable that triggers a dialogue when interacted.
/// </summary>
public class OpenDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string NPCName = "Dona Neuza"; //temp
    [SerializeField] private string startNode = "TestAnaScript"; //temp
    
    public string GetHoverText() 
    {
        return "Conversar com " + NPCName;
    }
    
    public void HandleInteract() 
    {
        dialogueRunner.StartDialogue(startNode); 
    }

    public bool CanBeFound() 
    {
        return true;
    }
}