using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Performs raycasts with 'CheckUnderMouse' and allows to switch between virtual cameras using the camera stack with 'GoToCamera' and 'GoBack'.
/// </summary>
public class CameraController : MonoBehaviour {
    // References
    Camera mainCamera;
    CinemachineBrain cinemachine;

    // Internal
    Stack<CameraArea> cameraStack = new Stack<CameraArea>();

    public CameraArea currentCameraArea => cameraStack.Count > 0 ? cameraStack.Peek() : null;
    public CinemachineCamera currentCamera => currentCameraArea?.cam;


    public System.Action<CameraArea> onCurrentCameraAreaChange;


    void Awake() {
        mainCamera = Camera.main;
        cinemachine = mainCamera.GetComponent<CinemachineBrain>();
        GameManager.instance.navigation.onBeforeChangingAmbient += OnAmbientUnloading;
        GameManager.instance.navigation.onAfterChangingAmbient += OnAmbientLoaded;
    }

    void Start() {
        OnAmbientLoaded(null);
    }

    void OnDestroy() {
        if (GameManager.exists) {
            GameManager.instance.navigation.onBeforeChangingAmbient -= OnAmbientUnloading;
            GameManager.instance.navigation.onAfterChangingAmbient -= OnAmbientLoaded;
        }

        CinemachineCore.CameraActivatedEvent.RemoveListener(SetDefaultCameraDelayed);
    }

    void OnAmbientUnloading(AmbientInfo ambient) {
        cameraStack.Clear();
        onCurrentCameraAreaChange?.Invoke(null);
        CinemachineCore.CameraActivatedEvent.RemoveListener(SetDefaultCameraDelayed);
    }

    async void OnAmbientLoaded(AmbientInfo ambient) {
        await Awaitable.FixedUpdateAsync();

        if (!TryToSetDefaultCamera()) {
            CinemachineCore.CameraActivatedEvent.AddListener(SetDefaultCameraDelayed);
        }
    }

    void SetDefaultCameraDelayed(ICinemachineCamera.ActivationEventParams p) {
        if (TryToSetDefaultCamera())
            CinemachineCore.CameraActivatedEvent.RemoveListener(SetDefaultCameraDelayed);
    }

    bool TryToSetDefaultCamera() {
        if (cinemachine != null && cameraStack.Count == 0 && cinemachine.ActiveVirtualCamera is CinemachineCamera) {
            CameraArea area = GetCinemachinesCameraArea((CinemachineCamera) cinemachine.ActiveVirtualCamera);
            if (area != null) GoToCamera(area);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets the current camera area and put it on the cameraStack. The camera area's camera will be activated with Priority 10, and the previous one with -1.
    /// If the camArea is not a child of the current camArea, will replace it's position on the stack.
    /// </summary>
    /// <param name="camArea">The new current camera area.</param>
    public void GoToCamera(CameraArea camArea) {
        CameraArea currentArea = currentCameraArea;
        if (camArea == currentArea) return;

        if (currentCamera != null) currentCamera.Priority = -1;

        bool isChild = currentArea == null ? false : currentArea.HasChildArea(camArea);

        if (!isChild && cameraStack.Count > 0) {
            cameraStack.Pop(); // Remove the Peek to substitute for the current camera
        }

        cameraStack.Push(camArea);
        camArea.cam.Priority = 10;

        onCurrentCameraAreaChange?.Invoke(camArea);
    }

    /// <summary>
    /// If the stack has at least 2 cameraAreas, removes the current cameraArea and makes the previous the new current.
    /// </summary>
    public void GoBack() {
        if (cameraStack.Count <= 1) return;
        
        CameraArea previous = cameraStack.Pop();
        previous.cam.Priority = -1;

        CameraArea current = cameraStack.Peek();
        current.cam.Priority = 10;

        onCurrentCameraAreaChange?.Invoke(current);
    }

    /// <summary>
    /// Internal use only. Finds a cameraArea related with a cinemachineCamera. Used on Start to set current camera.
    /// </summary>
    /// <param name="cinemachineCamera"></param>
    /// <returns>The CameraArea related to the camera, or null if not found.</returns>
    CameraArea GetCinemachinesCameraArea(CinemachineCamera cinemachineCamera) {
        CameraArea[] areas = GameObject.FindObjectsByType<CameraArea>(FindObjectsSortMode.None);
        foreach (CameraArea area in areas) {
            if (area.cam == cinemachineCamera) return area;
        }

        return null;
    }

    public bool IsGameObjectCurrentArea(GameObject area) => currentCameraArea?.gameObject == area;


    #region Check Under Mouse
    /// <summary>
    /// Cache for optimatization. Used by 'CheckUnderMouse()' to store the last gameObject under the mouse.
    /// </summary>
    public GameObject lastUnderMouseGameObject {get; private set;} = null;
    public IUnderMouse[] lastUnderMouses {get; private set;} = null;

    
    /// <summary>
    /// This function can be called to check for GameObject and IUnderMouse under the current mouse position.
    /// Called every frame by Player
    /// </summary>
    /// <param name="underMouse">Outs the IUnderMouse if it was found, if not, will be null.</param>
    /// <param name="specificType">Optional parameter. Defines a search for an specific type (ex: IInteractable or IUseCollectable).</param>
    /// <returns></returns>
    public GameObject CheckUnderMouse(out IUnderMouse underMouse, Type specificType = null) {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        underMouse = null;

        if (Physics.Raycast(ray, out hit)) {
            GameObject under = hit.transform.gameObject;

            if (under == lastUnderMouseGameObject) {
                underMouse = GetSingleValidUnderMouse(lastUnderMouses, specificType);
                return lastUnderMouseGameObject;
            }

            lastUnderMouseGameObject = under;
            lastUnderMouses = under.GetComponents<IUnderMouse>();

            underMouse = GetSingleValidUnderMouse(lastUnderMouses, specificType);

        } else {
            lastUnderMouseGameObject = null;
            lastUnderMouses = null;
        }

        return lastUnderMouseGameObject;
    }

    IUnderMouse GetSingleValidUnderMouse(IUnderMouse[] options, Type limitByType = null) {
        if (options == null) return null;
        if (limitByType == null) limitByType = typeof(IUnderMouse);

        foreach (IUnderMouse option in options) {
            if (!limitByType.IsInstanceOfType(option)) continue;
            if (option.CanBeFound() && option.CheckConditions()) return option;
        }

        return null;
    }
    
    #endregion
}
