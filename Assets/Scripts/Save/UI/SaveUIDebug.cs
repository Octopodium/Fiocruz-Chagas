using UnityEngine;

public class SaveUIDebug : MonoBehaviour {
    public void Save() => GameManager.instance.saveManager.SaveData();
    public void Load() => GameManager.instance.saveManager.LoadPlayerData();
    public void Reset() => GameManager.instance.saveManager.ResetPlayerData();
}
