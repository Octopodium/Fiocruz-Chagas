using System;
using UnityEngine;

/// <summary>
/// Interface that defines an object as able to be found by the camera raycast.
/// </summary>
public abstract class IUnderMouse: MonoBehaviour{
    public ConditionDescription[] conditions;
    public ActionDescription[] extraActions;

    public Action<bool> OnHover;
    public Action OnInteracted;

    /// <summary>
    /// Defines the text shown when hovering. It will only be called if CanBeFound return true.
    /// </summary>
    /// <returns>A small description of the object or the action you can perform with it.</returns>
    public abstract string GetHoverText();

    /// <summary>
    /// Defines if this object can be found by the camera rasycast. In certain cases, the object may want to stay hidden for a time.
    /// </summary>
    /// <returns>True if will be detected by the camera raycast.</returns>
    public abstract bool CanBeFound();

    /// <summary>
    /// Checks every condition set in 'conditions'.
    /// </summary>
    /// <returns>Returns true if every condition is true (or if there is no condition to be checked)</returns>
    public virtual bool CheckConditions() {
        if (conditions == null || conditions.Length == 0) return true;

        foreach (ConditionDescription condition in conditions)
            if (!condition.GetValue()) return false;
        return true;
    }

    /// <summary>
    /// Tries to run every action set in 'extraActions'. If one action throws an error, will Debug.LogError it and continue to the next one (won't stop the flow).
    /// </summary>
    public virtual void RunActions() {
        if (extraActions == null || extraActions.Length == 0) return;

        foreach (ActionDescription action in extraActions)
            try { action.RunAction(); } catch (Exception e) { Debug.LogError (e);}
    }

    void OnDrawGizmosSelected() {
        MarkerSettings settings = GetComponent<MarkerSettings>();
        Vector3 positionOffset = settings != null ? settings.positionOffset : Vector3.zero;
        positionOffset = transform.TransformVector(positionOffset);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + positionOffset, 0.5f * (settings != null ? settings.scale : 1));
    }
}
