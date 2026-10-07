using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif



public enum ArrowOptions { NoArrow, GoBack, GoToArea }

[Serializable]
public struct ArrowSettings {
    public ArrowOptions option;
    public CameraArea area;

    public void Run() {
        if (option == ArrowOptions.GoBack) GameManager.instance.cam.GoBack();
        else if (option == ArrowOptions.GoToArea) GameManager.instance.cam.GoToCamera(area);
    }
}

[Serializable]
public struct AreaArrows {
    public ArrowSettings up;
    public ArrowSettings down;
    public ArrowSettings left;
    public ArrowSettings right;
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ArrowSettings))]
public class ArrowSettingsDrawer : PropertyDrawer {

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        Foldout container = new Foldout();
        container.text = property.displayName;

        SerializedProperty optionProp = property.FindPropertyRelative("option");
        PropertyField optionField = new PropertyField(optionProp, "Option");
        optionField.TrackPropertyValue(optionProp, RebuildUI);
        container.Add(optionField);

        SerializedProperty areaProp = property.FindPropertyRelative("area");

        if (((ArrowOptions) optionProp.enumValueIndex) == ArrowOptions.GoToArea) {
            PropertyField areaField = new PropertyField(areaProp, "Area");
            container.Add(areaField);
        } else {
            areaProp.objectReferenceValue = null;
        }


        return container;
    }

    void RebuildUI(SerializedProperty trackedProperty) {
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }

}

#endif