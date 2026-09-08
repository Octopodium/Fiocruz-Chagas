using System;
using UnityEngine;

/// <summary>
/// The Quest Data as a scriptable object. Holds each quest step and an optional quest to start after the end of the current one.
/// </summary>
[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject {
    [TextArea] public string title;
    public QuestStep[] steps;
    public Quest unlockNextQuest;
}
