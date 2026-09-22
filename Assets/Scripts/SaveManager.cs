using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour{
    private string savePath = "PlayerSaveData.json";
    public static SaveManager Instance;
    private PlayerData playerData;
    private bool loadedData = false;

    public Action<PlayerData> OnSaved;
    public Action<PlayerData> OnLoaded;
    List<ISaveable> saveables = new List<ISaveable>();


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
        // LoadPlayerData();
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

    public void AddSaveable(ISaveable saveable) {
        if (saveables.Contains(saveable)) return;
        saveables.Add(saveable);
    }

    public void RemoveSaveable(ISaveable saveable) {
        if (!saveables.Contains(saveable)) return;
        saveables.Remove(saveable);
    }


    /// <summary>
    /// Creates a new PlayerData and saves all relevant data into it. Then, writes this data into a Json file in the savePath file location.
    /// </summary>
    public void SaveData(){
        Debug.Log("Saving player data...");
        playerData = new PlayerData {
            playerName = "Rooty Tooty Fresh'n Fruity"
        };

        foreach (ISaveable saveable in saveables) {
            if (saveable == null) continue;
            PlayerData dataReturned = saveable.Save(playerData);
            if (dataReturned == null) continue;
            playerData = dataReturned;
        }


        Debug.Log(playerData.playerName + ":" + playerData.playerLocation);
        string jsonString = JsonUtility.ToJson(playerData, true);
        Debug.Log(jsonString);
        File.WriteAllText(savePath, jsonString);
        Debug.Log(File.ReadAllText(savePath));
        Debug.Log("Save complete!");



        OnSaved?.Invoke(playerData);
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

            foreach (ISaveable saveable in saveables) {
                if (saveable == null) continue;
                saveable.Load(playerData);
            }

            OnLoaded?.Invoke(playerData);
        }
        else
        {
            Debug.Log($"<color=yellow>No save data found</color>");
            loadedData = true;
        }
    }


    /// <summary>
    /// Creates a new blank PlayerData and writes this data into a Json file in the savePath file location.
    /// Then, proceeds to load said PlayerData.
    /// </summary>
    public void ResetPlayerData()
    {
        playerData = new PlayerData {
            playerName = "Rooty Tooty Fresh'n Fruity"
        };

        string jsonString = JsonUtility.ToJson(playerData, true);
        Debug.Log(jsonString);
        File.WriteAllText(savePath, jsonString);
        Debug.Log(File.ReadAllText(savePath));
        Debug.Log("Save reseted!");

        LoadPlayerData();

        Debug.Log("Full reset complete!");
    }

}
