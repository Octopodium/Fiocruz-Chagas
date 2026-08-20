using UnityEngine;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour, IDragHandler
{
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;

    private void Awake(){
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData){
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

}
