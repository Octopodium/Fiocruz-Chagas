using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractableMarker : MonoBehaviour
{
    [SerializeField] private Color defaultColor, highlightedColor, selectedColor;
    [SerializeField] private MaterialPropertyBlock mpb;
    [SerializeField] private readonly int colorHash = Shader.PropertyToID("_BaseColor");
    [SerializeField] private Renderer render;
    [SerializeField] private InnerInteractableMarker innerMarker;

    private void Awake()
    {
        render = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    void Start()
    {
        ChangeColor(defaultColor);
        GambiarrandoPos();
    }

    public void ChangeColor(Color color)
    {
        mpb.SetColor(colorHash, color);
        render.SetPropertyBlock(mpb);
        innerMarker.AjustColor(color);
    }

    private void AlternateColor()
    {
        Color newCol = new Color(Random.Range(0f,1f), Random.Range(0f,1.0f), Random.Range(0.0f,1.0f));
        Debug.Log(newCol);
        ChangeColor(newCol);
    }

    private void GambiarrandoPos()
    {
        BoxCollider box;
        if(box = GetComponentInParent<BoxCollider>())
        {
            transform.localPosition = box.center;
            transform.localScale = box.size;
        }
    }


    IUnderMouse[] interactables;
    Awaitable selectedAwaitable;

    public void SetupInteractables() {
        interactables = transform.parent.GetComponents<IUnderMouse>();
        foreach (IUnderMouse interactable in interactables) {
            interactable.OnHover += HandleInteractableHoverChanged;
            interactable.OnInteracted += HandleInteractableInteracted;
        }
    }

    public void RefreshVisibility(bool isInteractable) {
        foreach (IUnderMouse interactable in interactables) {
            if (!interactable.CanBeFound() || !interactable.CheckConditions() || !GameManager.instance.player.CanInteract()) continue;

            if ((interactable is IInteractable && isInteractable) || !(interactable is IInteractable || isInteractable) ) {
                SetVisibility(true);
                return;
            }
        }

        SetVisibility(false);
    }

    void SetVisibility(bool visible) {
        gameObject.SetActive(visible);
    }

    void HandleInteractableHoverChanged(bool entered) {
        if (selectedAwaitable != null) return;

        ChangeColor(entered? highlightedColor : defaultColor);
    }

    bool ContainsInteractable(IUnderMouse interactable) {
        foreach (IUnderMouse i in interactables) {
            if (i == interactable) return true;
        }

        return false;
    }
    
    void HandleInteractableInteracted() {
        if (!gameObject.activeInHierarchy) return;
        if (selectedAwaitable != null) selectedAwaitable.Cancel();

        ChangeColor(selectedColor);
        InteractedEffectTimer(1.0f);
    }

    async void InteractedEffectTimer(float waitFor) {
        selectedAwaitable = Awaitable.WaitForSecondsAsync(waitFor);
        await selectedAwaitable;
        selectedAwaitable = null;

        if (this != null)
            HandleInteractableHoverChanged(ContainsInteractable(GameManager.instance.player.currentInteractable));
    }
}
