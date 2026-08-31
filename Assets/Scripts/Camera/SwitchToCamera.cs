using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Interactable used to switch between cameras. Most cases of use is to get close to an area of the same ambient.
/// This interactable allows to "change the principal camera" by using 'addToStack' as false and being in the peek of the stack.
/// </summary>
public class SwitchToCamera : MonoBehaviour, IInteractable {

    public string areaName = "area";
    public CinemachineCamera cam;
    /// <summary>
    /// If true, the camera will be added to the cameraStack. Using the GoBack method will go to the previous camera.
    /// If false, this camera will replace the previous camera on the cameraStack. If you are not sure, leave it as true.
    /// </summary>
    public bool addToStack = true;
    bool isOnCamera = false;

    /// <summary>
    /// An array of GameObjects that will stays unactive until the camera changes to this area.
    /// When the camera leaves, it will return to be unactive.
    /// </summary>
    public GameObject[] onlyEnableWhenOnCamera;

    Collider[] colliders;

    void Awake() {
        colliders = GetComponents<Collider>();
    }

    void Start() {
        GameManager.instance.cam.onCurrentCameraChange += HandleCameraChanged;

        bool isCurrent = GameManager.instance.cam.currentCamera == cam;
        isOnCamera = !isCurrent;
        SetIsOnCamera(isCurrent);
    }
    
    public string GetHoverText() {
        return "Ver " + areaName;
    }

    public bool CanBeFound() {
        return !isOnCamera;
    }


    public void HandleInteract() {
        GameManager.instance.cam.GoToCamera(cam, addToStack);
    }

    void HandleCameraChanged(CinemachineCamera camera) {
        SetIsOnCamera(camera == cam);
    }

    void SetIsOnCamera(bool is_it) {
        if (is_it == isOnCamera) return;

        isOnCamera = is_it;

        foreach (GameObject obj in onlyEnableWhenOnCamera) {
            obj.SetActive(is_it);
        }

        foreach (Collider col in colliders) {
            col.enabled = !is_it;
        }
    }

    void OnDestroy() {
        GameManager.instance.cam.onCurrentCameraChange -= HandleCameraChanged;
    }
}
