using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Reflection;
using System.Collections.Generic;
#endif


/// <summary>
/// The unit of a quest. It's one of the demands of the quest.
/// See implementations at '/Steps'.
/// </summary>
[Serializable]
public abstract class QuestStep : ScriptableObject{
    public Action OnFinished;
    [TextArea] public string description;

    /// <summary>
    /// Called when the QuestStep becomes the current step of the current Quest.
    /// </summary>
    public abstract void Start();
    /// <summary>
    /// Called by 'Finish' or by QuestControl.ForceStopQuest to stop anything happening on the QuestStep.
    /// </summary>
    public abstract void Stop();
    /// <summary>
    /// Optional override, called on every Update if the QuestStep is the current step.
    /// </summary>
    public virtual void HandleUpdate() {}
    /// <summary>
    /// Optional override, called on every FixedUpdate if the QuestStep is the current step.
    /// </summary>
    public virtual void HandleFixedUpdate() {}

    /// <summary>
    /// Called internally by QuestStep's implementations to signal that the step is over.
    /// </summary>
    protected void Finish() {
        OnFinished?.Invoke();
        Stop();
    }
}


#if UNITY_EDITOR


/// <summary>
/// Displays the QuestSteps content in it's field, so you can edit every aspect of the Quest on the Quest's scriptable.
/// </summary>
[CustomPropertyDrawer(typeof(QuestStep))]
public class QuestStepDrawer : PropertyDrawer {

    public const string BASE_PATH_FOR_NEW_SCRIPTABLE = "Assets/Resources/Quests";
    public static Dictionary<Type, string> ACCEPTED_SCRIPTABLES = new Dictionary<Type, string> {
        {typeof(QSGetItem), "Get Item Step"},
        {typeof(QSUseItem), "Use Item Step"},
        {typeof(QSTalkWith), "Talk With Step"},
    };

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        VisualElement container = new VisualElement();

        DrawElement(container, property);

        container.TrackPropertyValue(property, (trackedProperty) =>  {

            ActiveEditorTracker.sharedTracker.ForceRebuild();
        });

        

        return container;
    }

    void DrawElement(VisualElement container, SerializedProperty property) {
        container.Clear();

        VisualElement header = CreateHeader(property);
        container.Add(header);

        VisualElement body = CreateBody(property);
        container.Add(body);
    }

    VisualElement CreateHeader(SerializedProperty property) {
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;

        PropertyField field = new PropertyField(property);
        field.style.flexGrow = 1;
        field.BindProperty(property); 
        
        Button customButton = new Button{ text = "Add" };
        customButton.style.flexShrink = 1;
        customButton.clicked += () => ShowCreatePopup(customButton, property);

        container.Add(field);
        container.Add(customButton);
        return container;
    }

    protected VisualElement CreateBody(SerializedProperty property) {
        VisualElement container = new VisualElement();

        QuestStep obj = (QuestStep) property.objectReferenceValue;
        if (obj == null) return container;
        SerializedObject serializedObject = new SerializedObject(obj);

        VisualElement props = BuildInspectorProperties(serializedObject);

        Foldout foldout = new Foldout() { text = obj.name };
        foldout.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 1f));
        foldout.style.paddingRight = 8;
        foldout.style.paddingBottom = 8;
        foldout.Add(props);
        container.Add(foldout);

        return container;
    }

    void ShowCreatePopup(VisualElement button, SerializedProperty property) {
        GenericMenu menu = new GenericMenu();
        Rect screenRect = button.worldBound;

        foreach (Type type in ACCEPTED_SCRIPTABLES.Keys) {
            string label = ACCEPTED_SCRIPTABLES[type];
            menu.AddItem(new GUIContent(label), false, () => CreateNewStep(property, type));
        }

        menu.DropDown(screenRect);
    }

    void CreateNewStep(SerializedProperty property, Type type) {
        string absolutePath = EditorUtility.SaveFilePanel(
            "Save ScriptableObject",
            BASE_PATH_FOR_NEW_SCRIPTABLE,
            type.Name + ".asset",
            "asset"
        );

        if (string.IsNullOrEmpty(absolutePath))
            return;
        
        string relativePath = FileUtil.GetProjectRelativePath(absolutePath);
        ScriptableObject scriptableObject = ScriptableObject.CreateInstance(type);
        

        AssetDatabase.CreateAsset(scriptableObject, relativePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        property.objectReferenceValue = scriptableObject;
        property.serializedObject.ApplyModifiedProperties();
    }


    private static VisualElement BuildInspectorProperties(SerializedObject obj) {
        // Code reference: https://discussions.unity.com/t/uitoolkit-inspectorelement-does-not-draw-content-of-scriptableobject-correctly/847153/2
        VisualElement container = new VisualElement {name = obj.targetObject.name};
        
        SerializedProperty iterator = obj.GetIterator();
        Type targetType = obj.targetObject.GetType();
        List<MemberInfo> members = new List<MemberInfo>(targetType.GetMembers());

        if (!iterator.NextVisible(true)) return container;

        do {
            if (iterator.propertyPath == "m_Script" && obj.targetObject != null) continue;
            PropertyField propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };
            propertyField.Bind(iterator.serializedObject);

            MemberInfo member = members.Find(x => x.Name == propertyField.bindingPath);
            if (member != null) {
                IEnumerable<Attribute> headers = member.GetCustomAttributes(typeof(HeaderAttribute));
                IEnumerable<Attribute> spaces = member.GetCustomAttributes(typeof(SpaceAttribute));
                foreach (Attribute x in headers) {
                    HeaderAttribute actual = (HeaderAttribute) x;
                    Label header = new Label { text = actual.header};
                    header.style.unityFontStyleAndWeight = FontStyle.Bold;
                    container.Add(new Label { text = " ", name = "Header Spacer"});
                    container.Add(header);
                }

                foreach (Attribute unused in spaces) {
                    container.Add(new Label { text = " " });
                }
            }

            container.Add(propertyField);
        } while (iterator.NextVisible(false));

        return container;
    }
    
}

#endif