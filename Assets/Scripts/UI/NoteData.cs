using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// ScriptableObject that represents a note/subject entry in the Notebook system.
/// </summary>
[CreateAssetMenu(fileName = "NewNote", menuName = "Notebook/Note Data")]
public class NoteData : ScriptableObject
{
    [SerializeField] private string noteId;
    [SerializeField] private string title;
    [SerializeField] private string buttonLabel;

    [TextArea(2, 5)]
    [SerializeField] private List<string> bulletPoints = new List<string>();

    [SerializeField] private Sprite noteImage;
    [SerializeField] private bool preserveImageAspect = true;

    // Properties
    public string NoteId => string.IsNullOrEmpty(noteId) ? name : noteId;
    public string Title => title;
    public string ButtonLabel => string.IsNullOrEmpty(buttonLabel) ? title : buttonLabel;
    public IReadOnlyList<string> BulletPoints => bulletPoints;
    public Sprite NoteImage => noteImage;
    public bool HasImage => noteImage != null;
    public bool PreserveImageAspect => preserveImageAspect;

    /// <summary>
    /// Formats a slice of bullet points as a multi-line string.
    /// </summary>
    public string GetFormattedBulletPoints(int startIndex, int count, string bulletSymbol = "- ")
    {
        if (bulletPoints == null || bulletPoints.Count == 0 || count <= 0)
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();
        int end = Mathf.Min(startIndex + count, bulletPoints.Count);
        bool first = true;

        for (int i = startIndex; i < end; i++)
        {
            if (string.IsNullOrWhiteSpace(bulletPoints[i])) continue;

            if (!first)
            {
                sb.AppendLine();
            }

            sb.Append(bulletSymbol).Append(bulletPoints[i]);
            first = false;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns all bullet points formatted as a single multi-line string.
    /// </summary>
    public string GetFormattedBulletPoints(string bulletSymbol = "- ")
    {
        return GetFormattedBulletPoints(0, bulletPoints != null ? bulletPoints.Count : 0, bulletSymbol);
    }
}
