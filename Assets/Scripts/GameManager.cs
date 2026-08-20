using UnityEngine;

/// <summary>
/// GameManager is the main Singleton of the game and wraps all big systems in a single place of reference.
/// For it's use as a static reference, it has priority on the execution order to happen before any normal script, this way it's Awake happens before every other one.
/// </summary>
public class GameManager : MonoBehaviour {
    public static GameManager instance;
    

    public Player player;
    public AmbientNavigation navigation;

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
