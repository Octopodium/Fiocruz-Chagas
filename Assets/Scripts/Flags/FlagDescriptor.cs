using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif


public enum FlagType { BOOL, INT, FLOAT, STRING }


/// <summary>
/// Descriptor of a register flag. Used to relate a flag with it's type. Referenced on FlagsRegister.
/// </summary>
[Serializable]
public class FlagDescriptor {
    public string name;
    public FlagType type;
    public string readableType => GetReadableType(type);


    /// <summary>
    /// Get the Type related to the FlagType enum.
    /// </summary>
    /// <param name="flagType">FlagType enum to check</param>
    /// <returns>FlagType's related Type. Null if not valid parameter</returns>
    public static Type GetTypeByFlagTypes(FlagType flagType) {
        switch (flagType) {
            case FlagType.BOOL:
                return typeof(bool);
            case FlagType.INT:
                return typeof(int);
            case FlagType.FLOAT:
                return typeof(float);
            case FlagType.STRING:
                return typeof(string);
        }

        return null;
    }

    /// <summary>
    /// Get the FlagType enum that represents a Type
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <returns>The FlagType that represents the Type. Null if the Type cannot be represented</returns>
    public static FlagType? GetFlagTypeByType(Type type) {
        if (type == typeof(bool)) return FlagType.BOOL;
        if (type == typeof(int)) return FlagType.INT;
        if (type == typeof(float)) return FlagType.FLOAT;
        if (type == typeof(string)) return FlagType.STRING;
        return null;
    }

    /// <summary>
    /// Get the readable type name from a FlagType enum
    /// </summary>
    /// <param name="flagType">The FlagType enum</param>
    /// <returns>A readable type name</returns>
    public static string GetReadableType(FlagType flagType) {
        switch (flagType) {
            case FlagType.BOOL:
                return "bool";
            case FlagType.INT:
                return "int";
            case FlagType.FLOAT:
                return "float";
            case FlagType.STRING:
                return "string";
        }

        return null;
    }
}



#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(FlagDescriptor))]
public class FlagDescriptorDrawer : PropertyDrawer {

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        VisualElement container = new VisualElement();
        BuildUI(container, property);

        return container;
    }

    public void BuildUI(VisualElement container, SerializedProperty property) {
        VisualElement content = new VisualElement();
        content.style.flexDirection = FlexDirection.Row;


        PropertyField nameField = new PropertyField(property.FindPropertyRelative("name"), "");
        nameField.style.flexGrow = 1;
        
        PropertyField typeField = new PropertyField(property.FindPropertyRelative("type"), "");

        content.Add(nameField);
        content.Add(typeField);


        container.Add(content);
    }

}




#endif