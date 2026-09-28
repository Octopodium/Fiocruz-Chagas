using UnityEngine;

/// <summary>
/// Helper component that unlocks a specific NoteData in the NotebookManager when triggered.
/// Useful for Dialogue events, button clicks, trigger zones, or quest completions.
/// </summary>
public class NotebookUnlockTrigger : MonoBehaviour
{
    [Header("Note Target")]
    [Tooltip("Target NoteData asset to unlock")]
    [SerializeField] private NoteData noteToUnlock;

    [Tooltip("Target Note ID to unlock (used if noteToUnlock is not assigned)")]
    [SerializeField] private string noteId;

    [Header("Auto Trigger Options")]
    [SerializeField] private bool unlockOnStart = false;
    [SerializeField] private bool unlockOnEnable = false;
    [SerializeField] private bool unlockOnTriggerEnter = false;

    private void Start()
    {
        if (unlockOnStart)
        {
            TriggerUnlock();
        }
    }

    private void OnEnable()
    {
        if (unlockOnEnable)
        {
            TriggerUnlock();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (unlockOnTriggerEnter && other.CompareTag("Player"))
        {
            TriggerUnlock();
        }
    }

    /// <summary>
    /// Unlocks the configured note in the NotebookManager.
    /// </summary>
    public void TriggerUnlock()
    {
        if (NotebookManager.Instance == null)
        {
            Debug.LogWarning("[NotebookUnlockTrigger] NotebookManager instance is not ready.");
            return;
        }

        if (noteToUnlock != null)
        {
            NotebookManager.Instance.UnlockNote(noteToUnlock);
        }
        else if (!string.IsNullOrEmpty(noteId))
        {
            NotebookManager.Instance.UnlockNote(noteId);
        }
    }

    /// <summary>
    /// Unlocks a specific note ID dynamically via UnityEvent or script.
    /// </summary>
    public void TriggerUnlockById(string customNoteId)
    {
        if (NotebookManager.Instance != null)
        {
            NotebookManager.Instance.UnlockNote(customNoteId);
        }
    }
}
