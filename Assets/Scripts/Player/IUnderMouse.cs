using UnityEngine;

/// <summary>
/// Interface that defines an object as able to be found by the camera raycast.
/// </summary>
public abstract class IUnderMouse: MonoBehaviour{
    public ConditionDescription[] conditions;

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
    public bool CheckConditions() {
        foreach (ConditionDescription condition in conditions)
            if (!condition.GetValue()) return false;
        return true;
    }
}
