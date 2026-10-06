using UnityEngine;

/// <summary>
/// Used on gameObject that has at least one IUnderMouse, configures how the InteractableMarker will be displayed for this gameObject.
/// </summary>
[RequireComponent(typeof(IUnderMouse))]
public class MarkerSettings : MonoBehaviour {
    public Vector3 positionOffset = Vector3.zero;
    public float scale = 1;
}
