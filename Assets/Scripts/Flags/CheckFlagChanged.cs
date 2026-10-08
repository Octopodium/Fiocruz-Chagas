using UnityEngine;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;
#endif

/// <summary>
/// Called everytime a flag changes, checks for a flag with a certain value and when it becomes said value, runs actions.
/// If it can run more than once, it will run the action every alternance of conditional result.
/// For example: will run the first time the condition is true, then the condition needs to become false before it turns true again to run the second time.
/// </summary>
public class CheckFlagChanged : MonoBehaviour {
    public string flag;
    public GenericValue value;
    public ActionDescription[] actions;

    [Tooltip("After the first time the condition becomes true, can it run the action again if the condition becomes true once more?")]
    public bool canRunAgain = false;

    bool wasEqualBefore = false;
    bool runnedOnce = false;

    void Awake() {
        GameManager.instance.flags.OnFlagChanged += HandleFlagChanged;
    }

    void OnDestroy() {
        if (!GameManager.exists) return;
        GameManager.instance.flags.OnFlagChanged -= HandleFlagChanged;
    }

    void Start() {
        Check(GameManager.instance.flags.GetFlag(flag, true));
    }

    void HandleFlagChanged(string flag, object value) {
        if (flag != this.flag) return;
        Check(value);
    }

    public void Check(object currentValue) {
        if (!canRunAgain && runnedOnce) return;

        if (Equals(currentValue, value.GetValue())) {
            if (wasEqualBefore) return;
            wasEqualBefore = true;
            runnedOnce = true;

            ActionDescription.RunAll(actions);
        } else {
            wasEqualBefore = false;
        }
    }

}




#if UNITY_EDITOR

[CustomEditor(typeof(CheckFlagChanged))]
public class Car_Inspector : Editor {
    SerializedProperty flagProp;
    SerializedProperty valueProp;
    SerializedProperty actionsProp;
    SerializedProperty runAgainProp;

    void OnEnable() {
        // Setup the SerializedProperties.
        flagProp = serializedObject.FindProperty("flag");
        valueProp = serializedObject.FindProperty("value");
        actionsProp = serializedObject.FindProperty("actions");
        runAgainProp = serializedObject.FindProperty("canRunAgain");
    }

    void RebuildUI(SerializedProperty trackedProperty) {
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }

    public override VisualElement CreateInspectorGUI() {
        VisualElement container = new VisualElement();

        VisualElement flagRow = new VisualElement();
        flagRow.style.flexDirection = FlexDirection.Row;

        Label flagLabel = new Label("Flag:");
        flagLabel.style.width = Length.Percent(30);
        flagLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
        flagRow.Add(flagLabel);


        SortedDictionary<string, object> data = new SortedDictionary<string, object>();

        foreach (FlagDescriptor descriptor in FlagsRegister.instance.availableFlags) {
            data[descriptor.name + ": " + descriptor.readableType] = descriptor.name;
        }

        VisualElement flagSelectorHolder = new VisualElement();
        flagSelectorHolder.style.width = Length.Percent(70);
        flagSelectorHolder.style.flexDirection = FlexDirection.Row;

        VisualElement flagField = CreatePopup(flagProp, data);
        flagField.style.flexGrow = 1;
        flagField.TrackPropertyValue(flagProp, RebuildUI);
        flagSelectorHolder.Add(flagField);

        Button aboutFlags = new Button(() => { Selection.activeObject=AssetDatabase.LoadMainAssetAtPath("Assets/Resources/"+FlagsRegister.mainRegisterResourcePath+".asset"); });
        aboutFlags.text = "?";
        aboutFlags.tooltip = "The displayed flag list is based on registered flags on the FlagsRegister";
        flagSelectorHolder.Add(aboutFlags);
        flagRow.Add(flagSelectorHolder);

        container.Add(flagRow);


        VisualElement valueRow = new VisualElement();
        valueRow.style.flexDirection = FlexDirection.Row;

        Label valueLabel = new Label("Value:");
        valueLabel.style.width = Length.Percent(30);
        valueLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
        valueRow.Add(valueLabel);

        PropertyField valueField = new PropertyField(valueProp);
        valueField.style.flexGrow = 1;
        valueRow.Add(valueField);
        container.Add(valueRow);


        FlagDescriptor flagDescriptor = FlagsRegister.instance.GetDescriptorByName(flagProp.stringValue);
        Type flagType = flagDescriptor != null ? FlagDescriptor.GetTypeByFlagTypes(flagDescriptor.type) : null;

        if (flagType != null) {
            GenericValue val = (GenericValue) valueProp.boxedValue;
            val.type = GenericValue.GetGenericType(flagType);
            valueProp.boxedValue = val;
            valueProp.serializedObject.ApplyModifiedProperties();
        }

        PropertyField actionsField = new PropertyField(actionsProp);
        container.Add(actionsField);

        PropertyField runAgainField = new PropertyField(runAgainProp);
        container.Add(runAgainField);

        return container;
    }


    public PopupField<string> CreatePopup(SerializedProperty property, SortedDictionary<string, object> choices) {
        int defaultIndex = Mathf.Max(0, choices.Values.ToList().FindIndex(x => object.Equals(x,property.boxedValue)));
        PopupField<string> dropdown = new PopupField<string>(
            choices: choices.Keys.ToList(),
            defaultIndex: defaultIndex
        );

        dropdown.RegisterValueChangedCallback(evt => {
            property.boxedValue = choices[evt.newValue];
            serializedObject.ApplyModifiedProperties();
        });

        dropdown.AddToClassList(PopupField<string>.alignedFieldUssClassName);

        string choice = choices.Keys.ToList()[defaultIndex];
        property.boxedValue = choices[choice];

        return dropdown;
    }
}

#endif