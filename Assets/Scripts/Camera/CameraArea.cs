using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Controls a camera area, that is basically defined by everything you can see on this camera's area. Not necessary on single camera scenes.
/// </summary>
public class CameraArea : MonoBehaviour {
    public CinemachineCamera cam;
    public string areaName;

    [Tooltip("Objects that can only be active if this is the current area. Normally means triggers to leave the area.")]
    public GameObject[] onlyEnableOnArea;

    [Tooltip("All areas that can be considered child of this area, like a wall in the current area, or a iten to zoom closer")]
    public CameraArea[] childAreas;
    public bool isOnCamera {get; private set;} = false;

    void Awake() {
        GameManager.instance.cam.onCurrentCameraAreaChange += HandleCameraChanged;
    }

    async void Start() {
        await Awaitable.EndOfFrameAsync();

        bool isCurrent = GameManager.instance.cam.currentCameraArea == this;
        isOnCamera = !isCurrent;
        SetIsOnCamera(isCurrent);
    }

    void OnDestroy() {
        GameManager.instance.cam.onCurrentCameraAreaChange -= HandleCameraChanged;
    }

    /// <summary>
    /// Checks if an area is a child of this area. If includes depth, will go to every child of every other child to find it (heavier). Prevents circular references by default.
    /// </summary>
    /// <param name="cam">The camera area to be searched</param>
    /// <param name="includeDepth">Optional, false by default. If true, will search every child of every branch (layer by layer, spread style) until there's no more child to look for.</param>
    /// <returns></returns>
    public bool HasChildArea(CameraArea cam, bool includeDepth = false) {
        List<CameraArea> depthNextToSearch = new List<CameraArea>();
        HashSet<CameraArea> visitedDepth = new HashSet<CameraArea>();

        foreach (CameraArea area in childAreas) {
            if (cam == area) return true;
            if (includeDepth) {
                depthNextToSearch.AddRange(area.childAreas);
                visitedDepth.Add(area);
            }
        }

        while (depthNextToSearch.Count > 0) {
            List<CameraArea> currentlySearching = new List<CameraArea>(depthNextToSearch);
            depthNextToSearch.Clear();

            foreach (CameraArea area in currentlySearching) {
                if (visitedDepth.Contains(area) || area == null) continue;
                if (cam == area) return true;
                depthNextToSearch.AddRange(area.childAreas);
                visitedDepth.Add(area);
            }

            currentlySearching.Clear();
        }

        return false;
    }

    void HandleCameraChanged(CameraArea area) {
        SetIsOnCamera(area == this);
    }

    void SetIsOnCamera(bool is_it) {
        if (is_it == isOnCamera) return;

        isOnCamera = is_it;

        foreach (GameObject obj in onlyEnableOnArea) {
            obj.SetActive(is_it);
        }
    }

    void OnDrawGizmosSelected() {
        if (childAreas == null) return;

        Gizmos.color = Color.yellow;
        foreach (CameraArea area in childAreas) {
            if (area == null) continue;
            Gizmos.DrawLine(transform.position, area.transform.position);
        }
    }
}
