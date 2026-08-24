using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance { get; private set; }

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text characterNameText;
    public Image background;
    [SerializeField] private float textSpeed = 0.05f;

    [Header("VN Elements")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public Image[] characterImages;
    // Fade
    public CanvasGroup backgroundCanvasGroup;
    public CanvasGroup characterCanvasGroup;
    public Image fadeOverlay;
    // Choices
    public Transform choiceButtonContainer;
    public GameObject choiceButtonPrefab;

    // Action event
    public System.Action<string, string> OnCommandTriggered;

    // Wobble text animation
    public MaybeWobble maybeWobble;

    private DialogueContainer currentDialogue;
    private DialogueNodeData currentNode;
    private bool isTyping = false;
    private string fullText = "";
    private Dictionary<string, DialogueNodeData> nodeLookup;

    // Targeted coroutine
    private Coroutine transitionCoroutine;
    private Coroutine typingCoroutine;

    // State/global variable
    private Dictionary<string, bool> globalConditions = new Dictionary<string, bool>();

    // Character tracking
    private string currentSpeakerName = "";
    private Dictionary<string, int> characterSideMap = new Dictionary<string, int>();
    private Sprite[] activeSpritesBySide;
    private string[] sideToCharacterMap;
    private Coroutine characterStatesCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCondition(string variableName, bool value)
    {
        if (string.IsNullOrEmpty(variableName)) return;
        globalConditions[variableName] = value;
    }

    public bool CheckCondition(string variableName)
    {
        if (string.IsNullOrEmpty(variableName)) return false;
        if (globalConditions.TryGetValue(variableName, out bool val))
        {
            return val;
        }
        return false;
    }

    public void StartDialogue(DialogueContainer dialogue)
    {
        currentDialogue = dialogue;
        BuildNodeLookup();

        currentSpeakerName = "";
        characterSideMap.Clear();
        int imageCount = (characterImages != null) ? characterImages.Length : 0;
        activeSpritesBySide = new Sprite[imageCount];
        sideToCharacterMap = new string[imageCount];

        if (characterStatesCoroutine != null)
        {
            StopCoroutine(characterStatesCoroutine);
            characterStatesCoroutine = null;
        }

        if (characterImages != null)
        {
            for (int i = 0; i < characterImages.Length; i++)
            {
                if (characterImages[i] != null)
                {
                    characterImages[i].gameObject.SetActive(false);
                }
            }
        }

        if (characterCanvasGroup != null)
        {
            characterCanvasGroup.alpha = 0f;
        }

        var entryLink = dialogue.NodeLinks.Find(x => x.portName == "Next");
        if (entryLink != null && nodeLookup.ContainsKey(entryLink.targetNodeGuid))
        {
            dialoguePanel.SetActive(true);
            ProceedToNode(nodeLookup[entryLink.targetNodeGuid]);
        }
        else
        {
            Debug.LogError("Dialogue Graph entry link not found or target is invalid!");
        }
    }

    private void BuildNodeLookup()
    {
        nodeLookup = new Dictionary<string, DialogueNodeData>();
        foreach (var node in currentDialogue.DialogueNodeData)
        {
            nodeLookup[node.Guid] = node;
        }
    }

    private void ProceedToNode(DialogueNodeData node)
    {
        currentNode = node;

        if (!string.IsNullOrEmpty(currentNode.commandString))
        {
            ExecuteCommand(currentNode.commandString);
        }

        if (currentNode.nodeType == NodeType.Condition)
        {
            bool result = CheckCondition(currentNode.conditionVariableName);
            string portToFollow = result ? "True" : "False";

            var link = currentDialogue.NodeLinks.Find(x => x.baseNodeGuid == currentNode.Guid && x.portName == portToFollow);
            if (link != null && nodeLookup.ContainsKey(link.targetNodeGuid))
            {
                ProceedToNode(nodeLookup[link.targetNodeGuid]);
            }
            else
            {
                EndDialogue();
            }
        }
        else if (currentNode.nodeType == NodeType.End)
        {
            EndDialogue();
        }
        else
        {
            ClearChoiceButtons();
            
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
                transitionCoroutine = null;
            }
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            transitionCoroutine = StartCoroutine(PerformTransitionsAndShowText());
        }
    }

    // Audio 
    private void PlayAudioForCurrentNode()
    {
        if (bgmSource != null && currentNode.bgmAudio != null)
        {
            if (bgmSource.clip != currentNode.bgmAudio)
            {
                bgmSource.clip = currentNode.bgmAudio;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }

        if (sfxSource != null && currentNode.sfxAudio != null)
        {
            sfxSource.PlayOneShot(currentNode.sfxAudio);
        }
    }

    // Character methods
    private void SetupCharacters()
    {
        if (characterImages == null || characterImages.Length == 0) return;

        bool hasSpeakerName = !string.IsNullOrEmpty(currentNode.characterName);
        bool hasCharacterSprite = currentNode.characterSprites != null && 
                                  currentNode.characterSprites.Count > 0 && 
                                  currentNode.characterSprites[0] != null;

        bool shouldUpdateSituation = hasSpeakerName && hasCharacterSprite;

        if (shouldUpdateSituation)
        {
            currentSpeakerName = currentNode.characterName;

            int speakerSideIndex = -1;
            if (characterSideMap.TryGetValue(currentSpeakerName, out int existingSide))
            {
                speakerSideIndex = existingSide;
            }
            else
            {
                for (int i = 0; i < characterImages.Length; i++)
                {
                    if (string.IsNullOrEmpty(sideToCharacterMap[i]))
                    {
                        speakerSideIndex = i;
                        break;
                    }
                }

                if (speakerSideIndex == -1)
                {
                    for (int i = 0; i < characterImages.Length; i++)
                    {
                        if (characterImages[i] != null)
                        {
                            speakerSideIndex = i;
                            break;
                        }
                    }
                }

                characterSideMap[currentSpeakerName] = speakerSideIndex;
                sideToCharacterMap[speakerSideIndex] = currentSpeakerName;
            }

            activeSpritesBySide[speakerSideIndex] = currentNode.characterSprites[0];
            UpdateCharacterVisuals(speakerSideIndex, currentNode.characterFadeTime * 0.5f);
        }
        else
        {
            int speakerSideIndex = -1;
            if (!string.IsNullOrEmpty(currentSpeakerName))
            {
                characterSideMap.TryGetValue(currentSpeakerName, out speakerSideIndex);
            }
            UpdateCharacterVisuals(speakerSideIndex, currentNode.characterFadeTime * 0.5f);
        }
    }

    private void UpdateCharacterVisuals(int speakerSideIndex, float duration)
    {
        if (characterStatesCoroutine != null)
        {
            StopCoroutine(characterStatesCoroutine);
        }
        characterStatesCoroutine = StartCoroutine(AnimateCharacterVisualsCoroutine(speakerSideIndex, duration));
    }

    // Scale effect on character switch
    private IEnumerator AnimateCharacterVisualsCoroutine(int speakerSideIndex, float duration)
    {
        int count = characterImages.Length;
        Vector3[] startScales = new Vector3[count];
        float[] startAlphas = new float[count];
        Vector3[] targetScales = new Vector3[count];
        float[] targetAlphas = new float[count];

        for (int i = 0; i < count; i++)
        {
            if (characterImages[i] == null) continue;

            startScales[i] = characterImages[i].transform.localScale;
            startAlphas[i] = characterImages[i].color.a;

            if (activeSpritesBySide[i] != null)
            {
                characterImages[i].sprite = activeSpritesBySide[i];
                if (!characterImages[i].gameObject.activeSelf)
                {
                    characterImages[i].gameObject.SetActive(true);
                    startAlphas[i] = 0f;
                    characterImages[i].color = new Color(characterImages[i].color.r, characterImages[i].color.g, characterImages[i].color.b, 0f);
                    startScales[i] = Vector3.one;
                }

                if (i == speakerSideIndex)
                {
                    targetScales[i] = new Vector3(1.1f, 1.1f, 1.1f);
                    targetAlphas[i] = 1.0f;
                }
                else
                {
                    targetScales[i] = new Vector3(1.0f, 1.0f, 1.0f);
                    targetAlphas[i] = 0.7f;
                }
            }
            else
            {
                targetScales[i] = Vector3.one;
                targetAlphas[i] = 0f;
            }
        }

        if (duration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                for (int i = 0; i < count; i++)
                {
                    if (characterImages[i] == null) continue;

                    characterImages[i].transform.localScale = Vector3.Lerp(startScales[i], targetScales[i], t);
                    Color c = characterImages[i].color;
                    characterImages[i].color = new Color(c.r, c.g, c.b, Mathf.Lerp(startAlphas[i], targetAlphas[i], t));
                }

                yield return null;
            }
        }

        for (int i = 0; i < count; i++)
        {
            if (characterImages[i] == null) continue;

            characterImages[i].transform.localScale = targetScales[i];
            Color c = characterImages[i].color;
            characterImages[i].color = new Color(c.r, c.g, c.b, targetAlphas[i]);

            if (activeSpritesBySide[i] == null)
            {
                characterImages[i].gameObject.SetActive(false);
            }
        }

        characterStatesCoroutine = null;
    }

    // Fade effect on character switch
    private IEnumerator PerformTransitionsAndShowText()
    {
        isTyping = false;

        PlayAudioForCurrentNode();

        float charFadeTime = currentNode.characterFadeTime;

        if (currentNode.backgroundSprite != null && background != null)
        {
            background.sprite = currentNode.backgroundSprite;
        }
        SetupCharacters();

        if (characterCanvasGroup != null && charFadeTime > 0f && characterCanvasGroup.alpha < 1f)
        {
            float startAlpha = characterCanvasGroup.alpha;
            float elapsed = 0f;
            while (elapsed < charFadeTime)
            {
                elapsed += Time.deltaTime;
                characterCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / charFadeTime);
                yield return null;
            }
            characterCanvasGroup.alpha = 1f;
        }
        else if (characterCanvasGroup != null)
        {
            characterCanvasGroup.alpha = 1f;
        }

        if (characterNameText != null)
        {
            characterNameText.text = currentSpeakerName;
        }

        fullText = currentNode.dialogueText ?? "";
        typingCoroutine = StartCoroutine(TypeSentence(fullText));
        yield return typingCoroutine;
        typingCoroutine = null;

        ShowChoices();
        transitionCoroutine = null;
    }
    //
       
    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        int index = 0;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            if (index > 0 && maybeWobble != null)
            {
                yield return null;
                maybeWobble.StartCoroutine(maybeWobble.AnimateWobbleChar(dialogueText));
            }
            index++;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    private void CompleteSentence()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = fullText;
        isTyping = false;

        ShowChoices();
    }

    public void AdvanceToNextNode()
    {
        if (isTyping)
        {
            CompleteSentence();
            return;
        }

        var links = currentDialogue.NodeLinks.FindAll(x => x.baseNodeGuid == currentNode.Guid);
        bool hasChoices = links.Count > 1 || (links.Count == 1 && links[0].portName != "Next" && links[0].portName != "output");

        if (hasChoices) 
        {
            return;
        }

        if (links.Count == 1)
        {
            var link = links[0];
            if (nodeLookup.ContainsKey(link.targetNodeGuid))
            {
                ProceedToNode(nodeLookup[link.targetNodeGuid]);
                return;
            }
            else
            {
            }
        }

        EndDialogue();
    }

    // Choices methods
    private void ShowChoices()
    {
        ClearChoiceButtons();

        var links = currentDialogue.NodeLinks.FindAll(x => x.baseNodeGuid == currentNode.Guid);
        bool hasChoices = links.Count > 1 || (links.Count == 1 && links[0].portName != "Next" && links[0].portName != "output");

        if (hasChoices && choiceButtonContainer != null && choiceButtonPrefab != null)
        {
            choiceButtonContainer.gameObject.SetActive(true);
            foreach (var link in links)
            {
                var btnObj = Instantiate(choiceButtonPrefab, choiceButtonContainer);
                var textMesh = btnObj.GetComponentInChildren<TMP_Text>();
                if (textMesh != null)
                {
                    textMesh.text = link.portName;
                }

                var button = btnObj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => {
                        SelectChoice(link);
                    });
                }
            }
        }
    }

    private void SelectChoice(NodeLinkData link)
    {
        ClearChoiceButtons();
        if (nodeLookup.ContainsKey(link.targetNodeGuid))
        {
            ProceedToNode(nodeLookup[link.targetNodeGuid]);
        }
        else
        {
            EndDialogue();
        }
    }

    private void ClearChoiceButtons()
    {
        if (choiceButtonContainer != null)
        {
            foreach (Transform child in choiceButtonContainer)
            {
                Destroy(child.gameObject);
            }
            choiceButtonContainer.gameObject.SetActive(false);
        }
    }
    //

    private void EndDialogue()
    {
        if (currentNode != null && currentNode.nodeType == NodeType.End)
        {
            if (currentNode._loadScene && !string.IsNullOrEmpty(currentNode.sceneName))
            {
                SceneManager.LoadScene(currentNode.sceneName);
                return;
            }
        }

        dialoguePanel.SetActive(false);

        if (characterImages != null)
        {
            for (int i = 0; i < characterImages.Length; i++)
            {
                if (characterImages[i] != null)
                {
                    characterImages[i].gameObject.SetActive(false);
                }
            }
        }

        if (characterStatesCoroutine != null)
        {
            StopCoroutine(characterStatesCoroutine);
            characterStatesCoroutine = null;
        }
    }

    // End node event
    private void ExecuteCommand(string command)
    {
        if (string.IsNullOrEmpty(command)) return;

        string[] parts = command.Split(':');
        string action = parts[0].Trim();
        string parameter = parts.Length > 1 ? parts[1].Trim() : "";

        switch (action.ToLower())
        {
            case "loadscene":
                SceneManager.LoadScene(parameter);
                break;
            case "unlock":
                SetCondition(parameter, true);
                break;
            default:
                OnCommandTriggered?.Invoke(action, parameter);
                break;
        }
    }
}