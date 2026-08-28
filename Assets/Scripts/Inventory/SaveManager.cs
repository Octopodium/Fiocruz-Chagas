using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour{
    private string savePath = "PlayerSaveData.json";

    [Serializable]
    public class PlayerData{
        public string[] inventory;
        public string playerName;
        public string playerLocation;
    }

    private void Awake(){
        savePath = Path.Combine(Application.persistentDataPath, savePath);
        Debug.Log($"Save file path : {savePath}");
    }

    /// <summary>
    /// Based on the player inventory contained in the InventoryManager, creates an array with all of the inventory's collectables nameIDs.
    /// </summary>
    /// <returns></returns>
    private string[] GetInventory(){
        Collectable[] inventory = InventoryManager.Instance.GetInventory();
        string[] inventoryID = new string[inventory.Length];
        for(int i = 0; i < inventory.Length; i++){
            inventoryID[i] = inventory[i].GetName();
        }
        return inventoryID;
    }

    /// <summary>
    /// Gets the current player scene from the SceneManager and return its name.
    /// </summary>
    /// <returns></returns>
    private string GetPlayerLocation(){
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Creates a new PlayerData and saves all relevant data into it. Then, writes this data into a Json file in the savePath file location.
    /// </summary>
    public void SaveData(){
        Debug.Log("Saving player data...");
        PlayerData playerData = new PlayerData{
            inventory = GetInventory(),
            playerName = "Rooty Tooty Fresh'n Fruity",
            playerLocation = GetPlayerLocation()
        };
        string jsonString = JsonUtility.ToJson(playerData, false);
        File.WriteAllText(savePath, jsonString);
        Debug.Log("Save complete!");
    }

    /// <summary>
    /// reads the content from the save file Json in the savePath location, then converts the content into a PlayerData object and returns it. 
    /// </summary>
    /// <returns></returns>
    public void LoadPlayerData()
    {
        string content = File.ReadAllText(savePath);
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(content);
        InventoryManager.Instance.LoadInventory(playerData.inventory);
        Debug.Log($"Player location : {playerData.playerLocation}");
        Debug.Log($"Player name : {playerData.playerName}");
    }

}
