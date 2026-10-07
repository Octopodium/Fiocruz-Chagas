using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneActivator : MonoBehaviour
{
    [SerializeField] private string sceneToActivate;
    private Scene scene;
    [SerializeField] private bool activateOnAwake = true;
    [SerializeField] private bool setupTransition = true;
    private SaturationTransition transitionController;
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
        if(setupTransition) {
            SetUpSaturation();
            StopTime();
        }
    }

    private void SetUpSaturation()
    {
        transitionController = GameObject.FindAnyObjectByType<SaturationTransition>();
        transitionController.SetSaturation(-100);
    }

    private void StopTime()
    {
        Time.timeScale = 0.0f;
    }

    public void CallTransition()
    {
        transitionController.FadeColor();
    }
}
