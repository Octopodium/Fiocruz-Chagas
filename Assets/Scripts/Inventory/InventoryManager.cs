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

    private Collectable[] LoadCollectableList(){
        Collectable[] collectables = Resources.LoadAll<Collectable>("Collectables");
        return collectables;
    }

    private void InitializeLookupTable(){
        foreach(var collectable in allCollectables){
            lookupTable.Add(collectable.GetName(), collectable);
        }
    }

    public void AddCollectable(Collectable collectable){
        inventory.Add(collectable);
        OnAddToInventory?.Invoke(collectable);
        Debug.Log($"Added {collectable.GetName()} to inventory. \nCurrent inventory size : {inventory.Count}");
    }

    public void RemoveCollectable(Collectable collectable){
        if(inventory.Remove(collectable)){
            Debug.Log($"Removed {collectable.GetName()} from inventory succesfully. \nCurrent inventory size : {inventory.Count}");
            OnRemoveFromInventory?.Invoke(collectable);
        }
        else{
            Debug.Log($"{collectable.GetName()} wasn't found within inventory and could not be removed. \nCurrent inventory size : {inventory.Count}");
        }
    }

    public void OpenCollectableNote(string nameID){
        Collectable collectable = lookupTable[nameID];
        Debug.Log($"Opening notes on {collectable.GetName()}.");
        cardViewManager.SetUpNote(collectable);
    }

    public Collectable[] GetInventory(){
        return inventory.ToArray();
    }

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
