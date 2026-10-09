using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the navigation between ambients. Use GoTo or GoToCoroutine to switch the current ambient.
/// </summary>
public class AmbientNavigation : MonoBehaviour, ISaveable {
    public AmbientInfo currentAmbient {get; private set;}

    Scene? currentScene;
    AsyncOperation loadingScene;


    public System.Action<AmbientInfo> onBeforeChangingAmbient, onAfterChangingAmbient;
    public System.Action<float> onAmbientLoadingProgress;


    void Awake() {
        SceneManager.activeSceneChanged += HandleSceneChanged;

        if (currentAmbient == null) currentAmbient = GetCurrentSceneAmbient();

        GameManager.instance.saveManager.AddSaveable(this);
    }

    void OnDestroy(){
        SceneManager.activeSceneChanged -= HandleSceneChanged;
        
        if (GameManager.exists)
            GameManager.instance?.saveManager.RemoveSaveable(this);
    }


    /// <summary>
    /// Changes current ambient and unloads previous one. This function only starts the coroutine GoToCoroutine. For more control over when it finishes, call the coroutine directly.
    /// </summary>
    /// <param name="info">AmbientInfo of the ambient to change to</param>
    /// <param name="fadeOptions">Optional parameter. Defines the fading configuration when going to another scene, by default will fade to black, load the scene and then fade from black.</param>
    public void GoTo(AmbientInfo info, FadeController.FadeOptions fadeOptions = FadeController.FadeOptions.FadeInOut) {
        StartCoroutine(GoToCoroutine(info, fadeOptions));
    }

    public void GoTo(AmbientInfo info) {
        StartCoroutine(GoToCoroutine(info, FadeController.FadeOptions.FadeInOut));
    }

    /// <summary>
    /// Changes current ambient and unloads previous one.
    /// </summary>
    /// <param name="info">AmbientInfo of the ambient to change to</param>
    /// <param name="fadeOptions">Optional parameter. Defines the fading configuration when going to another scene, by default will fade to black, load the scene and then fade from black.</param>
    /// <returns>Returns an Coroutine that will end after the ambient is loaded and the previous one unloaded</returns>
    public IEnumerator GoToCoroutine(AmbientInfo info, FadeController.FadeOptions fadeOptions = FadeController.FadeOptions.FadeInOut) {
        if (loadingScene != null) yield break;
        bool isSameAmbient = currentAmbient == info;
        
        Scene lastScene = currentScene.Value;
        if (!isSameAmbient) currentScene = null;

        onBeforeChangingAmbient?.Invoke(info);

        onAmbientLoadingProgress?.Invoke(0f);

        if (fadeOptions == FadeController.FadeOptions.FadeInOut || fadeOptions == FadeController.FadeOptions.FadeInOnly)
            yield return UIManager.instance.fade.FadeToBlackCoroutine();
        else if (fadeOptions == FadeController.FadeOptions.FadeOutOnly)
            UIManager.instance.fade.SetOnBlack();
        

        if (!isSameAmbient) {
            currentAmbient = info;
            loadingScene = SceneManager.LoadSceneAsync(info.sceneName, LoadSceneMode.Additive);   

            while (!loadingScene.isDone) { 
                onAmbientLoadingProgress?.Invoke(loadingScene.progress);
                yield return null;
            }
        } else {
            yield return null;
        }


        onAmbientLoadingProgress?.Invoke(1f);
        loadingScene = null;

        if (!isSameAmbient)
            yield return UnloadSceneCoroutine(lastScene);

        onAfterChangingAmbient?.Invoke(info);


        if (fadeOptions == FadeController.FadeOptions.FadeInOut || fadeOptions == FadeController.FadeOptions.FadeOutOnly)
            yield return UIManager.instance.fade.FadeFromBlackCoroutine();

        
    }

    /// <summary>
    /// Immediately changes scene to the ambient passed. Will not make any smooth transition nor call onAmbientLoadingProgress.
    /// </summary>
    /// <param name="info">AmbientInfo of the ambient to change to</param>
    public static void StartAtAmbient(AmbientInfo info) {
        SceneManager.LoadScene(info.sceneName);   
    }


    /// <summary>
    /// Internal use only. Simple coroutine that unloads a scene.
    /// </summary>
    /// <param name="scene">Scene to be unloaded</param>
    /// <returns>Returns an Coroutine that will end when scene finishes unloading</returns>
    IEnumerator UnloadSceneCoroutine(Scene scene) {
        AsyncOperation unloadingScene = SceneManager.UnloadSceneAsync(scene);
        yield return new WaitUntil(() => unloadingScene.isDone);
    }


    void HandleSceneChanged(Scene current, Scene next) {
        currentScene = next;
    }


    AmbientInfo GetCurrentSceneAmbient() {
        string currentScene = SceneManager.GetActiveScene().name;
        foreach (AmbientInfo ambient in AmbientInfo.GetAmbients()) {
            if (ambient.sceneName == currentScene) return ambient;
        }

        return null;
    }


    public PlayerData Save(PlayerData data) {
        data.playerLocation = currentAmbient != null ? currentAmbient.name : "";
        return data;
    }

    public void Load(PlayerData data) {
        string ambientName = data.playerLocation;
        AmbientInfo ambient = AmbientInfo.GetAmbient(ambientName);
        if (ambient == null) {
            UIManager.instance.fade.SetOnBlack();
            UIManager.instance.fade.FadeFromBlack();
            return;
        }
        
        if (SaveManager.comingFromMenu) GoTo(ambient, FadeController.FadeOptions.FadeOutOnly);
        else GoTo(ambient);
    }
}
