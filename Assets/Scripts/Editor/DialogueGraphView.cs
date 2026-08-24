using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(200,300);

    public DialogueGraphView(){
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        var gridBackgorund = new GridBackground();
        Insert(0, gridBackgorund);
        gridBackgorund.StretchToParentSize();

        AddElement(GenerateEntryNode());
        AddElement(GenerateExitNode());
    }

    private DialogueNode GenerateEntryNode(){
        var node = new DialogueNode{
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRY DIALOGUE",
            _entryPoint = true,
            nodeType = NodeType.Start
        };

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        node.AddToClassList("entry-node");

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100,200,100,200));
        return node;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter){
        var compatiblePorts = new List<Port>();
        ports.ForEach((port) => {
            if(startPort != port && startPort.node != port.node && startPort.direction != port.direction){
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }

    public Port GeneratePort(DialogueNode node, Direction direction, Port.Capacity capacity = Port.Capacity.Single){
        return node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
    }

    public void CreateNode(string nodeName, NodeType type){
        DialogueNode tempNode = null;
        switch (type)
        {
            case NodeType.Dialogue:
                tempNode = CreateDialogueNode(nodeName, null);
                break;
            case NodeType.Condition:
                tempNode = GenerateConditionNode(null);
                break;
            case NodeType.End:
                tempNode = GenerateExitNode(null);
                break;
        }
        if (tempNode != null)
        {
            AddElement(tempNode);
        }
    }

    public DialogueNode CreateDialogueNode(string nodeName, DialogueNodeData nodeData = null){
        var dialogueNode = new DialogueNode{
            title = nodeName,
            NodeName = nodeName,
            GUID = nodeData != null ? nodeData.Guid : Guid.NewGuid().ToString(),
            nodeType = NodeType.Dialogue
        };

        if (nodeData != null) {
            dialogueNode.DialogueText = nodeData.dialogueText;
            dialogueNode.backgroundSprite = nodeData.backgroundSprite;
            dialogueNode.characterSprites = new List<Sprite>(nodeData.characterSprites ?? new List<Sprite>());
            dialogueNode.characterFadeTime = nodeData.characterFadeTime;
            dialogueNode.backgroundFadeTime = nodeData.backgroundFadeTime;
            dialogueNode.backgroundFadeColor = nodeData.backgroundFadeColor;
            dialogueNode.characterName = nodeData.characterName;
            dialogueNode.bgmAudio = nodeData.bgmAudio;
            dialogueNode.sfxAudio = nodeData.sfxAudio;
        } else {
            dialogueNode.DialogueText = "Write your conversation here";
            dialogueNode.backgroundFadeColor = Color.black;
            dialogueNode.characterSprites = new List<Sprite>();
            dialogueNode.characterName = "";
        }

        var addChoiceBtn = new Button(() => {
            AddChoicePort(dialogueNode);
        }) { text = "Add Choice" };
        dialogueNode.titleContainer.Add(addChoiceBtn);

        SetupDialogueNodeUI(dialogueNode);

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "input";
        dialogueNode.inputContainer.Add(inputPort);

        if (nodeData == null) {
            AddChoicePort(dialogueNode, "Next");
        } else {
            if (nodeData.choices == null || nodeData.choices.Count == 0) {
                AddChoicePort(dialogueNode, "Next");
            } else {
                foreach (var choice in nodeData.choices) {
                    AddChoicePort(dialogueNode, choice);
                }
            }
        }

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return dialogueNode;
    }

    private void SetupDialogueNodeUI(DialogueNode dialogueNode)
    {
        var charNameField = new TextField("Char Name") { value = dialogueNode.characterName };
        charNameField.RegisterValueChangedCallback(evt => dialogueNode.characterName = evt.newValue);
        dialogueNode.mainContainer.Add(charNameField);

        var dialogueConversation = CreateTextField();
        dialogueConversation.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.DialogueText = evt.newValue;
        });
        dialogueConversation.SetValueWithoutNotify(dialogueNode.DialogueText);
        dialogueNode.mainContainer.Add(dialogueConversation);

        var visualFoldout = new Foldout { text = "Visual Settings", value = false };

        dialogueNode.backgroundSpriteField = new ObjectField {
            label = "BG Sprite",
            objectType = typeof(Sprite),
            value = dialogueNode.backgroundSprite,
            allowSceneObjects = false
        };
        dialogueNode.backgroundSpriteField.RegisterValueChangedCallback(evt => {
            dialogueNode.backgroundSprite = evt.newValue as Sprite;
        });
        visualFoldout.Add(dialogueNode.backgroundSpriteField);

        var bgFadeTimeField = new FloatField("BG Fade Time") { value = dialogueNode.backgroundFadeTime };
        bgFadeTimeField.RegisterValueChangedCallback(evt => dialogueNode.backgroundFadeTime = evt.newValue);
        visualFoldout.Add(bgFadeTimeField);

        var bgFadeColorField = new ColorField("BG Fade Color") { value = dialogueNode.backgroundFadeColor };
        bgFadeColorField.RegisterValueChangedCallback(evt => dialogueNode.backgroundFadeColor = evt.newValue);
        visualFoldout.Add(bgFadeColorField);

        var charFadeTimeField = new FloatField("Char Fade Time") { value = dialogueNode.characterFadeTime };
        charFadeTimeField.RegisterValueChangedCallback(evt => dialogueNode.characterFadeTime = evt.newValue);
        visualFoldout.Add(charFadeTimeField);

        var charSpritesContainer = new VisualElement();
        var charSpritesListContainer = new VisualElement();
        
        var addCharBtn = new Button(() => {
            dialogueNode.characterSprites.Add(null);
            RenderCharacterSpritesList(dialogueNode, charSpritesListContainer);
        }) { text = "Add Character Sprite" };
        
        charSpritesContainer.Add(addCharBtn);
        charSpritesContainer.Add(charSpritesListContainer);
        RenderCharacterSpritesList(dialogueNode, charSpritesListContainer);
        visualFoldout.Add(charSpritesContainer);

        dialogueNode.mainContainer.Add(visualFoldout);

        var audioFoldout = new Foldout { text = "Audio Settings", value = false };

        var bgmField = new ObjectField {
            label = "BGM",
            objectType = typeof(AudioClip),
            value = dialogueNode.bgmAudio,
            allowSceneObjects = false
        };
        bgmField.RegisterValueChangedCallback(evt => dialogueNode.bgmAudio = evt.newValue as AudioClip);
        audioFoldout.Add(bgmField);

        var sfxField = new ObjectField {
            label = "SFX / Voice",
            objectType = typeof(AudioClip),
            value = dialogueNode.sfxAudio,
            allowSceneObjects = false
        };
        sfxField.RegisterValueChangedCallback(evt => dialogueNode.sfxAudio = evt.newValue as AudioClip);
        audioFoldout.Add(sfxField);

        dialogueNode.mainContainer.Add(audioFoldout);
    }

    private void RenderCharacterSpritesList(DialogueNode dialogueNode, VisualElement container)
    {
        container.Clear();
        for (int i = 0; i < dialogueNode.characterSprites.Count; i++)
        {
            int index = i;
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            
            var spriteField = new ObjectField {
                label = $"Char {index + 1}",
                objectType = typeof(Sprite),
                value = dialogueNode.characterSprites[index],
                allowSceneObjects = false
            };
            spriteField.style.flexGrow = 1;
            spriteField.RegisterValueChangedCallback(evt => {
                dialogueNode.characterSprites[index] = evt.newValue as Sprite;
            });
            
            var removeBtn = new Button(() => {
                dialogueNode.characterSprites.RemoveAt(index);
                RenderCharacterSpritesList(dialogueNode, container);
            }) { text = "-" };

            row.Add(spriteField);
            row.Add(removeBtn);
            container.Add(row);
        }
    }

    public void AddChoicePort(DialogueNode node, string overriddenPortName = "")
    {
        var port = GeneratePort(node, Direction.Output);
        
        var oldLabel = port.contentContainer.Q<Label>("type");
        if (oldLabel != null) port.contentContainer.Remove(oldLabel);

        var portName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {node.outputContainer.childCount + 1}" : overriddenPortName;
        
        var textField = new TextField() { value = portName };
        textField.RegisterValueChangedCallback(evt => {
            port.portName = evt.newValue;
        });
        port.portName = portName;
        port.contentContainer.Add(new Label("  "));
        port.contentContainer.Add(textField);

        var deleteButton = new Button(() => DeleteChoicePort(node, port)) { text = "X" };
        port.contentContainer.Add(deleteButton);

        node.outputContainer.Add(port);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    private void DeleteChoicePort(DialogueNode node, Port port)
    {
        var edge = edges.ToList().FirstOrDefault(x => x.output == port);
        if (edge != null)
        {
            edge.input.Disconnect(edge);
            RemoveElement(edge);
        }
        node.outputContainer.Remove(port);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    public TextField CreateTextField(){
        var dialogueConversation = new TextField("");
        dialogueConversation.multiline = true;
        dialogueConversation.style.height = 100;
        dialogueConversation.style.width = 200;

        dialogueConversation.style.whiteSpace = WhiteSpace.Normal;
        dialogueConversation.style.unityOverflowClipBox = OverflowClipBox.ContentBox;

        foreach (VisualElement child in dialogueConversation.Children()){
            child.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperLeft);
        }

        return dialogueConversation;
    }

    public DialogueNode GenerateExitNode(DialogueNodeData nodeData = null){ //criando um exit node pra gente chamar a cena no final do dialogo
        var node = new DialogueNode{
            NodeName = "End",
            title = "END",
            GUID = nodeData != null ? nodeData.Guid : Guid.NewGuid().ToString(),
            DialogueText = "Exit Node",
            _exitPoint = true,
            nodeType = NodeType.End
        };

        if (nodeData != null) {
            node._loadScene = nodeData._loadScene;
            node.sceneName = nodeData.sceneName;
            node.commandString = nodeData.commandString;
        } else {
            node.sceneName = "";
            node._loadScene = false;
            node.commandString = "";
        }

        var toggle = new Toggle("Load Scene?") { value = node._loadScene };
        toggle.RegisterValueChangedCallback(evt => node._loadScene = evt.newValue);

        var sceneField = new TextField("Scene Name:") { value = node.sceneName };
        sceneField.name = "SceneNameField";
        sceneField.RegisterValueChangedCallback(evt => node.sceneName = evt.newValue);
        sceneField.SetEnabled(node._loadScene);
    
        toggle.RegisterValueChangedCallback(evt => {
            sceneField.SetEnabled(evt.newValue);
        });

        var commandField = new TextField("Command:") { value = node.commandString };
        commandField.name = "CommandField";
        commandField.RegisterValueChangedCallback(evt => node.commandString = evt.newValue);

        node.mainContainer.Add(toggle);
        node.mainContainer.Add(sceneField);
        node.mainContainer.Add(commandField);

        node.capabilities &= ~Capabilities.Deletable;

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        node.AddToClassList("exit-node");

        var generatedPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        generatedPort.portName = "End";
        node.inputContainer.Add(generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(500,200,500,200));
        return node;
    }

    public DialogueNode GenerateConditionNode(DialogueNodeData nodeData = null){
        var node = new DialogueNode{
            NodeName = "Condition",
            title = "CONDITION",
            GUID = nodeData != null ? nodeData.Guid : Guid.NewGuid().ToString(),
            nodeType = NodeType.Condition
        };

        if (nodeData != null) {
            node.conditionVariableName = nodeData.conditionVariableName;
        } else {
            node.conditionVariableName = "VariableName";
        }

        var conditionField = new TextField("Variable Name:") { value = node.conditionVariableName };
        conditionField.RegisterValueChangedCallback(evt => node.conditionVariableName = evt.newValue);
        node.mainContainer.Add(conditionField);

        var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "input";
        node.inputContainer.Add(inputPort);

        var truePort = GeneratePort(node, Direction.Output);
        truePort.portName = "True";
        node.outputContainer.Add(truePort);

        var falsePort = GeneratePort(node, Direction.Output);
        falsePort.portName = "False";
        node.outputContainer.Add(falsePort);

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        node.RefreshExpandedState();
        node.RefreshPorts();
        node.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return node;
    }
}
