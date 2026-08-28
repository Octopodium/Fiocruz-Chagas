using UnityEngine;
using System;
using System.IO;

public class SaveManager : MonoBehaviour{
    private string savePath = "PlayerSaveData.json";

    [Serializable]
    public class PlayerData{
        public string[] inventory;
        public string playerName;
    }

    private void Awake(){
        savePath = Path.Combine(Application.persistentDataPath, savePath);
        Debug.Log($"Save file path : {savePath}");
    }

    private void Start(){
        SaveData();
    }

    private string[] GetInventory(){
        Collectable[] inventory = InventoryManager.Instance.GetInventory();
        string[] inventoryID = new string[inventory.Length];
        for(int i = 0; i < inventory.Length; i++){
            inventoryID[i] = inventory[i].GetName();
        }
        return inventoryID;
    }

    public void SaveData(){
        Debug.Log("Saving player data...");
        PlayerData playerData = new PlayerData{
            inventory = GetInventory(),
            playerName = "Rooty Tooty Fresh'n Fruity"
        };

        string jsonString = JsonUtility.ToJson(playerData, false);
        File.WriteAllText(savePath, jsonString);
        Debug.Log("Save complete!");
    }

}
