using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

/// <summary>
/// Core Manager for the in-game Notebook system.
/// Manages note unlocking, dynamic UI grid display, detail view, YarnSpinner integration, and save/load persistence.
/// </summary>
public class NotebookManager : MonoBehaviour, ISaveable
{
    public static NotebookManager Instance { get; private set; }

    [SerializeField] private List<NoteData> noteDatabase = new List<NoteData>();
    [SerializeField] private bool loadFromResources = false;

    [Tooltip("Optional notes that start unlocked immediately (useful for testing)")]
    [SerializeField] private List<NoteData> defaultUnlockedNotes = new List<NoteData>();

    [Header("Notebook Canvas & Navigation")]
    [SerializeField] private Button closeNotebookButton;
    [SerializeField] private Button hudOpenButton;

    [Header("Subject Grid View")]
    [SerializeField] private GameObject gridPanel;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private NotebookSubjectButton subjectButtonPrefab;

    [Header("Detail View")]
    [SerializeField] private NoteDetailView detailView;

    // Runtime state
    private readonly HashSet<string> unlockedNoteIds = new HashSet<string>();
    private readonly Dictionary<string, NoteData> notesLookup = new Dictionary<string, NoteData>();
    private readonly List<NotebookSubjectButton> spawnedButtons = new List<NotebookSubjectButton>();
    private bool isNotebookOpen = false;

    // Events
    public Action<NoteData> OnNoteUnlocked;
    public Action<NoteData> OnNoteOpened;
    public Action OnNotebookOpened;
    public Action OnNotebookClosed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeDatabase();

        foreach (NoteData note in defaultUnlockedNotes)
        {
            if (note != null) UnlockNote(note);
        }

        if (closeNotebookButton != null)
        {
            closeNotebookButton.onClick.AddListener(CloseNotebook);
        }

        if (hudOpenButton != null)
        {
            hudOpenButton.onClick.AddListener(OpenNotebook);
        }

        if (GameManager.instance != null && GameManager.instance.saveManager != null)
        {
            GameManager.instance.saveManager.AddSaveable(this);
        }
    }

    private void Start()
    {
        if (GameManager.instance != null && GameManager.instance.saveManager != null)
        {
            GameManager.instance.saveManager.AddSaveable(this);
        }

        CloseNotebook();
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null && GameManager.instance.saveManager != null)
        {
            GameManager.instance.saveManager.RemoveSaveable(this);
        }

        if (closeNotebookButton != null)
        {
            closeNotebookButton.onClick.RemoveListener(CloseNotebook);
        }

        if (hudOpenButton != null)
        {
            hudOpenButton.onClick.RemoveListener(OpenNotebook);
        }
    }

    #region Database Initialization

    private void InitializeDatabase()
    {
        notesLookup.Clear();

        if (loadFromResources)
        {
            NoteData[] loadedNotes = Resources.LoadAll<NoteData>("Notes");
            foreach (NoteData note in loadedNotes)
            {
                if (note != null && !noteDatabase.Contains(note))
                {
                    noteDatabase.Add(note);
                }
            }
        }

        foreach (NoteData note in noteDatabase)
        {
            if (note == null) continue;

            string id = note.NoteId;
            if (!notesLookup.ContainsKey(id))
            {
                notesLookup.Add(id, note);
            }
            else
            {
                Debug.LogWarning($"[NotebookManager] Duplicate NoteId found: '{id}'. Make sure each NoteData has a unique ID.");
            }
        }
    }

    #endregion

    #region Note Unlocking & Management

    /// <summary>
    /// Unlocks a note by its unique NoteId and updates the UI grid.
    /// Can also be called directly from Yarn Spinner scripts via <<unlock_note note_id>>.
    /// </summary>
    [YarnCommand("unlock_note")]
    public static void YarnUnlockNote(string noteId)
    {
        if (Instance != null)
        {
            Instance.UnlockNote(noteId);
        }
    }

    /// <summary>
    /// Opens the notebook directly from a Yarn script via <<open_notebook>>.
    /// </summary>
    [YarnCommand("open_notebook")]
    public static void YarnOpenNotebook()
    {
        if (Instance != null)
        {
            Instance.OpenNotebook();
        }
    }

    /// <summary>
    /// Unlocks a note by its unique NoteId.
    /// </summary>
    public void UnlockNote(string noteId)
    {
        if (string.IsNullOrEmpty(noteId)) return;

        if (!notesLookup.TryGetValue(noteId, out NoteData note))
        {
            Debug.LogWarning($"[NotebookManager] Cannot unlock note '{noteId}'. Note not found in database.");
            return;
        }

        if (unlockedNoteIds.Add(noteId))
        {
            Debug.Log($"[NotebookManager] Unlocked note: '{note.Title}' (ID: {noteId})");
            OnNoteUnlocked?.Invoke(note);
            RefreshGrid();
        }
    }

    /// <summary>
    /// Unlocks a note directly from its NoteData asset.
    /// </summary>
    public void UnlockNote(NoteData note)
    {
        if (note == null) return;

        if (!notesLookup.ContainsKey(note.NoteId))
        {
            notesLookup[note.NoteId] = note;
            if (!noteDatabase.Contains(note))
            {
                noteDatabase.Add(note);
            }
        }

        UnlockNote(note.NoteId);
    }

    public void LockNote(string noteId)
    {
        if (unlockedNoteIds.Remove(noteId))
        {
            RefreshGrid();
        }
    }

    public bool IsNoteUnlocked(string noteId)
    {
        return unlockedNoteIds.Contains(noteId);
    }

    public void ClearAllNotes()
    {
        unlockedNoteIds.Clear();
        RefreshGrid();
    }

    public NoteData GetNoteById(string noteId)
    {
        return notesLookup.TryGetValue(noteId, out NoteData note) ? note : null;
    }

    #endregion

    #region UI & Navigation

    public void ToggleNotebook()
    {
        if (isNotebookOpen)
        {
            CloseNotebook();
        }
        else
        {
            OpenNotebook();
        }
    }

    public void OpenNotebook()
    {
        isNotebookOpen = true;
        SetNotebookUIVisible(true);
        ShowGrid();
        OnNotebookOpened?.Invoke();
    }

    public void CloseNotebook()
    {
        isNotebookOpen = false;
        if (detailView != null)
        {
            detailView.Hide();
        }
        SetNotebookUIVisible(false);
        OnNotebookClosed?.Invoke();
    }

    public void ShowGrid()
    {
        if (gridPanel != null)
        {
            gridPanel.SetActive(true);
        }

        if (detailView != null)
        {
            detailView.Hide();
        }

        RefreshGrid();
    }

    public void OpenNoteDetail(NoteData note)
    {
        if (note == null) return;

        if (gridPanel != null)
        {
            gridPanel.SetActive(false);
        }

        if (detailView != null)
        {
            detailView.DisplayNote(note, onBack: ShowGrid);
        }

        OnNoteOpened?.Invoke(note);
    }

    public void OpenNoteDetailById(string noteId)
    {
        NoteData note = GetNoteById(noteId);
        if (note != null)
        {
            OpenNoteDetail(note);
        }
    }

    private List<NoteData> GetUnlockedNotesList()
    {
        List<NoteData> list = new List<NoteData>();
        foreach (NoteData note in noteDatabase)
        {
            if (note != null && unlockedNoteIds.Contains(note.NoteId))
            {
                list.Add(note);
            }
        }
        return list;
    }

    /// <summary>
    /// Instantiates buttons for all unlocked notes directly inside the gridContainer (GridLayoutGroup handles layout).
    /// </summary>
    public void RefreshGrid()
    {
        // Clear previous buttons
        foreach (var btn in spawnedButtons)
        {
            if (btn != null) Destroy(btn.gameObject);
        }
        spawnedButtons.Clear();

        List<NoteData> unlockedList = GetUnlockedNotesList();

        // Instantiate buttons for all unlocked notes
        if (gridContainer != null && subjectButtonPrefab != null)
        {
            foreach (NoteData note in unlockedList)
            {
                NotebookSubjectButton btn = Instantiate(subjectButtonPrefab, gridContainer);
                btn.Setup(note, OpenNoteDetail);
                spawnedButtons.Add(btn);
            }
        }
    }

    private void SetNotebookUIVisible(bool isVisible)
    {
        if (TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
        else
        {
            gameObject.SetActive(isVisible);
        }
    }

    #endregion

    #region ISaveable Implementation

    public PlayerData Save(PlayerData data)
    {
        if (data == null) return data;

        NotebookSaveData save = new NotebookSaveData
        {
            unlockedNoteIds = new List<string>(unlockedNoteIds).ToArray()
        };

        data.notebook = save;
        Debug.Log($"[NotebookManager] Saved {unlockedNoteIds.Count} unlocked notes.");
        return data;
    }

    public void Load(PlayerData data)
    {
        unlockedNoteIds.Clear();

        if (data != null && data.notebook != null && data.notebook.unlockedNoteIds != null)
        {
            foreach (string id in data.notebook.unlockedNoteIds)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    unlockedNoteIds.Add(id);
                }
            }
            Debug.Log($"[NotebookManager] Loaded {unlockedNoteIds.Count} unlocked notes from save data.");
        }
        else
        {
            Debug.Log("[NotebookManager] No notebook save data found or data is empty.");
        }

        RefreshGrid();
    }

    #endregion
}
