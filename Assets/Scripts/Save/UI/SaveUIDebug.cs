using UnityEngine;

public class SaveUIDebug : MonoBehaviour {
    public static string unlockSequence = "ABABAABB";
    public string currentSequence;
    public GameObject savePanel;


    public void AddToSequence(string letter) {
        if (currentSequence.Length >= unlockSequence.Length) currentSequence = currentSequence.Substring(letter.Length, unlockSequence.Length - letter.Length);
        currentSequence += letter;
        
        if (currentSequence == unlockSequence) savePanel.SetActive(true);
    }

    public void ClosePanel() {
        savePanel.SetActive(false);
        currentSequence = "";
    }


    public void Save() => GameManager.instance.saveManager.SaveData();
    public void Load() => GameManager.instance.saveManager.LoadPlayerData();
    public void Reset() => GameManager.instance.saveManager.ResetPlayerData();
}
