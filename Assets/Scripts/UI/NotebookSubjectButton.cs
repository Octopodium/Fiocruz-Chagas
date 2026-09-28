using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component attached to the Subject Button prefab in the Notebook grid.
/// </summary>
public class NotebookSubjectButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text labelTmpText;

    private NoteData currentNote;
    private Action<NoteData> onClickCallback;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }
    }

    /// <summary>
    /// Configures the button display for a given note and assigns the click callback.
    /// </summary>
    public void Setup(NoteData noteData, Action<NoteData> onClick)
    {
        currentNote = noteData;
        onClickCallback = onClick;

        if (currentNote == null) return;

        string label = currentNote.ButtonLabel;

        if (labelTmpText != null)
        {
            labelTmpText.text = label;
        }
    }

    private void HandleClick()
    {
        if (currentNote != null && onClickCallback != null)
        {
            onClickCallback.Invoke(currentNote);
        }
    }
}
