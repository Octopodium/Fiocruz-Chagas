using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Essa classe não está acabada, favor não utilizar por hora!
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
    public IUnderMouse lastUnderMouseInterface {get; private set;} = null;

    
    /// <summary>
    /// This function can be called to check for GameObject and IUnderMouse under the current mouse position.
    /// Called every frame by Player
    /// </summary>
    /// <param name="underMouse">Outs the IUnderMouse if it was found, if not, will be null.</param>
    /// <returns></returns>
    public GameObject CheckUnderMouse(out IUnderMouse underMouse) {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        underMouse = null;

        if (Physics.Raycast(ray, out hit)) {
            GameObject under = hit.transform.gameObject;

            if (under == lastUnderMouseGameObject) {
                underMouse = lastUnderMouseInterface;
                if (underMouse != null) underMouse = underMouse.CanBeFound() ? underMouse : null;

                return lastUnderMouseGameObject;
            }

            underMouse = under.GetComponent<IUnderMouse>();

            lastUnderMouseGameObject = under;
            lastUnderMouseInterface = underMouse;

            if (underMouse != null) underMouse = underMouse.CanBeFound() ? underMouse : null;

        } else {
            lastUnderMouseGameObject = null;
            lastUnderMouseInterface = null;
        }

        return lastUnderMouseGameObject;
    }
    
    #endregion
}
