using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#endif


/// <summary>
/// A generic conditional to be set in inspector. To execute the conditional, call 'GetValue()'.
/// </summary>
[Serializable]
public class ConditionDescription {
    public enum ReferenceType {ObjectReference, GameManager, PlayerReference}
    public ReferenceType referenceType = ReferenceType.ObjectReference;
    public GameObject gameObjectReferenced;
    public string componentType;
    public string functionName;

    public GenericValue functionParameter;

    public enum CompareOptions { Equals, Different, Greater, Lower }
    public CompareOptions compareOptions = CompareOptions.Equals;
    public GenericValue compareValue;



    /// <summary>
    /// Get the referenced values and executes the condition. If anything goes wrong, returns false.
    /// </summary>
    /// <returns>The result of the condition.</returns>
    public bool GetValue() {
        object obj;
        GameObject gameObject = GetObject();
        if (gameObject == null) return false;

        if (componentType == typeof(GameObject).Name) {
            obj = gameObject;
        } else if (referenceType == ReferenceType.ObjectReference) {
            obj = gameObject.GetComponent(componentType);
        } else {
            object system = (referenceType == ReferenceType.GameManager) ? GameManager.instance : GameManager.instance.player;
            if (componentType == "this") obj = system;
            else obj = GetPropertyValue(system, componentType);
        }

        if (obj == null) return false;

        object resultValue = GetPropertyValue(obj, functionName);
        if (typeof(MulticastDelegate).IsAssignableFrom(resultValue.GetType())) {
            object parameter = functionParameter.GetValue();
            
            if (parameter == null) resultValue = ((Delegate) resultValue).DynamicInvoke();
            else resultValue = ((Delegate) resultValue).DynamicInvoke(parameter);
        }


        object comparisonValue = compareValue.GetValue();
        bool v = CheckCondition(resultValue, compareOptions, comparisonValue);

        return v;
    }

    /// <summary>
    /// Internal use only. Returns the gameObjectReferenced or gets the gameObject related with the referenceType.
    /// </summary>
    /// <returns>Returns the gameObject related with the component in which will check the condition.</returns>
    GameObject GetObject() {
        if (referenceType == ReferenceType.ObjectReference) return gameObjectReferenced;
        if (referenceType == ReferenceType.GameManager) return GameManager.instance.gameObject;
        return GameManager.instance.player.gameObject;
    }

    /// <summary>
    /// Tries to get the value of the related property/field/method specified by 'name' in 'obj'.
    /// </summary>
    /// <param name="obj">The object to check for the property/field/method.</param>
    /// <param name="name">The name of the propery/field/method.</param>
    /// <returns>Returns the value of said property/field/method. If the name refers to a method, returns a delagate, not the return of the method.</returns>
    public static object GetPropertyValue(object obj, string name) {
        Type type = obj.GetType();

        PropertyInfo propertyInfo = type.GetProperty(name);
        if (propertyInfo != null) return propertyInfo.GetValue(obj);


        FieldInfo fieldInfo = type.GetField(name);
        if (fieldInfo != null) return fieldInfo.GetValue(obj);


        MethodInfo methodInfo = type.GetMethodFromUnique(name);
        if (methodInfo != null) {
            Delegate methodDelegate = methodInfo.CreateDelegateWithoutFunc(obj);
            return methodInfo.CreateDelegate(methodDelegate.GetType(), obj);
        }

        return null;
    }

    /// <summary>
    /// Executes a condition check of value1 with value2 by the criterea described by CompareOptions.
    /// If both values are unable to perform said comparison, will raise an error (ideally you should only use this if you know you can execute said comparison).
    /// </summary>
    /// <param name="value1">A generic value to compare with another value.</param>
    /// <param name="compare">The comparison operator. If the comparison is numeric, will convert value1 and value2 to 'double' to execute it.</param>
    /// <param name="value2">The second generic value to be compared with the first one.</param>
    /// <returns></returns>
    public static bool CheckCondition(object value1, CompareOptions compare, object value2) {
        if (compare == CompareOptions.Equals) return object.Equals(value1, value2);
        else if (compare == CompareOptions.Different) return !object.Equals(value1, value2);
        else {
            double val1Double = (double) Convert.ChangeType(value1, typeof(double)); 
            double val2Double = (double) Convert.ChangeType(value2, typeof(double)); 

            if (compare == CompareOptions.Greater) return val1Double > val2Double;
            return val1Double < val2Double;
        }
        
    }


    
}



public static class ReflectionHelpers {
    /// <summary>
    /// An unique name that specifies the parameters of the method in it's name.
    /// </summary>
    /// <param name="method">The MethodInfo that will call. (ex: methodInfo.GetUniqueMethodName())</param>
    /// <returns>Return a unique name (in the scope of the class) for the method.</returns>
    public static string GetUniqueMethodName(this MethodInfo method) {
        string uniqueName = method.Name;

        // Append Parameters
        ParameterInfo[] parameters = method.GetParameters();
        uniqueName += "(" + string.Join(":::", parameters.Select(p => p.ParameterType.AssemblyQualifiedName)) + ")";
        uniqueName += ": " + method.ReturnType.AssemblyQualifiedName;

        return uniqueName;
    }

    /// <summary>
    /// Returns a MethodInfo based on the unique name from MethodInfo.GetUniqueMethodName.
    /// </summary>
    /// <param name="type">The Type that will call. (ex: type.GetMethodFromUnique(name))</param>
    /// <param name="uniqueName">The unique name returned from MethodInfo.GetUniqueMethodName</param>
    /// <returns>Returns the MethodInfo related with the uniqueName, if not found, returns null.</returns>
    public static MethodInfo GetMethodFromUnique(this Type type, string uniqueName) {
        int parameterListStartIndex = uniqueName.IndexOf('(');
        if (parameterListStartIndex < 0) return null;

        string nameFromUniqueName = uniqueName.Substring(0, parameterListStartIndex);

        int length = uniqueName.IndexOf(')') - parameterListStartIndex - 1;
        string[] parametersNames = uniqueName.Substring(parameterListStartIndex + 1, length).Split(":::");
        Type[] parametersTypes = parametersNames.Select(p => Type.GetType(p)).Where(p => p != null).ToArray();

        return type.GetMethod(nameFromUniqueName, parametersTypes);
    }

    /// <summary>
    /// Creates a Delegate from MethodInfo without necessarily knowing each parameter type.
    /// </summary>
    /// <param name="method">The MethodInfo that will call. (ex: methodInfo.CreateDelegateWithoutFunc(obj))</param>
    /// <param name="target">Only needed for non static methods. The target that will execute the method as if it was being called by it ( like target.[method name]( ) )</param>
    /// <returns>Returns the MethodInfo's delegate, so you can call it like a function.</returns>
    public static Delegate CreateDelegateWithoutFunc(this MethodInfo method, object target = null) {
        IEnumerable<Type> paramTypes = method.GetParameters().Select(p => p.ParameterType).Where(p => p != null);
            
        Type[] types = paramTypes.Append(method.ReturnType).ToArray();
        Type delegateType = Expression.GetDelegateType(types);

        return target == null ? method.CreateDelegate(delegateType) : method.CreateDelegate(delegateType, target);
    }
}




#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ConditionDescription))]
public class ConditionDescriptionDrawer : PropertyDrawer {

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

        // Left Section
        VisualElement leftSection = DrawLeftSection(property, out ConditionDescription.ReferenceType referenceType);
        leftSection.style.width = Length.Percent(30);
        part1Content.Add(leftSection);

        // Right Section
        VisualElement rightSection = DrawRightSection(property, referenceType);
        rightSection.style.width = Length.Percent(70);
        part1Content.Add(rightSection);


        
        // Second Part
        VisualElement p = DrawParameters(property, referenceType);
        content.Add(p);

        
        // Add fields to the container.
        //foldout.Add(content);
        container.Add(content);
    }

    VisualElement DrawLeftSection(SerializedProperty property, out ConditionDescription.ReferenceType referenceType) {
        VisualElement content = new VisualElement();

        // Create property fields.
        SerializedProperty referenceProperty = property.FindPropertyRelative("referenceType");
        VisualElement isGMPopup = new PropertyField(referenceProperty, "");
        isGMPopup.TrackPropertyValue(referenceProperty, RebuildUI);
        content.Add(isGMPopup);
        referenceType = (ConditionDescription.ReferenceType) referenceProperty.enumValueIndex;

        SerializedProperty goProperty = property.FindPropertyRelative("gameObjectReferenced");
        PropertyField gameObjectField = new PropertyField(goProperty, "");
        gameObjectField.TrackPropertyValue(goProperty, RebuildUI);
        content.Add(gameObjectField);
        gameObjectField.SetEnabled(referenceType == ConditionDescription.ReferenceType.ObjectReference);

        return content;
    }

    VisualElement DrawRightSection(SerializedProperty property, ConditionDescription.ReferenceType referenceType) {
        VisualElement content = new VisualElement();

        VisualElement componentField = DrawClassSelector(property, referenceType, out bool hasObject);
        content.Add(componentField);

        SerializedProperty functionProperty = property.FindPropertyRelative("functionName");

        if (referenceType != ConditionDescription.ReferenceType.ObjectReference || hasObject) {
            SerializedProperty componentProperty = property.FindPropertyRelative("componentType");
            string componentName = componentProperty.stringValue;
            Type componentType = componentName != null ? GetComponentType(property, referenceType, componentName) : null;

            SortedDictionary<string, object> data = componentType != null ? GetTypeFunctions(componentType) : new SortedDictionary<string, object>();
            VisualElement functionField;

            if (data.Count > 0) {
                functionField = CreatePopup(property, functionProperty, data);
            } else {
                functionField = CreatePopup(property, functionProperty, new SortedDictionary<string, object> {{"No valid function", ""}});
                functionField.SetEnabled(false);
            }

            functionField.TrackPropertyValue(functionProperty, RebuildUI);
            content.Add(functionField);

        }

        return content;
    }

    VisualElement DrawClassSelector(SerializedProperty property, ConditionDescription.ReferenceType referenceType, out bool hasObject) {
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Column;

        SerializedProperty componentProperty = property.FindPropertyRelative("componentType");
        hasObject = false;

        if (referenceType == ConditionDescription.ReferenceType.ObjectReference) {
            GameObject gameObjectReferenced = (GameObject) property.FindPropertyRelative("gameObjectReferenced").objectReferenceValue;
            hasObject = gameObjectReferenced != null;

            if (!hasObject) {
                container = CreatePopup(property, componentProperty, new SortedDictionary<string, object>{{"No Object", ""}});
                container.SetEnabled(false);
            } else {
                SortedDictionary<string, object> objectsData = GetComponentsTypeNames(gameObjectReferenced);
                container = CreatePopup(property, componentProperty, objectsData);
            }

        } else if(referenceType == ConditionDescription.ReferenceType.PlayerReference) {
            SortedDictionary<string, object> systemsData = GetTypeSystems(typeof(Player));
            container = CreatePopup(property, componentProperty, systemsData);
        } else {
            SortedDictionary<string, object> systemsData = GetTypeSystems(typeof(GameManager));
            container = CreatePopup(property, componentProperty, systemsData);
        }

        container.TrackPropertyValue(componentProperty, RebuildUI);


        return container;
    }


    VisualElement DrawParameters(SerializedProperty property, ConditionDescription.ReferenceType referenceType) {
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;

        VisualElement leftRow = new VisualElement();
        leftRow.style.flexDirection = FlexDirection.Column;
        leftRow.style.width = Length.Percent(30);

        VisualElement rightRow = new VisualElement();
        rightRow.style.flexDirection = FlexDirection.Column;
        rightRow.style.width = Length.Percent(70);



        // Function parameter
        string componentName = property.FindPropertyRelative("componentType").stringValue;
        string functionName = property.FindPropertyRelative("functionName").stringValue;

        Type type = GetComponentType(property, referenceType, componentName);
        MethodInfo method = type != null ? type.GetMethodFromUnique(functionName) : null;

        if (method != null) {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 1) {
                Type parameterType = parameters[0].ParameterType;

                Label functionParameterLabel = new Label("Parameter:");
                functionParameterLabel.style.height = Length.Pixels(20);
                functionParameterLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
                leftRow.Add(functionParameterLabel);

                SerializedProperty parameterProperty = property.FindPropertyRelative("functionParameter");
                GenericValue val = (GenericValue) parameterProperty.boxedValue;
                val.type = GenericValue.GetGenericType(parameterType);
                parameterProperty.boxedValue = val;
                parameterProperty.serializedObject.ApplyModifiedProperties();

                PropertyField parameterField = new PropertyField(parameterProperty, "");
                rightRow.Add(parameterField);
            }
        }


        // Compare value
        SerializedProperty comparisonOptionProperty = property.FindPropertyRelative("compareOptions");
        PropertyField comparisonOptionField = new PropertyField(comparisonOptionProperty, "");
        leftRow.Add(comparisonOptionField);

        Type returnType;
        if (method != null) returnType = method.ReturnType;
        else returnType = GetFieldOrPropertyType(type, functionName);

        SerializedProperty compareValueProperty = property.FindPropertyRelative("compareValue");
        GenericValue propertyValue = (GenericValue) compareValueProperty.boxedValue;
        propertyValue.type = GenericValue.GetGenericType(returnType);
        compareValueProperty.boxedValue = propertyValue;
        compareValueProperty.serializedObject.ApplyModifiedProperties();

        PropertyField compareValueField = new PropertyField(compareValueProperty, "");
        rightRow.Add(compareValueField);




        container.Add(leftRow);
        container.Add(rightRow);

        return container;
    }



    Type GetComponentType(SerializedProperty mainProperty, ConditionDescription.ReferenceType referenceType, string componentName) {
        if (componentName == typeof(GameObject).Name) return typeof(GameObject);

        if (referenceType == ConditionDescription.ReferenceType.ObjectReference) {
            GameObject gameObjectReferenced = (GameObject) mainProperty.FindPropertyRelative("gameObjectReferenced").objectReferenceValue;
            object val = gameObjectReferenced != null ? gameObjectReferenced.GetComponent(componentName) : null;
            return val == null ? null : val.GetType();
        }

        Type type = (referenceType == ConditionDescription.ReferenceType.GameManager) ? typeof(GameManager) : typeof(Player);
        if (componentName == "this") return type;
        
        return GetFieldOrPropertyType(type, componentName);
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

    SortedDictionary<string, object> GetComponentsTypeNames(GameObject go) {
        SortedDictionary<string, object> data = new SortedDictionary<string, object>();

        foreach (Component component in go.GetComponents<Component>()) {
            if (data.ContainsKey(component.GetType().Name)) continue;
            data.Add(component.GetType().Name, component.GetType().Name);
        }

        if (!data.ContainsKey(typeof(GameObject).Name))
            data.Add(typeof(GameObject).Name, typeof(GameObject).Name);

        return data;
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
            if (method.IsSpecialName || method.IsGenericMethod || method.ContainsGenericParameters  || method.IsConstructor || !IsTypeValid(method.ReturnType)) continue;

            ParameterInfo[] parameters = method.GetParameters();
            string methodName = method.Name + "(" + (parameters.Length == 0? " " : parameters[0].ParameterType.Name) + "): " + method.ReturnType.Name;
            
            if(parameters.Length > 1 || (parameters.Length == 1 && !IsTypeValid(parameters[0].ParameterType))) continue;
            if(methodNames.ContainsKey(methodName)) continue;

            methodNames.Add(methodName, method.GetUniqueMethodName());
        }


        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
            if (!IsTypeValid(field.FieldType)) continue;

            string fieldName = field.Name + ": " + field.FieldType.Name;
            if(methodNames.ContainsKey(fieldName)) continue;
            methodNames.Add(fieldName, field.Name);
        }

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
            if (!IsTypeValid(prop.PropertyType)) continue;

            string propName = prop.Name + ": " + prop.PropertyType.Name;
            if(methodNames.ContainsKey(propName)) continue;
            methodNames.Add(propName, prop.Name);
        }

        return methodNames;
    }

    bool IsTypeValid(Type type) {
        return GenericValue.GetGenericType(type) != GenericValue.GenericType.NULL;
    }

    #endregion
}

#endif