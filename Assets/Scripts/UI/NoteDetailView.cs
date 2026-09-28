using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Single reusable detail view panel that displays a note's title, bullet points, image, and page navigation.
/// Bullet points are paginated according to bulletPointsPerPage.
/// </summary>
public class NoteDetailView : MonoBehaviour
{
    [SerializeField] private TMP_Text titleTmpText;
    [SerializeField] private TMP_Text bulletPointsTmpText;
    [SerializeField] private Image noteImage;

    [SerializeField] private int bulletPointsPerPage = 4;

    [SerializeField] private Button prevPageButton;

    [SerializeField] private Button nextPageButton;

    [SerializeField] private Button backButton;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject panelRoot;

    private NoteData currentNote;
    private int currentPageIndex = 0;
    private Action onBackRequested;

    private void Awake()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(HandleBackClick);
        }

        if (prevPageButton != null)
        {
            prevPageButton.onClick.AddListener(PreviousPage);
        }

        if (nextPageButton != null)
        {
            nextPageButton.onClick.AddListener(NextPage);
        }

        if (panelRoot == null)
        {
            panelRoot = gameObject;
        }
    }

    private void OnDestroy()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(HandleBackClick);
        }

        if (prevPageButton != null)
        {
            prevPageButton.onClick.RemoveListener(PreviousPage);
        }

        if (nextPageButton != null)
        {
            nextPageButton.onClick.RemoveListener(NextPage);
        }
    }

    /// <summary>
    /// Displays the specified note data inside the single detail view starting from page 0.
    /// </summary>
    public void DisplayNote(NoteData noteData, Action onBack = null)
    {
        if (noteData == null) return;

        currentNote = noteData;
        currentPageIndex = 0;
        onBackRequested = onBack;

        // Set Title
        if (titleTmpText != null)
        {
            titleTmpText.text = currentNote.Title;
        }

        // Set Image
        SetupImage(currentNote.NoteImage, currentNote.PreserveImageAspect);

        // Render Page Content
        UpdatePageDisplay();

        SetVisible(true);
    }

    /// <summary>
    /// Returns the total number of pages based on bulletPointsPerPage.
    /// </summary>
    private int GetTotalPages()
    {
        if (currentNote == null || currentNote.BulletPoints == null || currentNote.BulletPoints.Count == 0)
        {
            return 1;
        }

        int perPage = Mathf.Max(1, bulletPointsPerPage);
        return Mathf.Max(1, Mathf.CeilToInt((float)currentNote.BulletPoints.Count / perPage));
    }

    /// <summary>
    /// Advances to the next page of bullet points.
    /// </summary>
    public void NextPage()
    {
        if (currentNote == null) return;

        int totalPages = GetTotalPages();
        if (currentPageIndex < totalPages - 1)
        {
            currentPageIndex++;
            UpdatePageDisplay();
        }
    }

    /// <summary>
    /// Goes to the previous page of bullet points.
    /// </summary>
    public void PreviousPage()
    {
        if (currentNote == null) return;

        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageDisplay();
        }
    }

    /// <summary>
    /// Updates bullet points text and page controls for the current page index.
    /// </summary>
    private void UpdatePageDisplay()
    {
        if (currentNote == null) return;

        int totalPages = GetTotalPages();
        int perPage = Mathf.Max(1, bulletPointsPerPage);
        int startIndex = currentPageIndex * perPage;

        // Update Bullets for this page slice
        if (bulletPointsTmpText != null)
        {
            bulletPointsTmpText.text = currentNote.GetFormattedBulletPoints(startIndex, perPage, "- ");
        }

        // Update Pagination Controls
        bool hasMultiplePages = totalPages > 1;

        if (prevPageButton != null)
        {
            prevPageButton.gameObject.SetActive(hasMultiplePages);
            prevPageButton.interactable = currentPageIndex > 0;
        }

        if (nextPageButton != null)
        {
            nextPageButton.gameObject.SetActive(hasMultiplePages);
            nextPageButton.interactable = currentPageIndex < totalPages - 1;
        }
    }

    /// <summary>
    /// Sets up the note image, ensuring correct aspect ratio and scaling.
    /// </summary>
    private void SetupImage(Sprite sprite, bool preserveAspect)
    {
        bool hasSprite = sprite != null;

        if (noteImage != null)
        {
            noteImage.gameObject.SetActive(hasSprite);

            if (hasSprite)
            {
                noteImage.sprite = sprite;
                noteImage.preserveAspect = preserveAspect;
            }
        }
    }

    /// <summary>
    /// Hides the detail view.
    /// </summary>
    public void Hide()
    {
        SetVisible(false);
    }

    /// <summary>
    /// Controls visibility via CanvasGroup or GameObject activation.
    /// </summary>
    public void SetVisible(bool isVisible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
        else if (panelRoot != null)
        {
            panelRoot.SetActive(isVisible);
        }
    }

    private void HandleBackClick()
    {
        Hide();
        onBackRequested?.Invoke();
    }
}
