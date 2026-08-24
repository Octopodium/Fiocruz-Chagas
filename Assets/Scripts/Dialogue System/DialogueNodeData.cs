using UnityEngine;
using System;
using System.Collections.Generic;

public enum NodeType
{
    Start,
    Dialogue,
    Condition,
    End
}

[Serializable]
public class DialogueNodeData
{
    public string Guid;
    public string nodeName;
    public string dialogueText;
    public Vector2 position;

    // Node classification
    public NodeType nodeType = NodeType.Dialogue;

    // End node and events
    public string sceneName;
    public bool _loadScene;
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

    // Choices list (to preserve unconnected choice ports)
    public List<string> choices = new List<string>();

    // Condition node
    public string conditionVariableName;
}
