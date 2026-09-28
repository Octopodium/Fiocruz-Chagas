using System;

/// <summary>
/// Serializable data structure storing the state of unlocked notebook notes.
/// </summary>
[Serializable]
public class NotebookSaveData
{
    public string[] unlockedNoteIds;
}
