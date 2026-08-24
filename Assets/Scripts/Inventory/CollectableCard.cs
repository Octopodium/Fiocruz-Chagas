using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(DampedFollower))]
public class CollectableCard : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Color defaultColor, disabledColor;
    public string nameID {get; private set;}
    private DampedFollower dampedFollower;
    private RectTransform rectTransform;
    private bool beingDraged = false;

    private void Awake(){
        SetInteractable(false);
        dampedFollower = GetComponent<DampedFollower>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData){
        if(beingDraged) return;
        Debug.Log($"Asking for notes on {nameText.text}.");
        InventoryManager.Instance.OpenCollectableNote(nameText.text);
    }

    public void OnBeginDrag(PointerEventData eventData){
        dampedFollower.following = false;
        beingDraged = true;
    }

    public void OnDrag(PointerEventData eventData){
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData){
        dampedFollower.following = true;
        beingDraged = false;
    }

    public void SetUpCard(Collectable collectable){
        iconImage.sprite = collectable.GetSprite();
        nameText.text = collectable.GetName();
        nameID = collectable.GetName();
        Debug.Log($"Card for {collectable.GetName()} finished set up.");
    }

    public void SetInteractable(bool interactable){
        backgroundImage.raycastTarget = interactable;
        iconImage.raycastTarget = interactable;
        Debug.Log($"{nameText.text} card has been " + (interactable ? "activated" : "deactivated"));
        backgroundImage.color = interactable ? defaultColor : disabledColor;
    }

}
