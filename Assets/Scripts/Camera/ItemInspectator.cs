using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ItemInspectator : MonoBehaviour {
    [SerializeField] GameObject inspectorLeaveTrigger;
    [SerializeField] GameObject inspectableHolder;
    public Inspectable currentInspectable {get; private set;}
    Sequence currentTweenSequence;

    public float rotateSpeed = 2f;
    public float positionTransitionTime = 0.5f;
    bool inspecting = false;
    
    void Start() {
        enabled = false;
        inspectorLeaveTrigger.SetActive(false);
    }

    public void Inspect(Inspectable inspectable) {
        currentInspectable = inspectable;

        previousState = GetPreviousState(inspectable.transform);
        inspectable.transform.SetParent(inspectableHolder.transform);

        if (currentTweenSequence != null)
            currentTweenSequence.Kill();
        
        currentTweenSequence = DOTween.Sequence();
        currentTweenSequence.Append(inspectable.transform.DOLocalMove(Vector3.zero, positionTransitionTime));
        currentTweenSequence.OnComplete(TweenComplete);

        inspectorLeaveTrigger.SetActive(true);

        enabled = true;
    }

    public void StopInspecting() {
        currentInspectable.transform.SetParent(previousState.parent);

        if (currentTweenSequence != null)
            currentTweenSequence.Kill();

        currentTweenSequence = DOTween.Sequence();
        currentTweenSequence.Append(currentInspectable.transform.DOLocalMove(previousState.position, positionTransitionTime));
        currentTweenSequence.Join(currentInspectable.transform.DOLocalRotateQuaternion(previousState.rotation, positionTransitionTime));
        currentTweenSequence.OnComplete(TweenComplete);

        currentInspectable.beingInspected = false;

        enabled = false;
        currentInspectable = null;
        inspectorLeaveTrigger.SetActive(false);
    }

    void TweenComplete() {
        currentTweenSequence = null;
    }


    void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame) { // Pressed left button
            if (!EventSystem.current.IsPointerOverGameObject(Pointer.current.deviceId)) // It wasn't handled by EventSystem (so not an UI element)
                CheckIfClickOnInspectable();
        } else if (Mouse.current.leftButton.wasReleasedThisFrame) {
            inspecting = false;
        }
    }

    void FixedUpdate() {
        CheckForMovement();
    }

    void CheckForMovement() {
        if (!inspecting || !Mouse.current.leftButton.isPressed || currentInspectable == null) return;
        Vector2 delta = Mouse.current.delta.ReadValue();

        currentInspectable.transform.Rotate(Vector3.down, delta.x * rotateSpeed, Space.World);
        currentInspectable.transform.Rotate(Vector3.right, delta.y * rotateSpeed, Space.World);
    }

    void CheckIfClickOnInspectable() {
        GameObject underMouse = GameManager.instance.cam.CheckUnderMouse(out IUnderMouse _);
        inspecting = underMouse != null && IsPartOfInteractable(underMouse);

        if (underMouse == inspectorLeaveTrigger) StopInspecting();
    }

    bool IsPartOfInteractable(GameObject clickedObject) {
        Transform iterator = clickedObject.transform;

        do {
            if (iterator == currentInspectable.transform)
                return true;
            
            iterator = iterator.parent;
        } while(iterator != null);

        return false;
    }



    #region Previous State
    struct PreviousState {
        public Transform parent;
        public Vector3 position;
        public Quaternion rotation;
    }

    PreviousState previousState;

    PreviousState GetPreviousState(Transform obj) {
        PreviousState previous = new PreviousState();
        previous.parent = obj.parent;
        previous.position = obj.localPosition;
        previous.rotation = obj.localRotation;
        return previous;
    }

    #endregion

}
