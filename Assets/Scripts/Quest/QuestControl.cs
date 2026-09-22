using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Responsible to control Quests and it's QuestSteps.  
/// </summary>
public class QuestControl : MonoBehaviour, ISaveable {
    public Quest startingQuest;
    public System.Action<Quest> OnQuestStarted, OnQuestFinished;
    public System.Action<Quest,QuestStep> OnQuestStepStarted, OnQuestStepEnded;

    // Internal
    public Quest currentQuest {get; protected set;}
    public QuestStep currentStep {get; protected set;}
    HashSet<string> finishedQuests = new HashSet<string>();
    int questStepIndex = -1;

    void Awake() {
        GameManager.instance.saveManager.AddSaveable(this);
    }

    void OnDestroy() {
        GameManager.instance?.saveManager.RemoveSaveable(this);
    }

    void Start() {
        if (startingQuest != null)
            StartQuest(startingQuest);
    }

    /// <summary>
    /// Starts a quest (and it's first step). If another quest happening, force stops it.
    /// </summary>
    /// <param name="questToStart">The quest to be started</param>
    /// <param name="startFromIndex">Optional parameter. Start quest from step with index. Most times leave it -1 or empty. Useful set for Load data.</param>
    public void StartQuest(Quest questToStart, int startFromIndex = 0) {
        if (finishedQuests.Contains(questToStart.name)) return;

        if (currentQuest == questToStart) return;
        else if (currentQuest != null) ForceStopQuest(currentQuest, false);

        currentQuest = questToStart;

        OnQuestStarted?.Invoke(currentQuest);

        questStepIndex = startFromIndex - 1;
        currentStep = null;
        NextStep();
    }

    /// <summary>
    /// Internal use only. Called once on StartQuest, and called by the currentStep's OnFinished.
    /// Every NextStep iterates the 'questStepIndex' and changes the 'currentStep'. If on end, calls StopQuest.
    /// </summary>
    void NextStep() {
        if (currentQuest == null) return;

        if (currentStep != null) {
            currentStep.OnFinished -= NextStep;
            OnQuestStepEnded?.Invoke(currentQuest, currentStep);
            currentStep = null;
        }
        
        if (questStepIndex >= currentQuest.steps.Length - 1) {
            StopQuest();
            return;
        }

        questStepIndex += 1;
        currentStep = currentQuest.steps[questStepIndex];
        currentStep.OnFinished += NextStep;
        OnQuestStepStarted?.Invoke(currentQuest, currentStep);
        currentStep.Start();
    }

    /// <summary>
    /// Internal use only. Called by NextStep when there are no more steps left, or called by ForceStopQuest.
    /// Stops the current quest and starts the 'unlockNextQuest' if able to.
    /// </summary>
    /// <param name="canCallNextQuest">If the current quest has 'unlockNextQuest', this parameter defines if it can be auto-started.</param>
    void StopQuest(bool canCallNextQuest = true) {
        questStepIndex = -1;
        currentStep = null;

        OnQuestFinished?.Invoke(currentQuest);

        Quest lastQuest = currentQuest;
        currentQuest = null;

        finishedQuests.Add(lastQuest.name);

        if (canCallNextQuest && lastQuest.unlockNextQuest != null) {
            StartQuest(lastQuest.unlockNextQuest);
        }

    }

    /// <summary>
    /// Forces the current quest to stop completly. It will be considered finished.
    /// </summary>
    /// <param name="questToForceStop">The quest to stop completly. If different of currentQuest, does nothing.</param>
    /// <param name="canCallNextQuest">If the quest to stop has 'unlockNextQuest', this parameter defines if it can be auto-started.</param>
    public void ForceStopQuest(Quest questToForceStop, bool canCallNextQuest = true) {
        if (currentQuest != questToForceStop) return;

        if (currentStep != null) {
            currentStep.Stop();
            currentStep.OnFinished -= NextStep;
            currentStep = null;
        }

        StopQuest(canCallNextQuest);
    }


    
    void Update() {
        if (currentStep != null) currentStep.HandleUpdate();
    }

    void FixedUpdate() {
        if (currentStep != null) currentStep.HandleFixedUpdate();
    }


    public PlayerData Save(PlayerData data) {
        QuestSaveData questData = new QuestSaveData();
        questData.currentQuest = currentQuest != null ? currentQuest.name : "";
        questData.currentStep = questStepIndex;
        questData.finishedQuests = finishedQuests.ToArray();

        data.quests = questData;
        return data;
    }

    public void Load(PlayerData data) {
        QuestSaveData questData = data.quests;

        finishedQuests = questData.finishedQuests.ToHashSet();
        questStepIndex = questData.currentStep;

        bool dataHasCurrentQuest = !string.IsNullOrEmpty(questData.currentQuest);

        if (currentQuest != null) {
            Quest runningNow = currentQuest;

            if (dataHasCurrentQuest && runningNow.name == questData.currentQuest) {
                if (questStepIndex == questData.currentStep) return;
                currentStep.Stop();
                currentStep.OnFinished -= NextStep;
                currentStep = null;
                questStepIndex = questData.currentStep - 1;
                NextStep();
                return;
            }

            bool wasFinished = finishedQuests.Contains(runningNow.name);
            ForceStopQuest(currentQuest, false);
            if (!wasFinished) finishedQuests.Remove(runningNow.name);
            runningNow = null;
        }

        if (dataHasCurrentQuest) {
            Quest toStart = Quest.GetQuestByName(questData.currentQuest);
            StartQuest(toStart, Mathf.Max(0, questData.currentStep));
        }
    }

}

[Serializable]
public struct QuestSaveData {
    public string currentQuest;
    public int currentStep;
    public string[] finishedQuests;
}