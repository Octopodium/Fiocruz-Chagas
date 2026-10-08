using UnityEngine;

/// <summary>
/// Works like an IUnderMouse but tries to run on Start and on Start only.
/// </summary>
public class SceneStartEvent : MonoBehaviour {
    public ConditionDescription[] conditions;
    public ActionDescription[] extraActions;

    void Start() => Try();

    public void Try() {
        if (ConditionDescription.CheckAll(conditions))
            ActionDescription.RunAll(extraActions);
    }
}