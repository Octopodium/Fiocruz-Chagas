using UnityEngine;

public class DummyActionRunner : MonoBehaviour {
    public ConditionDescription[] conditions;
    public ActionDescription[] extraActions;

    public void DummyRun() {
        if (ConditionDescription.CheckAll(conditions))
            ActionDescription.RunAll(extraActions);
    }
}