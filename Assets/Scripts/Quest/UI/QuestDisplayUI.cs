using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the current quest and steps.
/// It only listens to QuestControl event's and displays it.
/// </summary>
public class QuestDisplayUI : MonoBehaviour {
    public GameObject questDisplayHolder;
    public Text questTitleText;
    public Text questStepText;

    void Awake() {
        GameManager.instance.player.quest.OnQuestStarted += HandleQuestChanged;
        GameManager.instance.player.quest.OnQuestFinished += HandleQuestEnded;
        GameManager.instance.player.quest.OnQuestStepStarted += UpdateVisual;

        UpdateVisual(GameManager.instance.player.quest.currentQuest, GameManager.instance.player.quest.currentStep);
    }

    void OnDestroy() {
        GameManager.instance.player.quest.OnQuestStarted -= HandleQuestChanged;
        GameManager.instance.player.quest.OnQuestFinished -= HandleQuestEnded;
        GameManager.instance.player.quest.OnQuestStepStarted -= UpdateVisual;
    }

    void HandleQuestChanged(Quest quest) => UpdateVisual(quest, null);
    void HandleQuestEnded(Quest quest) => UpdateVisual(null, null);

    void UpdateVisual(Quest quest, QuestStep step) {
        bool hasQuest = quest != null;
        bool hasQuestStep = step != null;
        
        questDisplayHolder.SetActive(hasQuest);
        questStepText.gameObject.SetActive(hasQuestStep);
        
        if (hasQuest) questTitleText.text = quest.title;
        else questTitleText.text = "";

        if (hasQuestStep) questStepText.text = "- " + step.description;
        else questStepText.text = "";
    }
}
