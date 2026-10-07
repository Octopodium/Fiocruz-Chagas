using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text buttonText;

    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color hoverTextColor = Color.black;

    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private Tween fillTween;
    private Tween textTween;

    private void Awake()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
        }

        if (buttonText != null)
        {
            buttonText.color = normalTextColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Animate(1f, hoverTextColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animate(0f, normalTextColor);
    }

    private void Animate(float targetFill, Color targetColor)
    {
        fillTween?.Kill();
        textTween?.Kill();

        if (fillImage != null)
        {
            fillTween = fillImage.DOFillAmount(targetFill, duration).SetEase(easeType).SetUpdate(true);
        }

        if (buttonText != null)
        {
            textTween = buttonText.DOColor(targetColor, duration).SetEase(easeType).SetUpdate(true);
        }
    }

    private void OnDestroy()
    {
        fillTween?.Kill();
        textTween?.Kill();
    }
}
