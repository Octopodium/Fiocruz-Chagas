using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Controls the inspectator view.
/// </summary>
public class ItemInspectator : MonoBehaviour {
    // References
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

    /// <summary>
    /// Called by Inspectable. Brings the object to front and allow the user to rotate and interact with it.
    /// Will change it's rotation, position and parenting temporarily.
    /// </summary>
    /// <param name="inspectable">The Inspectable to inspect.</param>
    public void Inspect(Inspectable inspectable) {
        if (inspectable == currentInspectable) return;
        if (currentInspectable != null)
            currentInspectable.SetBeingInspected(false);

        currentInspectable = inspectable;

        currentInspectable.SetBeingInspected(true); // Makes sure to update it's internal variables if isn't
        currentInspectable.onInspectingChanged += HandleOnInspectingChanged;

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

    /// <summary>
    /// Called when currentInspectable 'beingInspected' changes value.
    /// </summary>
    /// <param name="state">currentInspectable 'beingInspected' value</param>
    void HandleOnInspectingChanged(bool state) {
        if (!state)
            StopInspecting();
    }

    /// <summary>
    /// Called when currentInspectable 'beingInspected' is set to false (normally by clicking on inspectorLeaveTrigger).
    /// Restore the currentInspectable state before being Inspected (rotation, position and parenting).
    /// </summary>
    void StopInspecting() {
        currentInspectable.transform.SetParent(previousState.parent);

        if (currentTweenSequence != null)
            currentTweenSequence.Kill();

        currentTweenSequence = DOTween.Sequence();
        currentTweenSequence.Append(currentInspectable.transform.DOLocalMove(previousState.position, positionTransitionTime));
        currentTweenSequence.Join(currentInspectable.transform.DOLocalRotateQuaternion(previousState.rotation, positionTransitionTime));
        currentTweenSequence.OnComplete(TweenComplete);

        currentInspectable.onInspectingChanged -= HandleOnInspectingChanged;
        currentInspectable.SetBeingInspected(false); // Makes sure to update it's internal variables if isn't

        enabled = false;
        currentInspectable = null;
        inspectorLeaveTrigger.SetActive(false);
    }

    /// <summary>
    /// Internal use only. Called everytime a tween sequence ends.
    /// </summary>
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

    /// <summary>
    /// Called every frame of FixedUpdate. If there's a currentInspectable being inspectated and the mouse is pressed, rotates the Inspectable based on mouseDelta.
    /// </summary>
    void CheckForMovement() {
        if (!inspecting || !Mouse.current.leftButton.isPressed || currentInspectable == null) return;
        Vector2 delta = Mouse.current.delta.ReadValue();

        currentInspectable.transform.Rotate(-Camera.main.transform.up, delta.x * rotateSpeed, Space.World);
        currentInspectable.transform.Rotate(Camera.main.transform.right, delta.y * rotateSpeed, Space.World);
    }

    /// <summary>
    /// Called on mouse pressed. Checks if a click happened on the Inspectable or the leave trigger.
    /// If on the Inspectable, sets 'inspecting' as true. If on the leave trigger, calls currentInspectable 'SetBeingInspected' with false.
    /// </summary>
    void CheckIfClickOnInspectable() {
        GameObject underMouse = GameManager.instance.cam.CheckUnderMouse(out IUnderMouse _);
        inspecting = underMouse != null && IsPartOfInspectable(underMouse);

        if (underMouse == inspectorLeaveTrigger)
            currentInspectable.SetBeingInspected(false);
    }

    /// <summary>
    /// Internal use only. Check if gameObject is child of the currentInspectable.
    /// This is used to consider a click in the currentInspectable's child as a click on itself.
    /// </summary>
    /// <param name="clickedObject"></param>
    /// <returns></returns>
    bool IsPartOfInspectable(GameObject clickedObject) {
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

    /// <summary>
    /// Internal use only. Creates a struct with information of the previous state of an object. Called on Inspect and it's return is used by StopInspecting.
    /// </summary>
    /// <param name="obj">The object to save it's state.</param>
    /// <returns>The struct of the object, to be used to restore it to a previous state.</returns>
    PreviousState GetPreviousState(Transform obj) {
        PreviousState previous = new PreviousState();
        previous.parent = obj.parent;
        previous.position = obj.localPosition;
        previous.rotation = obj.localRotation;
        return previous;
    }

    #endregion

}
