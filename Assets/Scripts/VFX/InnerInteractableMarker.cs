using UnityEngine;

public class InnerInteractableMarker : MonoBehaviour
{
    private MaterialPropertyBlock materialPropertyBlock;
    private Renderer render;
    private readonly int fresnelMultiplierHash = Shader.PropertyToID("_FresnelMultiplier");
    private readonly int ColorHash = Shader.PropertyToID("_BaseColor");
    [SerializeField] private float transparency;

    private void Awake()
    {
        render = GetComponent<Renderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void AjustTransparency()
    {
        materialPropertyBlock.SetFloat(fresnelMultiplierHash, transparency);
        render.SetPropertyBlock(materialPropertyBlock);
    }

    public void AjustColor(Color color)
    {
        materialPropertyBlock.SetColor(ColorHash, color);
        render.SetPropertyBlock(materialPropertyBlock);
    }

    private void LateUpdate()
    {
        AjustTransparency();
    }
}
