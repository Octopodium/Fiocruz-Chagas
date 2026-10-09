using UnityEngine;
using UnityEngine.UI;

public class MenuOptionsSwitchButton : MonoBehaviour {
    public Image iconImage;
    public Sprite[] sprites;
    public GameObject[] gameObjects;
    int index = 0;

    int nextIndex {
        get {
            if (index + 1 >= sprites.Length) return 0;
            return index + 1;
        }
    }

    void Start() {
        SetIndex(0);
    }

    public void HandleClick() {
        index = (index + 1) % sprites.Length;

        iconImage.sprite = sprites[nextIndex];

        for (int i = 0; i < gameObjects.Length; i++) {
            gameObjects[i].SetActive(i == index);
        }
    }

    public void SetIndex(int i) {
        index = i - 1;
        HandleClick();
    }
}
