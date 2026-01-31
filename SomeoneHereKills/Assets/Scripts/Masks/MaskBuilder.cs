using UnityEngine;
using static UnityEngine.Rendering.RayTracingAccelerationStructure;

public class MaskBuilder : MonoBehaviour
{
    public MaskPartLibrary library;
    public MaskConfig config;
    public UniversalMaskSettings settings;
    public Transform maskRoot;

    public SpriteRenderer baseRenderer;
    public SpriteRenderer cheeksRenderer;
    public SpriteRenderer hornsRenderer;
    public SpriteRenderer eyesRenderer;

    private void Start()
    {
        config = MaskRandomizer.Instance.GenerateMask(transform.parent.gameObject);
        Build();
    }

    [ContextMenu("Debug Mask")]
    public void Build()
    {
        if (library == null || settings == null) return;

        maskRoot.localScale = Vector3.one * settings.maskScale;

        // Base
        baseRenderer.sprite = library.bases[(int)config.maskBase];
        baseRenderer.transform.localPosition = settings.baseOffset;

        // Eyes
        eyesRenderer.sprite = library.eyes[(int)config.eyes];
        eyesRenderer.transform.localPosition = settings.eyesOffset;

        // Optional cheeks
        cheeksRenderer.sprite = (config.cheeks == Cheeks.None) ? null : library.cheeks[(int)config.cheeks];
        cheeksRenderer.transform.localPosition = settings.cheeksOffset;

        // Optional horns
        hornsRenderer.sprite = (config.horns == Horns.None) ? null : library.horns[(int)config.horns];
        hornsRenderer.transform.localPosition = settings.hornsOffset;
    }
}