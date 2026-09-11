using UnityEngine;

public class InnerInteractableMarker : MonoBehaviour
{
    private MaterialPropertyBlock materialPropertyBlock;
    private Renderer render;
    private int fresnelMultiplierHash = Shader.PropertyToID("_FresnelMultiplier");
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

    private void LateUpdate()
    {
        AjustTransparency();
    }
}
