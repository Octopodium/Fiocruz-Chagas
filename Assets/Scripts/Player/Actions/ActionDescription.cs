using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;



#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#endif


/// <summary>
/// A generic action to be set in inspector. To execute the action, call 'RunAction()'.
/// </summary>
[Serializable]
public class ActionDescription {
    public enum ActionType { Flags, UnityEvents, GameManager, PlayerReference }
    public enum AssignOption { Set, Add, Subtract, Multiply, Divide, And, Or }
    public ActionType type;
    public UnityEvent unityEvent;


    public string componentType;
    public string name;
    public AssignOption assignOption;
    public GenericValue value;


    /// <summary>
    /// Tries to run the action.
    /// </summary>
    public void RunAction() {
        if (type == ActionType.UnityEvents) unityEvent?.Invoke();
        else if (type == ActionType.Flags) SetFlagAssign(name, assignOption, value.GetValue());
        else {
            object baseComponent = (type == ActionType.GameManager) ? GameManager.instance : GameManager.instance.player;
            if (componentType != "this") baseComponent = ConditionDescription.GetPropertyValue(baseComponent, componentType);
            if (baseComponent == null) return;

            object resultValue = ConditionDescription.GetPropertyValue(baseComponent, name);
            if (typeof(MulticastDelegate).IsAssignableFrom(resultValue.GetType())) {
                object parameter = value.GetValue();
                Delegate delegateMethod = (Delegate) resultValue;
                
                if (delegateMethod.Method.GetParameters().Length == 0) delegateMethod.DynamicInvoke();
                else delegateMethod.DynamicInvoke(parameter);
            }
        }
    }

    /// <summary>
    /// Mostly internal use only, but won't kill if used any other place. Tries to assign flag with said options.
    /// </summary>
    /// <param name="flagName">Flag's name</param>
    /// <param name="assign">The assign option (must be able to do by the flag's type)</param>
    /// <param name="value">The value to be assigned</param>
    /// <returns>True if the assignment worked. Raises an error if assignment doesn't make sense with type.</returns>
    public static bool SetFlagAssign(string flagName, AssignOption assign, object value) {
        if (!GameManager.instance.flags.IsTypeValidForFlag(flagName, value.GetType())) {
            Debug.LogWarning("Flag '" + flagName + "' cannot be assign with value '" + value + "'.");
            return false;
        }

        if (assign == AssignOption.Set) {
            GameManager.instance.flags.SetFlag(flagName, value);
            return true;
        }

        object previousFlagValue = GameManager.instance.flags.GetFlag(flagName, true);

        if (previousFlagValue.GetType().IsAssignableFrom(typeof(string))) {
            if (assign != AssignOption.Add) {
                Debug.LogWarning("Failed trying to " + assign + " flag '" + flagName + "', as a String you can only Set or Add to it.");
                return false;
            }

            GameManager.instance.flags.SetFlag(flagName, previousFlagValue + value.ToString());
            return true;
        }



        double mathResult;

        switch (assign) {
            case AssignOption.Add:
                mathResult = (double) Convert.ChangeType(previousFlagValue, typeof(double)) + (double) Convert.ChangeType(value, typeof(double));
                GameManager.instance.flags.SetFlag(flagName, Convert.ChangeType(mathResult, value.GetType()));
                return true;
            case AssignOption.Subtract:
                mathResult = (double) Convert.ChangeType(previousFlagValue, typeof(double)) - (double) Convert.ChangeType(value, typeof(double));
                GameManager.instance.flags.SetFlag(flagName, Convert.ChangeType(mathResult, value.GetType()));
                return true;
            case AssignOption.Multiply:
                mathResult = (double) Convert.ChangeType(previousFlagValue, typeof(double)) * (double) Convert.ChangeType(value, typeof(double));
                GameManager.instance.flags.SetFlag(flagName, Convert.ChangeType(mathResult, value.GetType()));
                return true;
            case AssignOption.Divide:
                mathResult = (double) Convert.ChangeType(previousFlagValue, typeof(double)) / (double) Convert.ChangeType(value, typeof(double));
                GameManager.instance.flags.SetFlag(flagName, Convert.ChangeType(mathResult, value.GetType()));
                return true;
            case AssignOption.And:
                GameManager.instance.flags.SetFlag(flagName, Convert.ToBoolean(previousFlagValue) && Convert.ToBoolean(value));
                return true;
            case AssignOption.Or:
                GameManager.instance.flags.SetFlag(flagName, Convert.ToBoolean(previousFlagValue) || Convert.ToBoolean(value));
                return true;
        }

        return false;
    }



}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ActionDescription))]
public class ActionDescriptionDrawer : PropertyDrawer {

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        VisualElement container = new VisualElement();
        BuildUI(container, property);
        return container;
    }

    void RebuildUI(SerializedProperty trackedProperty) {
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }

    public void BuildUI(VisualElement container, SerializedProperty property) {
        Foldout foldout = new Foldout() { text = property.displayName };

        VisualElement content = new VisualElement();
        content.style.paddingBottom = 8;

        // First Part
        VisualElement part1Content = new VisualElement();
        part1Content.style.flexDirection = FlexDirection.Row;
        content.Add(part1Content);

        VisualElement typeContainer = new VisualElement();
        typeContainer.style.flexDirection = FlexDirection.Row;

        Label typeLabel = new Label("Type:");
        typeLabel.style.width = Length.Percent(30);
        typeLabel.style.height = Length.Pixels(20);
        typeLabel.style.marginLeft = Length.Pixels(3);
        typeLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

        SerializedProperty typeProp = property.FindPropertyRelative("type");
        PropertyField typeField = new PropertyField(typeProp, "");
        typeField.style.width = Length.Percent(70);
        typeField.TrackPropertyValue(typeProp, RebuildUI);

        typeContainer.Add(typeLabel);
        typeContainer.Add(typeField);

        content.Add(typeContainer);

        ActionDescription.ActionType type = (ActionDescription.ActionType) typeProp.enumValueIndex;

        if (type == ActionDescription.ActionType.UnityEvents) {
            PropertyField eventField = new PropertyField(property.FindPropertyRelative("unityEvent"), "Extra Action");
            content.Add(eventField);
        } else if (type == ActionDescription.ActionType.Flags) {
            content.Add(BuildFlagAssigner(property));
        } else {
            content.Add(DrawClassSelector(property, type));
            content.Add(DrawFunctionSelector(property, type));
        }

        
        // Add fields to the container.
        //foldout.Add(content);
        container.Add(content);
    }

    VisualElement BuildFlagAssigner(SerializedProperty property) {
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;

        VisualElement leftRow = new VisualElement();
        leftRow.style.width = Length.Percent(30);
        leftRow.style.flexDirection = FlexDirection.Column;
        VisualElement rightRow = new VisualElement();
        rightRow.style.width = Length.Percent(70);
        leftRow.style.flexDirection = FlexDirection.Column;

        container.Add(leftRow);
        container.Add(rightRow);

        Label flagLabel = new Label("Flag:");
        flagLabel.style.height = Length.Pixels(20);
        flagLabel.style.marginLeft = Length.Pixels(3);
        flagLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
        leftRow.Add(flagLabel);


        SerializedProperty nameProp = property.FindPropertyRelative("name");
        SortedDictionary<string, object> data = new SortedDictionary<string, object>();

        foreach (FlagDescriptor descriptor in FlagsRegister.instance.availableFlags) {
            data[descriptor.name + ": " + descriptor.readableType] = descriptor.name;
        }

        VisualElement nameField = CreatePopup(property, nameProp, data);
        nameField.TrackPropertyValue(nameProp, RebuildUI);
        rightRow.Add(nameField);


        SerializedProperty assingProp = property.FindPropertyRelative("assignOption");
        PropertyField assingField = new PropertyField(assingProp, "");
        leftRow.Add(assingField);



        SerializedProperty valueProperty = property.FindPropertyRelative("value");
        FlagDescriptor flagDescriptor = FlagsRegister.instance.GetDescriptorByName(nameProp.stringValue);
        Type flagType = flagDescriptor != null ? FlagDescriptor.GetTypeByFlagTypes(flagDescriptor.type) : null;

        GenericValue val = (GenericValue) valueProperty.boxedValue;
        if (flagType != null) {
            val.type = GenericValue.GetGenericType(flagType);
            valueProperty.boxedValue = val;
            valueProperty.serializedObject.ApplyModifiedProperties();
        }

        PropertyField parameterField = new PropertyField(valueProperty, "");
        rightRow.Add(parameterField);


        return container;
    }

    VisualElement DrawClassSelector(SerializedProperty property, ActionDescription.ActionType actionType) {
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Column;

        SerializedProperty componentProperty = property.FindPropertyRelative("componentType");

        if(actionType == ActionDescription.ActionType.PlayerReference) {
            SortedDictionary<string, object> systemsData = GetTypeSystems(typeof(Player));
            container = CreatePopup(property, componentProperty, systemsData);
        } else if(actionType == ActionDescription.ActionType.GameManager){
            SortedDictionary<string, object> systemsData = GetTypeSystems(typeof(GameManager));
            container = CreatePopup(property, componentProperty, systemsData);
        }

        container.TrackPropertyValue(componentProperty, RebuildUI);


        return container;
    }

    VisualElement DrawFunctionSelector(SerializedProperty property, ActionDescription.ActionType actionType) {
        VisualElement container = new VisualElement();
        VisualElement functionField;

        SerializedProperty functionProperty = property.FindPropertyRelative("name");
        SerializedProperty componentProperty = property.FindPropertyRelative("componentType");
        string componentName = componentProperty.stringValue;

        Type componentType = (actionType == ActionDescription.ActionType.GameManager) ? typeof(GameManager) : typeof(Player);
        if (componentName != "this") componentType = GetFieldOrPropertyType(componentType, componentName);

        SortedDictionary<string, object> data = componentType != null ? GetTypeFunctions(componentType) : new SortedDictionary<string, object>();

        if (data.Count > 0) {
            functionField = CreatePopup(property, functionProperty, data);
        } else {
            functionField = CreatePopup(property, functionProperty, new SortedDictionary<string, object> {{"No valid function", ""}});
            functionField.SetEnabled(false);
        }

        functionField.TrackPropertyValue(functionProperty, RebuildUI);
        
        container.Add(functionField);

        MethodInfo method = componentType != null ? componentType.GetMethodFromUnique(functionProperty.stringValue) : null;
        if (method != null) {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 1) {
                VisualElement paramHolder = new VisualElement();
                paramHolder.style.flexDirection = FlexDirection.Row;

                Type parameterType = parameters[0].ParameterType;

                Label functionParameterLabel = new Label("Parameter:");
                functionParameterLabel.style.width = Length.Percent(30);
                functionParameterLabel.style.height = Length.Pixels(20);
                functionParameterLabel.style.marginLeft = Length.Pixels(3);
                functionParameterLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
                paramHolder.Add(functionParameterLabel);

                SerializedProperty parameterProperty = property.FindPropertyRelative("value");
                GenericValue val = (GenericValue) parameterProperty.boxedValue;
                val.type = GenericValue.GetGenericType(parameterType);
                parameterProperty.boxedValue = val;
                parameterProperty.serializedObject.ApplyModifiedProperties();

                PropertyField parameterField = new PropertyField(parameterProperty, "");
                parameterField.style.width = Length.Percent(70);
                paramHolder.Add(parameterField);

                container.Add(paramHolder);
            }
        }


        return container;
    }

    Type GetFieldOrPropertyType(Type type, string name) {
        if (type == null) return null;

        FieldInfo fieldInfo = type.GetField(name);
        if (fieldInfo != null) return fieldInfo.FieldType;

        PropertyInfo propertyInfo = type.GetProperty(name);
        if (propertyInfo != null) return propertyInfo.PropertyType;
        return null;
    }


    #region Popup

    public PopupField<string> CreatePopup(SerializedProperty mainProperty, SerializedProperty property, SortedDictionary<string, object> choices) {
        PopupField<string> dropdown = new PopupField<string>(
            choices: choices.Keys.ToList(),
            defaultIndex: Mathf.Max(0, choices.Values.ToList().FindIndex(x => object.Equals(x,property.boxedValue)))
        );

        dropdown.RegisterValueChangedCallback(evt => {
            property.boxedValue = choices[evt.newValue];
            mainProperty.serializedObject.ApplyModifiedProperties();
        });

        dropdown.AddToClassList(PopupField<string>.alignedFieldUssClassName);

        return dropdown;
    }

    SortedDictionary<string, object> GetTypeSystems(Type type) {
        SortedDictionary<string, object> data = new SortedDictionary<string, object>();

        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
            if (data.ContainsKey(field.FieldType.Name) || field.FieldType.IsPrimitive) continue;
            data.Add(field.FieldType.Name, field.Name);
        }

        if (!data.ContainsKey(typeof(GameObject).Name))
            data.Add(typeof(GameObject).Name, typeof(GameObject).Name);

        if (!data.ContainsKey(type.Name))
            data.Add(type.Name, "this");

        return data;
    }


    SortedDictionary<string, object> GetTypeFunctions(Type type) {
        SortedDictionary<string, object> methodNames = new SortedDictionary<string, object>();

        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
            if (method.IsSpecialName || method.IsGenericMethod || method.ContainsGenericParameters  || method.IsConstructor) continue;

            ParameterInfo[] parameters = method.GetParameters();
            string methodName = method.Name + "(" + (parameters.Length == 0? " " : parameters[0].ParameterType.Name) + "): " + method.ReturnType.Name;
            
            if(parameters.Length > 1 || (parameters.Length == 1 && !IsTypeValid(parameters[0].ParameterType))) continue;
            if(methodNames.ContainsKey(methodName)) continue;

            methodNames.Add(methodName, method.GetUniqueMethodName());
        }

        return methodNames;
    }

    bool IsTypeValid(Type type) {
        return GenericValue.GetGenericType(type) != GenericValue.GenericType.NULL;
    }

    #endregion
}

#endif
