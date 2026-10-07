using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using DG.Tweening;

/// <summary>
/// Controls dialogue visual elements (background image, character portraits, text box)
/// and exposes Yarn Spinner commands to toggle and configure them directly from narrative scripts.
/// </summary>
public class DialogueVisualsController : MonoBehaviour
{
    public static DialogueVisualsController Instance { get; private set; }

    [System.Serializable]
    public struct NamedSprite
    {
        public string id;
        public Sprite sprite;
    }

    [Header("UI Element References")]
    [Tooltip("Background Image behind characters and dialogue.")]
    [SerializeField] private Image backgroundImage;

    [Tooltip("Parent container holding character portraits.")]
    [SerializeField] private GameObject characterArea;

    [Tooltip("Portrait image for Character 1 (e.g. left portrait).")]
    [SerializeField] private Image char1Image;

    [Tooltip("Portrait image for Character 2 (e.g. right portrait).")]
    [SerializeField] private Image char2Image;

    [Tooltip("Background frame/box for dialogue text.")]
    [SerializeField] private GameObject textBox;

    [Header("Sprite Library (Inspector)")]
    [Tooltip("Predefined sprites accessible by identifier in Yarn scripts.")]
    [SerializeField] private List<NamedSprite> spriteLibrary = new List<NamedSprite>();

    [Header("Default Initial Settings")]
    [Tooltip("Whether the background image is visible when dialogue initializes.")]
    [SerializeField] private bool defaultBackgroundActive = false;

    [Tooltip("Whether character portraits container is active by default.")]
    [SerializeField] private bool defaultCharacterAreaActive = false;

    [Tooltip("Whether the text box frame/background is active by default.")]
    [SerializeField] private bool defaultTextBoxActive = true;

    [Tooltip("Reset all visuals back to their default states when dialogue finishes.")]
    [SerializeField] private bool resetVisualsOnDialogueEnd = true;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        AutoFindReferences();
        ResetToDefaults();
    }

    private void Start()
    {
        // Subscribe to dialogue completion event to reset visuals if configured
        if (GameManager.instance != null && GameManager.instance.dialogue != null)
        {
            GameManager.instance.dialogue.onDialogueComplete.AddListener(HandleDialogueComplete);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null && GameManager.instance.dialogue != null)
        {
            GameManager.instance.dialogue.onDialogueComplete.RemoveListener(HandleDialogueComplete);
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Automatically locates UI references if not assigned in the Inspector.
    /// </summary>
    public void AutoFindReferences()
    {
        if (backgroundImage == null)
        {
            Transform bg = transform.Find("BackgroundImage");
            if (bg != null) backgroundImage = bg.GetComponent<Image>();
        }

        if (characterArea == null)
        {
            Transform ca = transform.Find("CharacterArea");
            if (ca != null) characterArea = ca.gameObject;
        }

        if (characterArea != null)
        {
            if (char1Image == null)
            {
                Transform c1 = characterArea.transform.Find("Char1");
                if (c1 != null) char1Image = c1.GetComponent<Image>();
            }

            if (char2Image == null)
            {
                Transform c2 = characterArea.transform.Find("Char2");
                if (c2 != null) char2Image = c2.GetComponent<Image>();
            }
        }

        if (textBox == null)
        {
            Transform ta = transform.Find("TextArea");
            if (ta != null)
            {
                Transform box = ta.Find("Box");
                if (box != null) textBox = box.gameObject;
            }
        }
    }

    /// <summary>
    /// Resets all visual elements to their default states.
    /// </summary>
    public void ResetToDefaults()
    {
        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(defaultBackgroundActive);
        }

        if (characterArea != null)
        {
            characterArea.SetActive(defaultCharacterAreaActive);
        }

        if (textBox != null)
        {
            textBox.SetActive(defaultTextBoxActive);
        }
    }

    private void HandleDialogueComplete()
    {
        if (resetVisualsOnDialogueEnd)
        {
            ResetToDefaults();
        }
    }

    /// <summary>
    /// Enables or disables the screen background image, optionally applying a sprite.
    /// </summary>
    public void SetBackground(bool enabled, string spriteName = "")
    {
        AutoFindReferences();

        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(enabled);

            if (enabled && !string.IsNullOrEmpty(spriteName))
            {
                Sprite s = FindSprite(spriteName);
                if (s != null)
                {
                    backgroundImage.sprite = s;
                }
            }
        }
    }

    /// <summary>
    /// Enables or disables the dialogue text box frame (background).
    /// </summary>
    public void SetTextBox(bool enabled)
    {
        AutoFindReferences();

        if (textBox != null)
        {
            textBox.SetActive(enabled);
        }
    }

    /// <summary>
    /// Enables or disables the character portraits area and optionally sets portraits for Char1 and Char2.
    /// </summary>
    public void SetCharacters(bool enabled, string char1Sprite = "", string char2Sprite = "")
    {
        AutoFindReferences();

        if (characterArea != null)
        {
            characterArea.SetActive(enabled);
        }

        if (!enabled) return;

        if (!string.IsNullOrEmpty(char1Sprite))
        {
            SetChar1(char1Sprite, true);
        }

        if (!string.IsNullOrEmpty(char2Sprite))
        {
            SetChar2(char2Sprite, true);
        }
    }

    /// <summary>
    /// Sets the portrait sprite and visibility for Character 1.
    /// </summary>
    public void SetChar1(string spriteName, bool show = true)
    {
        AutoFindReferences();

        if (char1Image == null) return;

        if (!show || IsEmptyOrNone(spriteName))
        {
            char1Image.gameObject.SetActive(false);
            return;
        }

        Sprite s = FindSprite(spriteName);
        if (s != null)
        {
            char1Image.sprite = s;
            char1Image.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Sets the portrait sprite and visibility for Character 2.
    /// </summary>
    public void SetChar2(string spriteName, bool show = true)
    {
        AutoFindReferences();

        if (char2Image == null) return;

        if (!show || IsEmptyOrNone(spriteName))
        {
            char2Image.gameObject.SetActive(false);
            return;
        }

        Sprite s = FindSprite(spriteName);
        if (s != null)
        {
            char2Image.sprite = s;
            char2Image.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Toggles the character area container active state.
    /// </summary>
    public void SetCharacterArea(bool enabled)
    {
        AutoFindReferences();

        if (characterArea != null)
        {
            characterArea.SetActive(enabled);
        }
    }

    /// <summary>
    /// Resolves a sprite by identifier: first in the inspector SpriteLibrary, then in Resources.
    /// </summary>
    public Sprite FindSprite(string spriteName)
    {
        if (IsEmptyOrNone(spriteName)) return null;

        // 1. Search in Inspector SpriteLibrary
        if (spriteLibrary != null)
        {
            foreach (var entry in spriteLibrary)
            {
                if (entry.sprite == null) continue;

                if (string.Equals(entry.id, spriteName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(entry.sprite.name, spriteName, StringComparison.OrdinalIgnoreCase))
                {
                    return entry.sprite;
                }
            }
        }

        // 2. Search in Resources/Dialogue/<spriteName> or Resources/<spriteName>
        Sprite loaded = Resources.Load<Sprite>("Dialogue/" + spriteName);
        if (loaded == null)
        {
            loaded = Resources.Load<Sprite>(spriteName);
        }

        if (loaded != null)
        {
            return loaded;
        }

        Debug.LogWarning($"[DialogueVisualsController] Sprite '{spriteName}' could not be found in SpriteLibrary or Resources.");
        return null;
    }

    private static bool IsEmptyOrNone(string value)
    {
        return string.IsNullOrEmpty(value) ||
               value.Equals("none", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("off", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("-", StringComparison.OrdinalIgnoreCase);
    }

    private static DialogueVisualsController EnsureInstance()
    {
        if (Instance == null)
        {
            Instance = FindFirstObjectByType<DialogueVisualsController>();
        }
        return Instance;
    }

    /// <summary>
    /// Yarn command: <<set_background <true|false> [sprite_name]>>
    /// Controls screen background visibility and updates its sprite.
    /// </summary>
    [YarnCommand("set_background")]
    public static void YarnSetBackground(bool enabled, string spriteName = "")
    {
        DialogueVisualsController controller = EnsureInstance();
        if (controller != null)
        {
            controller.SetBackground(enabled, spriteName);
        }
        else
        {
            Debug.LogWarning("[DialogueVisualsController] No DialogueVisualsController instance found in the scene.");
        }
    }

    /// <summary>
    /// Yarn command: <<set_text_background <true|false>>>
    /// Controls visibility of the dialogue text box frame/background.
    /// </summary>
    [YarnCommand("set_text_background")]
    public static void YarnSetTextBackground(bool enabled)
    {
        DialogueVisualsController controller = EnsureInstance();
        if (controller != null)
        {
            controller.SetTextBox(enabled);
        }
        else
        {
            Debug.LogWarning("[DialogueVisualsController] No DialogueVisualsController instance found in the scene.");
        }
    }

    /// <summary>
    /// Alias Yarn command: <<set_textbox <true|false>>>
    /// </summary>
    [YarnCommand("set_textbox")]
    public static void YarnSetTextBox(bool enabled)
    {
        YarnSetTextBackground(enabled);
    }

    /// <summary>
    /// Yarn command: <<set_characters <true|false> [char1_sprite] [char2_sprite]>>
    /// Controls character portraits area and assigns sprites to Char1 and Char2.
    /// </summary>
    [YarnCommand("set_characters")]
    public static void YarnSetCharacters(bool enabled, string char1Sprite = "", string char2Sprite = "")
    {
        DialogueVisualsController controller = EnsureInstance();
        if (controller != null)
        {
            controller.SetCharacters(enabled, char1Sprite, char2Sprite);
        }
        else
        {
            Debug.LogWarning("[DialogueVisualsController] No DialogueVisualsController instance found in the scene.");
        }
    }

    /// <summary>
    /// Yarn command: <<set_character_area <true|false>>>
    /// Controls character area container visibility.
    /// </summary>
    [YarnCommand("set_character_area")]
    public static void YarnSetCharacterArea(bool enabled)
    {
        DialogueVisualsController controller = EnsureInstance();
        if (controller != null)
        {
            controller.SetCharacterArea(enabled);
        }
        else
        {
            Debug.LogWarning("[DialogueVisualsController] No DialogueVisualsController instance found in the scene.");
        }
    }

    /// <summary>
    /// Yarn command: <<set_char1 <sprite_name> [show]>>
    /// Assigns sprite and updates visibility for Character 1.
    /// </summary>
    [YarnCommand("set_char1")]
    public static void YarnSetChar1(string spriteName, bool show = true)
    {
        DialogueVisualsController controller = EnsureInstance();
        if (controller != null)
        {
            controller.SetChar1(spriteName, show);
        }
        else
        {
            Debug.LogWarning("[DialogueVisualsController] No DialogueVisualsController instance found in the scene.");
        }
    }

    /// <summary>
    /// Yarn command: <<set_char2 <sprite_name> [show]>>
    /// Assigns sprite and updates visibility for Character 2.
    /// </summary>
    [YarnCommand("set_char2")]
    public static void YarnSetChar2(string spriteName, bool show = true)
    {
        DialogueVisualsController controller = EnsureInstance();
        if (controller != null)
        {
            controller.SetChar2(spriteName, show);
        }
        else
        {
            Debug.LogWarning("[DialogueVisualsController] No DialogueVisualsController instance found in the scene.");
        }
    }

    /// <summary>
    /// Yarn command: <<fade_bg_image <target_alpha> [duration]>>
    /// </summary>
    [YarnCommand("fade_bg_image")]
    public static void YarnFadeBgImage(float targetAlpha, float duration = 1f)
    {
        DialogueVisualsController controller = EnsureInstance();
        if (controller == null || controller.backgroundImage == null)
        {
            Debug.LogWarning("[DialogueVisualsController] Instância ou Background Image não encontrados.");
            return;
        }

        Image bg = controller.backgroundImage;

        if (targetAlpha > 0f && !bg.gameObject.activeSelf)
        {
            Color c = bg.color;
            c.a = 0f;
            bg.color = c;
            bg.gameObject.SetActive(true);
        }

        bg.DOFade(targetAlpha, duration).OnComplete(() => 
        {
            if (targetAlpha <= 0f)
            {
                bg.gameObject.SetActive(false);
            }
        });
    }
}
