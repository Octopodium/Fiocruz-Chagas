using System;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif


/// <summary>
/// The bare minumum to have a serializable low typed field. This implementation follows heavily how UnityEvent implements it.
/// </summary>
[Serializable]
public struct GenericValue {
    public enum GenericType { NULL, INT, FLOAT, STRING, BOOL, OBJECT }
    public GenericType type;
    public int intValue;
    public float floatValue;
    public string stringValue;
    public bool boolValue;
    public UnityEngine.Object objectRefValue;

    /// <summary>
    /// Get the value as an object (untyped variable).
    /// </summary>
    /// <returns>The generic value as an object</returns>
    public object GetValue(){
        switch (type) {
            case GenericType.NULL:
                return null;
            case GenericType.INT:
                return intValue;
            case GenericType.FLOAT:
                return floatValue;
            case GenericType.STRING:
                return stringValue;
            case GenericType.BOOL:
                return boolValue;
            case GenericType.OBJECT:
                return objectRefValue;
        }
        return null;
    }

    /// <summary>
    /// Sets the value and defines the type. If can't define a type, sets value as null.
    /// </summary>
    /// <param name="val">Value as an object. Although an object, if the struct doesn't support it, will be ignored and saved as null.</param>
    public void SetValue(object val) {
        ClearValues();

        Type valueType = val.GetType();

        if (valueType == typeof(int)) {
            type = GenericType.INT;
            intValue = (int) val;
        } else if (valueType == typeof(float)) {
            type = GenericType.FLOAT;
            floatValue = (float) val;
        } else if (valueType == typeof(string)) {
            type = GenericType.STRING;
            stringValue = (string) val;
        } else if (valueType == typeof(bool)) {
            type = GenericType.BOOL;
            boolValue = (bool) val;
        } else if (valueType == typeof(UnityEngine.Object)) {
            type = GenericType.OBJECT;
            objectRefValue = (UnityEngine.Object) val;
        }
    }

    /// <summary>
    /// Clear every value and set it as null
    /// </summary>
    public void ClearValues() {
        type = GenericType.NULL;
        intValue = default;
        floatValue = default;
        stringValue = default;
        boolValue = default;
        objectRefValue = default;
    }

    /// <summary>
    /// Gets the GenericType enum related to the Type. Can be used to check if a certain value is supported by the struct, as if it's not, will return GenericType.NULL.
    /// </summary>
    /// <param name="convertType">The enum associated with the type or GenericType.NULL if unsupported.</param>
    /// <returns></returns>
    public static GenericType GetGenericType(Type convertType) {
        if (convertType == typeof(int)) {
            return GenericType.INT;
        } else if (convertType == typeof(float)) {
            return GenericType.FLOAT;
        } else if (convertType == typeof(string)) {
            return GenericType.STRING;
        } else if (convertType == typeof(bool)) {
            return GenericType.BOOL;
        } else if (typeof(UnityEngine.Object).IsAssignableFrom(convertType)) {
            return GenericType.OBJECT;
        }

        return GenericType.NULL;
    }
}



#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(GenericValue))]
public class GenericValueDrawer : PropertyDrawer {

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        VisualElement container = new VisualElement();
        BuildUI(container, property);

        return container;
    }


    public void BuildUI(VisualElement container, SerializedProperty property) {
        VisualElement content = new VisualElement();
        content.style.flexDirection = FlexDirection.Row;

        SerializedProperty typeProperty = property.FindPropertyRelative("type");
        GenericValue.GenericType typeValue = (GenericValue.GenericType) typeProperty.enumValueIndex;
        typeProperty.Dispose();

        string propertyName = "intValue";

        switch (typeValue) {
            case GenericValue.GenericType.NULL:
                break;
            case GenericValue.GenericType.INT:
                propertyName = "intValue";
                break;
            case GenericValue.GenericType.FLOAT:
                propertyName = "floatValue";
                break;
            case GenericValue.GenericType.STRING:
                propertyName = "stringValue";
                break;
            case GenericValue.GenericType.BOOL:
                propertyName = "boolValue";
                break;
            case GenericValue.GenericType.OBJECT:
                propertyName = "objectRefValue";
                break;
        }

        SerializedProperty valueProperty = property.FindPropertyRelative(propertyName);
        PropertyField valueField = new PropertyField(valueProperty, "");
        valueField.style.flexGrow = 1;

        content.Add(valueField);


        container.Add(content);
    }

}

#endif