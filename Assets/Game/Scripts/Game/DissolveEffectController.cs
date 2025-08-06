using UnityEngine;
using DG.Tweening;

public class DissolveEffectController : MonoBehaviour
{
    private Renderer objRenderer;
    private Material dissolveMaterial;

    void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        dissolveMaterial = objRenderer.material;
    }

    public void StartDissolve()
    {
        dissolveMaterial.DOFloat(1f, "_DissolveThreshold", 1.0f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => gameObject.SetActive(false));
    }

    void OnEnable()
    {
        if (dissolveMaterial != null)
        {
            dissolveMaterial.SetFloat("_DissolveThreshold", 0f);
        }
    }
}
