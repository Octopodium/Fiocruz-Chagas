using UnityEngine;

/// <summary>
/// UIManager is the main Singleton related to UIs and references all big UI systems in a single place.
/// For it's use as a static reference, it has priority on the execution order to happen before any normal script, this way it's Awake happens before every other one. (After GameManager)
/// </summary>
public class UIManager : MonoBehaviour {
    public static UIManager instance;
    
    // References
    public InventoryDeckDisplay inventoryDeck;
    public FadeController fade;

    // Internal
    // ...

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
