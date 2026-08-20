using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryDeckDisplay : MonoBehaviour, IPointerClickHandler{
    [SerializeField] private HorizontalLayoutGroup hLayoutGroup;
    [SerializeField] private Canvas deckCanvas;
    [SerializeField, Range(-300f, 0f)] private float closedHandSpacing;
    [SerializeField, Range(0f, 500f)] private float openHandSpacing;
    [SerializeField] private float transitionTime;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<GameObject> dummies = new List<GameObject>();
    [SerializeField] private List<CollectableCard> cards = new List<CollectableCard>();
    private Vector2 dummyCardSize;
    private bool openDeck;

    private void Start(){
        dummyCardSize = cardPrefab.GetComponent<RectTransform>().sizeDelta;
        InitializeDeck(InventoryManager.Instance.GetInventory());

        InventoryManager.Instance.OnAddToInventory += AddCard;
        InventoryManager.Instance.OnRemoveFromInventory += RemoveCard;
    }

    private void OnDisable(){
        InventoryManager.Instance.OnAddToInventory -= AddCard;
        InventoryManager.Instance.OnRemoveFromInventory -= RemoveCard;
    }

    public void OnPointerClick(PointerEventData eventData){
        EventSystem.current.SetSelectedGameObject(gameObject);
        if(openDeck) CloseDeck();
        else OpenDeck();
    }

    private void OpenDeck(){
        Debug.Log("Deck opens.");
        openDeck = true;
        hLayoutGroup.spacing = openHandSpacing;
        foreach(var card in cards){
            if(card)card.SetInteractable(true);
        }
    }

    private void CloseDeck(){
        Debug.Log("Deck closes.");
        openDeck = false;
        hLayoutGroup.spacing = closedHandSpacing;
        foreach(var card in cards){
            if(card)card.SetInteractable(false);
        }
    }
    
    public void AddCard(Collectable collectable){
        GameObject newDummyCard = new GameObject("Dummy " + collectable.GetName(), typeof(RectTransform));
        newDummyCard.transform.SetParent(transform);
        RectTransform dummyRect = newDummyCard.GetComponent<RectTransform>();
        dummyRect.sizeDelta = dummyCardSize;
        dummies.Add(newDummyCard);

        CollectableCard card = Instantiate(cardPrefab, deckCanvas.transform).GetComponent<CollectableCard>();
        card.SetUpCard(collectable);
        card.GetComponent<DampedFollower>().SetTarget(newDummyCard.transform);
        cards.Add(card);
        Debug.Log($"New card for {collectable.name} was created succesfully.");
    }

    public void RemoveCard(Collectable collectable){
        for(int i = 0; i < cards.Count; i++){
            if(cards[i].nameID == collectable.GetName()){
                GameObject cardObject, dummyObject;
                cardObject = cards[i].gameObject;
                dummyObject = dummies[i];
                cards.RemoveAt(i);
                dummies.RemoveAt(i);
                Destroy(cardObject);
                Destroy(dummyObject);
                Debug.Log($"Collectable {collectable.GetName()} was removed succesfully.");
                return;
            }
        }
        Debug.Log($"Collectable {collectable.GetName()} couldn't be found and removed.");
    }

    private void InitializeDeck(Collectable[] inventory){
        foreach(var collectable in inventory){
            AddCard(collectable);
        }
        Debug.Log($"Inventory deck initialization has finished.");
    }

    private void ClearDeck(){
        for(int i = 0; i < cards.Count; i++){
            Destroy(cards[i]);
            Destroy(dummies[i]);
        }
        cards.Clear();
        dummies.Clear();
        Debug.Log($"Inventory deck has been completely cleared.");
    }
}
