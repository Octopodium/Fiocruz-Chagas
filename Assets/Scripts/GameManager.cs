using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

/// <summary>
/// GameManager is the main Singleton of the game and wraps all big systems in a single place of reference.
/// For it's use as a static reference, it has priority on the execution order to happen before any normal script, this way it's Awake happens before every other one.
/// </summary>
public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public static bool exists => instance != null && instance.gameObject != null;
    
    // References
    public Player player;
    public FlagsSystem flags;
    public AmbientNavigation navigation;
    public CameraController cam;
    public ItemInspectator inspectator;
    public DialogueRunner dialogue;
    public SaveManager saveManager;
    public NotebookManager notebook;

    // Fields
    public string menuSceneName;

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

    public void ReturnToMenu() {
        saveManager.OnSaved += CallbackGoToMenu;
        saveManager.SaveData();
    }

    void CallbackGoToMenu(PlayerData data) {
        saveManager.OnSaved -= CallbackGoToMenu;
        StartCoroutine(CallbackGoToMenuCoroutine());
    }

    IEnumerator CallbackGoToMenuCoroutine() {
        yield return UIManager.instance.fade.FadeToBlackCoroutine();

        instance = null;
        Destroy(gameObject);

        SceneManager.LoadScene(menuSceneName);
    }
}
