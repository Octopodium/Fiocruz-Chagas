using UnityEngine;
using Yarn.Unity;
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
        //if (dialogueRunner.IsDialogueRunning) Debug.Log("tarodano"); return;

        //Debug.Log("ta tentatno roda");
        dialogueRunner.StartDialogue(startNode); 
    }

    public bool CanBeFound() 
    {
        return true;
    }
}