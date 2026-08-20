using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AmbientNavigation : MonoBehaviour {
    public AmbientInfo currentAmbient;

    Dictionary<AmbientInfo, AsyncOperation> preloadedAmbients = new Dictionary<AmbientInfo, AsyncOperation>();
    Scene? currentScene;
    AsyncOperation loadingScene;
    AmbientInfo loadingAmbient;

    // In case a scene is unloading, stops the preloading process (because preload will get ) and saves preloads in buffer
    bool canPreloadScenes = true;
    List<AmbientInfo> preloadBuffer = new List<AmbientInfo>();



    void Awake() {
        SceneManager.activeSceneChanged += HandleSceneChanged;
    }

    /// <summary>
    /// Called by TravelToAmbient on Start to preload the Ambient's scene.
    /// </summary>
    /// <param name="info">AmbientInfo with the scene to preload</param>
    public void PreloadAmbient(AmbientInfo info) {
        if (preloadedAmbients.ContainsKey(info)) {
            return;
        }

        if (!canPreloadScenes && !preloadBuffer.Contains(info)) {
            preloadBuffer.Add(info);
            return;
        }

        AsyncOperation preloading = SceneManager.LoadSceneAsync(info.sceneName, LoadSceneMode.Additive);
        preloading.allowSceneActivation = false;

        preloadedAmbients.Add(info, preloading);
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
        
        loadingAmbient = info;

        if (preloadedAmbients.ContainsKey(info)) {
            loadingScene = preloadedAmbients[info];
            preloadedAmbients.Remove(info);
            loadingScene.allowSceneActivation = true;
        } else {
            loadingScene = SceneManager.LoadSceneAsync(info.sceneName, LoadSceneMode.Additive);   
        }

        canPreloadScenes = false;
        currentAmbient = info;

        yield return new WaitUntil(() => loadingScene.isDone);

        loadingAmbient = null;
        loadingScene = null;

        yield return UnloadSceneCoroutine(lastScene);

        canPreloadScenes = true;
        if (preloadBuffer.Count > 0) {
            foreach (AmbientInfo ambient in preloadBuffer) {
                PreloadAmbient(ambient);
            }

            preloadBuffer.Clear();
        }

        
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
