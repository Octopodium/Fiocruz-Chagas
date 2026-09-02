using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour{
    private string savePath = "PlayerSaveData.json";
    public static SaveManager Instance;
    private PlayerData playerData;
    private bool loadedData = false;

    private void Awake(){
        if (Instance)
        {
            Destroy(gameObject);
        }
        Instance = this;

        savePath = Path.Combine(Application.persistentDataPath, savePath);
        Debug.Log($"Save file path : {savePath}");
    }

    private void Start()
    {
        LoadPlayerData();
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
    /// Returns the currently loaded player data
    /// </summary>
    /// <returns></returns>
    public bool GetPlayerData(out PlayerData data)
    {
        data = loadedData ? playerData : null;
        return loadedData;
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
        playerData = new PlayerData(
            GetInventory(),
            "Rooty Tooty Fresh'n Fruity",
            GetPlayerLocation()
        );
        Debug.Log(playerData.playerName + ":" + playerData.playerLocation);
        string jsonString = JsonUtility.ToJson(playerData, true);
        Debug.Log(jsonString);
        File.WriteAllText(savePath, jsonString);
        Debug.Log(File.ReadAllText(savePath));
        Debug.Log("Save complete!");
    }

    /// <summary>
    /// reads the content from the save file Json in the savePath location, then converts the content into a PlayerData object and returns it. 
    /// </summary>
    /// <returns></returns>
    public void LoadPlayerData()
    {
        if (File.Exists(savePath))
        {
            string content = File.ReadAllText(savePath);
            playerData = JsonUtility.FromJson<PlayerData>(content);
            Debug.Log($"Player location : {playerData.playerLocation}");
            Debug.Log($"Player name : {playerData.playerName}");
            loadedData = true;
        }
        else
        {
            Debug.Log($"<color=yellow>No save data found</color>");
            loadedData = true;
        }
    }

}

[Serializable]
public class PlayerData{
    [SerializeField] private string[] _inventory;
    public string[] inventory {
        get{
            return _inventory;
        } 
        private set
        {
            _inventory = value;
        }
    }
    [SerializeField] private string _playerName;
    public string playerName
    {
        get
        {
            return _playerName;   
        } 
        private set
        {
            _playerName = value;
        }
    }
    [SerializeField] private string _playerLocation;
    public string playerLocation
    {
        get
        {
            return _playerLocation;
        } 
        private set
        {
            _playerLocation = value;
        }
    }
    public PlayerData(string[] inventory, string playerName, string playerLocation)
    {
        this.inventory = inventory;
        this.playerName = playerName;
        this.playerLocation = playerLocation;
    }
}
