using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName){
        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for(var i = 0; i < connectedPorts.Length; i++){
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                baseNodeGuid = outputNode.GUID,
                portName = connectedPorts[i].output.portName,
                targetNodeGuid = inputNode.GUID
            });
        }

        foreach (var node in Nodes.Where(node => !node._entryPoint))
        {
            var choices = new List<string>();
            foreach (var port in node.outputContainer.Query<Port>().ToList())
            {
                choices.Add(port.portName);
            }

            var nodeData = new DialogueNodeData{
                Guid = node.GUID,
                nodeName = node.NodeName,
                dialogueText = node.DialogueText,
                position = node.GetPosition().position,
                _loadScene = node.nodeType == NodeType.End ? node._loadScene : false,
                sceneName = node.nodeType == NodeType.End ? node.sceneName : "",
                commandString = node.nodeType == NodeType.End ? node.commandString : "",
                
                nodeType = node.nodeType,
                backgroundSprite = node.backgroundSprite,
                characterSprites = new List<Sprite>(node.characterSprites),
                characterFadeTime = node.characterFadeTime,
                backgroundFadeTime = node.backgroundFadeTime,
                backgroundFadeColor = node.backgroundFadeColor,
                characterName = node.characterName,
                bgmAudio = node.bgmAudio,
                sfxAudio = node.sfxAudio,
                conditionVariableName = node.nodeType == NodeType.Condition ? node.conditionVariableName : "",
                choices = choices
            };

            dialogueContainer.DialogueNodeData.Add(nodeData);
        }
        
        if(!AssetDatabase.IsValidFolder("Assets/Resources")){
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();

    }

    public void LoadGraph(string fileName){
        _containerCache = Resources.Load<DialogueContainer>(fileName);
        if(_containerCache == null){
            EditorUtility.DisplayDialog("File Not Found", "This dialogue graph does not exists.", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void ClearGraph(){
        Nodes.Find(x => x._entryPoint).GUID = _containerCache.NodeLinks[0].baseNodeGuid;

        foreach(var node in Nodes){
            if(node._entryPoint) continue;
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));
            _targetGraphView.RemoveElement(node);
        }
    }

    private void CreateNodes(){
        foreach(var nodeData in _containerCache.DialogueNodeData){
            DialogueNode tempNode = null;
            switch (nodeData.nodeType)
            {
                case NodeType.End:
                    tempNode = _targetGraphView.GenerateExitNode(nodeData);
                    break;
                case NodeType.Condition:
                    tempNode = _targetGraphView.GenerateConditionNode(nodeData);
                    break;
                case NodeType.Dialogue:
                default:
                    if (nodeData.nodeName == "End" || nodeData._loadScene)
                    {
                        tempNode = _targetGraphView.GenerateExitNode(nodeData);
                    }
                    else
                    {
                        tempNode = _targetGraphView.CreateDialogueNode(nodeData.nodeName, nodeData);
                    }
                    break;
            }

            tempNode.SetPosition(new Rect(nodeData.position, _targetGraphView.defaultNodeSize));
            _targetGraphView.AddElement(tempNode);
        }
    }

    private void ConnectNodes(){
        foreach(var link in _containerCache.NodeLinks){
            var outputNode = Nodes.FirstOrDefault(n => n.GUID == link.baseNodeGuid);
            var inputNode = Nodes.FirstOrDefault(n => n.GUID == link.targetNodeGuid);
        
            if(outputNode == null || inputNode == null) continue;

            var outputPort = outputNode.outputContainer.Query<Port>().Where(p => p.portName == link.portName).First();

            var inputPort = (Port)inputNode.inputContainer[0];
        
            LinkNodes(outputPort, inputPort);
        }
    }

    private void LinkNodes(Port outputSocket, Port inputSocket){
        var tempEdge = new Edge{
            output = outputSocket,
            input = inputSocket
        };
        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }

}