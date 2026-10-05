using UnityEngine;

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

    private void Start()
    {
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
}
