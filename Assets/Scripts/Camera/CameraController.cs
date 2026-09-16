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
    Stack<CinemachineCamera> cameraStack = new Stack<CinemachineCamera>();
    public CinemachineCamera currentCamera {
        get{ return cameraStack.Count > 0 ? cameraStack.Peek() : null;}
    }

    public System.Action<CinemachineCamera> onCurrentCameraChange;


    void Awake() {
        mainCamera = Camera.main;
        cinemachine = mainCamera.GetComponent<CinemachineBrain>();
    }

    void Start() {
        if (cinemachine != null && cameraStack.Count == 0 && cinemachine.ActiveVirtualCamera is CinemachineCamera)
            cameraStack.Push((CinemachineCamera) cinemachine.ActiveVirtualCamera);
    }

    /// <summary>
    /// Sets the current camera and put it on the cameraStack.
    /// </summary>
    /// <param name="camera">The new current camera.</param>
    /// <param name="addToStack">If true, the camera will be added to the cameraStack. If false, this camera will replace the previous camera on the cameraStack.</param>
    public void GoToCamera(CinemachineCamera camera, bool addToStack = true) {
        if (cameraStack.Count > 0 && camera == cameraStack.Peek()) return;

        if (currentCamera != null) currentCamera.Priority = -1;

        if (!addToStack && cameraStack.Count > 0) {
            cameraStack.Pop(); // Remove the Peek to substitute for the current camera
        }

        cameraStack.Push(camera);
        camera.Priority = 10;

        onCurrentCameraChange?.Invoke(camera);
    }

    /// <summary>
    /// If the stack has at least 2 cameras, removes the current camera and makes the previous the new current.
    /// </summary>
    public void GoBack() {
        if (cameraStack.Count <= 1) return;
        
        CinemachineCamera previous = cameraStack.Pop();
        previous.Priority = -1;

        CinemachineCamera current = cameraStack.Peek();
        current.Priority = 10;

        onCurrentCameraChange?.Invoke(current);
    }


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
