using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the navigation between ambients. Use GoTo or GoToCoroutine to switch the current ambient.
/// </summary>
public class AmbientNavigation : MonoBehaviour {
    public AmbientInfo currentAmbient;

    Scene? currentScene;
    AsyncOperation loadingScene;

    public System.Action<float> onAmbientLoadingProgress;



    void Awake() {
        SceneManager.activeSceneChanged += HandleSceneChanged;
    }


    /// <summary>
    /// Changes current ambient and unloads previous one. This function only starts the coroutine GoToCoroutine. For more control over when it finishes, call the coroutine directly.
    /// </summary>
    /// <param name="info">AmbientInfo of the ambient to change to</param>
    public void GoTo(AmbientInfo info) {
        StartCoroutine(GoToCoroutine(info));
    }

    /// <summary>
    /// Changes current ambient and unloads previous one.
    /// </summary>
    /// <param name="info">AmbientInfo of the ambient to change to</param>
    /// <returns>Returns an Coroutine that will end after the ambient is loaded and the previous one unloaded</returns>
    public IEnumerator GoToCoroutine(AmbientInfo info) {
        if (loadingScene != null) yield break;
        
        Scene lastScene = currentScene.Value;
        currentScene = null;

        onAmbientLoadingProgress?.Invoke(0f);
        yield return UIManager.instance.fade.FadeToBlackCoroutine();
        
        currentAmbient = info;
        loadingScene = SceneManager.LoadSceneAsync(info.sceneName, LoadSceneMode.Additive);   

        while (!loadingScene.isDone) { 
            onAmbientLoadingProgress?.Invoke(loadingScene.progress);
            yield return null;
        }

        onAmbientLoadingProgress?.Invoke(1f);
        loadingScene = null;

        yield return UnloadSceneCoroutine(lastScene);

        yield return UIManager.instance.fade.FadeFromBlackCoroutine();

        
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
}
