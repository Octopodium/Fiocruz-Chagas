using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneActivator : MonoBehaviour
{
    [SerializeField] private string sceneToActivate;
    private Scene scene;
    [SerializeField] private bool activateOnAwake = true;
    private void Awake()
    {
        if(activateOnAwake) SetSceneActive();
    }

    public async void SetSceneActive()
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneToActivate, LoadSceneMode.Additive);
        await loadScene;
        scene = SceneManager.GetSceneByName(sceneToActivate);
        SceneManager.SetActiveScene(scene);
    }
}
