using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[Serializable]
public class DialogueNode : Node
{
    public string GUID;
    public string NodeName;
    public string DialogueText;
    public bool _entryPoint = false;
    public bool _exitPoint = false;

    // Node classification
    public NodeType nodeType = NodeType.Dialogue;

    // End node and events
    public string sceneName;
    public bool _loadScene = false;
    public string commandString;

    // Visuals settings
    public Sprite backgroundSprite;
    public List<Sprite> characterSprites = new List<Sprite>();
    public float characterFadeTime = 0.5f;
    public float backgroundFadeTime = 0.5f;
    public Color backgroundFadeColor = Color.black;
    public string characterName;
    public float valueX;

    // Audio settings
    public AudioClip bgmAudio;
    public AudioClip sfxAudio;

    // Condition node
    public string conditionVariableName;

    // UI Field References
    public ObjectField backgroundSpriteField;
}
