using UnityEngine;
using UnityEngine.Events;

public class MenuOptions : MonoBehaviour {
    public AmbientInfo firstAmbient;
    PlayerData saveDataLoaded = null;

    [SerializeField] GameObject startTypeContainer;
    [SerializeField] FadeController fade;

    public UnityEvent onNewGameStart;
    public UnityEvent onContinueGameStart;

    public void CallOnNewGameStart() => onNewGameStart?.Invoke();
    public void CallOnContinueGameStart() => onContinueGameStart?.Invoke();

    public void HandleStartGameButton() {
        if (SaveManager.TryGetSaveData(out saveDataLoaded)) {
            startTypeContainer.SetActive(true);
        } else {
            CallOnNewGameStart();
        }
    }

    public void CloseStartTypeContainer() {
        startTypeContainer.SetActive(false);
        saveDataLoaded = null;
    }

    public void RequestNewGame() => fade.FadeToBlack(NewGame);
    void NewGame() {
        SaveManager.ResetSaveData();
        SaveManager.comingFromMenu = true;
        AmbientNavigation.StartAtAmbient(firstAmbient);
    }

    public void RequestContinueGame() => fade.FadeToBlack(ContinueGame);
    void ContinueGame() {
        SaveManager.comingFromMenu = true;
        if (saveDataLoaded != null && saveDataLoaded.playerLocation != "") {
            AmbientInfo ambientInfo = AmbientInfo.GetAmbient(saveDataLoaded.playerLocation);
            if (ambientInfo != null) {
                AmbientNavigation.StartAtAmbient(ambientInfo);
                return;
            }
        }

        AmbientNavigation.StartAtAmbient(firstAmbient);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
