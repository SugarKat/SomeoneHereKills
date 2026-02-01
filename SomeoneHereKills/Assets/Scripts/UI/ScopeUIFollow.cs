using UnityEngine;
using UnityEngine.InputSystem;

public class ScopeUIFollow : MonoBehaviour
{
    public RectTransform scopeRoot;
    public Canvas canvas;

    void Reset()
    {
        scopeRoot = (RectTransform)transform;
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (Mouse.current == null || scopeRoot == null || canvas == null) return;

        Camera cam = null;
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera; // MUST be your UICAM when Screen Space - Camera

        Vector2 mouse = Mouse.current.position.ReadValue();

        // Convert screen mouse -> local position in THIS canvas
        RectTransform canvasRect = (RectTransform)canvas.transform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mouse, cam, out Vector2 local);

        scopeRoot.anchoredPosition = local;
    }
}