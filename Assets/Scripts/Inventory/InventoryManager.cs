using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    [SerializeField] private List<Collectable> inventory = new List<Collectable>();
    [SerializeField] private List<Collectable> allCollectables = new List<Collectable>();
    [SerializeField] private Dictionary<string, Collectable> lookupTable = new Dictionary<string, Collectable>();
    [SerializeField] private CardViewManager cardViewManager;
    public Action<Collectable> OnAddToInventory, OnRemoveFromInventory;

    private void Awake(){
        if(Instance) Destroy(gameObject);
        Instance = this;
        
        allCollectables = new List<Collectable>(LoadCollectableList());
        InitializeLookupTable();
    }

    private void Start()
    {
        LoadInventory();
    }

    /// <summary>
    /// Loads all collectable assets from the "Collectables" folder in the Resources directory. This method is used to populate the allCollectables list with all available collectables in the game.
    /// </summary>
    /// <returns></returns>
    private Collectable[] LoadCollectableList(){
        Collectable[] collectables = Resources.LoadAll<Collectable>("Collectables");
        return collectables;
    }

    /// <summary>
    /// Initializes the lookup table by iterating through all collectables and adding them to the dictionary with their name as the key. This allows for quick access to collectables by their nameID.
    /// </summary>
    private void InitializeLookupTable(){
        foreach(var collectable in allCollectables){
            lookupTable.Add(collectable.GetName(), collectable);
        }
    }

    /// <summary>
    /// Adds a collectable to the inventory and invokes the OnAddToInventory event. This method is used to add a collectable to the player's inventory and notify any listeners that a new collectable has been added.
    /// </summary>
    /// <param name="collectable"></param>
    public void AddCollectable(Collectable collectable){
        inventory.Add(collectable);
        OnAddToInventory?.Invoke(collectable);
        Debug.Log($"Added {collectable.GetName()} to inventory. \nCurrent inventory size : {inventory.Count}");
    }

    /// <summary>
    /// Removes a collectable from the inventory and invokes the OnRemoveFromInventory event. This method is used to remove a collectable from the player's inventory and notify any listeners that a collectable has been removed.
    /// </summary>
    /// <param name="collectable"></param>
    public void RemoveCollectable(Collectable collectable){
        if(inventory.Remove(collectable)){
            Debug.Log($"Removed {collectable.GetName()} from inventory succesfully. \nCurrent inventory size : {inventory.Count}");
            OnRemoveFromInventory?.Invoke(collectable);
        }
        else{
            Debug.Log($"{collectable.GetName()} wasn't found within inventory and could not be removed. \nCurrent inventory size : {inventory.Count}");
        }
    }

    /// <summary>
    /// Removes all items from inventory and inventory deck.
    /// </summary>
    private void ClearInventory()
    {
        while(inventory.Count > 0)
        {
            RemoveCollectable(inventory[0]);
        }
        inventory.Clear();
        Debug.Log("Inventory was completely cleared.");
    }

    /// <summary>
    /// Opens the collectable note for the specified collectable by its nameID. This method retrieves the collectable from the lookup table and sets up the note view using the CardViewManager.
    /// </summary>
    /// <param name="nameID"></param>
    public void OpenCollectableNote(string nameID){
        Collectable collectable = lookupTable[nameID];
        Debug.Log($"Opening notes on {collectable.GetName()}.");
        cardViewManager.SetUpNote(collectable);
    }

    /// <summary>
    /// Returns the list of collectables in the player's inventory.
    /// </summary>
    /// <returns></returns>
    public Collectable[] GetInventory(){
        return inventory.ToArray();
    }

    /// <summary>
    /// Waits for SaveManager to finish loading player data, then proceeds to either convert the inventory data loaded, or initiate inventory without any data.
    /// </summary>
    /// <param name="loadData"></param>
    public async void LoadInventory()
    {
        if (!SaveManager.Instance)
        {
            Debug.LogWarning("No SaveManager Instance was found. initializing inventory without player data.");
            return;
        }
        PlayerData playerData;
        while(!SaveManager.Instance.GetPlayerData(out playerData))
        {
            Debug.Log($"<color=yellow>Awaiting for player data.</color>");
            await Awaitable.NextFrameAsync();
        }
        if(playerData != null)
        {
            string [] inventoryData = playerData.inventory;
            ClearInventory();
            foreach(var data in inventoryData)
            {
                AddCollectable(lookupTable[data]);
            }
            Debug.Log("Inventory succesfully loaded from player data.");
        }
        else
        {
            Debug.Log($"<color=yellow>No inventory data was loaded.</color>");
        }
    }

    /// <summary>
    /// Prints the names of all collectables in the player's inventory to the console for debugging purposes.
    /// </summary>
    public void DebugInventory(){
        Collectable[] collectables = GetInventory();
        foreach(Collectable collectable in collectables){
            Debug.Log(collectable.GetName());
        }
    }

    /// <summary>
    /// Used by any other system that may require to know a collectable other than by it's nameId.
    /// Alternatively, you can get the Collectable by using Resources.Load<Collectable>("Collectables/"+nameId).
    /// </summary>
    /// <param name="nameId">The Collectable's nameId. You can get a Collectable nameId by it's method GetName()</param>
    /// <returns>Returns the stored Collectable with this nameId. If not found, returns null.</returns>
    public Collectable GetCollectableById(string nameId) {
        return lookupTable.ContainsKey(nameId) ? lookupTable[nameId] : null;
    }
}
