using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Interactable used to switch between cameras. Most cases of use is to get close to an area of the same ambient.
/// This interactable allows to "change the principal camera" by using 'addToStack' as false and being in the peek of the stack.
/// </summary>
public class SwitchToCamera : IInteractable {
    public CameraArea area;


    Collider[] colliders;

    void Awake() {
        colliders = GetComponents<Collider>();
        GameManager.instance.cam.onCurrentCameraAreaChange += HandleCameraChanged;
    }

    void OnDestroy() {
        if (GameManager.exists)
            GameManager.instance.cam.onCurrentCameraAreaChange -= HandleCameraChanged;
    }

    async void Start() {
        await Awaitable.EndOfFrameAsync();
        bool isCurrent = GameManager.instance.cam.currentCameraArea == area;
        SetIsOnCamera(isCurrent);
    }
    
    public override string GetHoverText() {
        return "Ver " + area.areaName;
    }

    public override bool CanBeFound() {
        return (area != null ? !area.isOnCamera : false) && enabled;
    }


    public override void HandleInteract() {
        GameManager.instance.cam.GoToCamera(area);
    }

    void HandleCameraChanged(CameraArea camArea) {
        SetIsOnCamera(camArea == area || area.HasChildArea(camArea));
    }

    void SetIsOnCamera(bool is_it) {
        foreach (Collider col in colliders) {
            col.enabled = !is_it;
        }

        enabled = !is_it;
    }

    void OnEnable() => RefreshMarker();
    void OnDisable() => RefreshMarker();

    void RefreshMarker() {
        GameManager.instance.player.indicators.RefreshIndicators();
    }


    void OnDrawGizmosSelected() {
        if (area == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, area.transform.position);
    }
}
