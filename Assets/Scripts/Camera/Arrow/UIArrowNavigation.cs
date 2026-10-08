using UnityEngine;
using Yarn.Unity;

public class UIArrowNavigation : MonoBehaviour {
    public GameObject upButton, downButton, leftButton, rightButton;

    public void GoUp() => GameManager.instance.cam.currentCameraArea?.navigationArrows.up.Run();
    public void GoDown() => GameManager.instance.cam.currentCameraArea?.navigationArrows.down.Run();
    public void GoLeft() => GameManager.instance.cam.currentCameraArea?.navigationArrows.left.Run();
    public void GoRight() => GameManager.instance.cam.currentCameraArea?.navigationArrows.right.Run();

    void Awake() {
        GameManager.instance.cam.onCurrentCameraAreaChange += RefreshVisual;
        GameManager.instance.inspectator.OnInspectingChanged += SetActiveRedirect;

        GameManager.instance.dialogue.onDialogueStart.AddListener(SetActiveFalse);
        GameManager.instance.dialogue.onDialogueComplete.AddListener(SetActiveTrue);
    }

    void OnDestroy() {
        if (GameManager.exists) {
            GameManager.instance.cam.onCurrentCameraAreaChange -= RefreshVisual;
            GameManager.instance.inspectator.OnInspectingChanged -= SetActiveRedirect;

            GameManager.instance.dialogue.onDialogueStart.RemoveListener(SetActiveFalse);
            GameManager.instance.dialogue.onDialogueComplete.RemoveListener(SetActiveTrue);
        }
    }

    void Start() {
        RefreshVisual(GameManager.instance.cam.currentCameraArea);
    }

    public void RefreshVisual(CameraArea area) {
        if (area == null) {
            upButton.SetActive(false);
            downButton.SetActive(false);
            leftButton.SetActive(false);
            rightButton.SetActive(false);
            return;
        }

        AreaArrows areaArrows = area.navigationArrows;
        upButton.SetActive(areaArrows.up.option != ArrowOptions.NoArrow);
        downButton.SetActive(areaArrows.down.option != ArrowOptions.NoArrow);
        leftButton.SetActive(areaArrows.left.option != ArrowOptions.NoArrow);
        rightButton.SetActive(areaArrows.right.option != ArrowOptions.NoArrow);
    }

    public void SetActiveArrows(bool active) {
        gameObject.SetActive(active);
    }

    void SetActiveRedirect(Inspectable inspectable) => SetActiveArrows(inspectable == null);
    void SetActiveTrue() => SetActiveArrows(true);
    void SetActiveFalse() => SetActiveArrows(false);
}
