using UnityEngine;
using UnityEngine.UI;

public class CardViewManager : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] Text descriptionText, nameText;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start(){
        SetNoteOpen(false);
    }

    public void SetUpNote(Collectable collectable){
        image.sprite = collectable.GetSprite();
        descriptionText.text = collectable.GetDescription();
        nameText.text = collectable.GetName();
        SetNoteOpen(true);
    }

    public void SetNoteOpen(bool isOpen){
        canvasGroup.interactable = isOpen;
        canvasGroup.blocksRaycasts = isOpen;
        canvasGroup.alpha = isOpen ? 1 : 0; 
    }
}
