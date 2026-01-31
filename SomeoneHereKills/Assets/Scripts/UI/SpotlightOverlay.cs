using UnityEngine;

public class SpotlightOverlay : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform canvasRect;
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform left;
    public RectTransform right;

    [Header("Optional")]
    public Vector2 padding = new Vector2(12, 12);

    [Tooltip("Pixels to overlap panels to avoid 1px seams.")]
    public float overlapPx = 2f;

    public void FocusUI(RectTransform target)
    {
        if (!target) return;

        // Ensure layout is up-to-date (critical for correct corners)
        Canvas.ForceUpdateCanvases();

        Vector3[] wc = new Vector3[4];
        target.GetWorldCorners(wc);

        Vector2 min, max;
        WorldCornersToCanvasMinMax(wc, out min, out max);

        min -= padding;
        max += padding;

        ApplyHoleLocal(min, max);
    }

    public void FocusWorld(Camera cam, Vector3 worldPos, Vector2 holeSizePx)
    {
        if (!cam) return;

        Canvas.ForceUpdateCanvases();

        Vector3 sp = cam.WorldToScreenPoint(worldPos);
        if (sp.z < 0f)
        {
            Hide();
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, sp, null, out var centerLocal);

        Vector2 half = holeSizePx * 0.5f;
        Vector2 min = centerLocal - half - padding;
        Vector2 max = centerLocal + half + padding;

        ApplyHoleLocal(min, max);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void WorldCornersToCanvasMinMax(Vector3[] worldCorners, out Vector2 min, out Vector2 max)
    {
        min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        for (int i = 0; i < 4; i++)
        {
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(null, worldCorners[i]);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screen, null, out var local);

            min = Vector2.Min(min, local);
            max = Vector2.Max(max, local);
        }
    }

    void ApplyHoleLocal(Vector2 min, Vector2 max)
    {
        var r = canvasRect.rect;

        // Hole bounds in canvas local space
        float holeLeft = Mathf.Clamp(min.x, r.xMin, r.xMax);
        float holeRight = Mathf.Clamp(max.x, r.xMin, r.xMax);
        float holeBottom = Mathf.Clamp(min.y, r.yMin, r.yMax);
        float holeTop = Mathf.Clamp(max.y, r.yMin, r.yMax);

        // --- Pixel snapping (THIS fixes the 1px gaps) ---
        // Expand outward: left/bottom -> floor, right/top -> ceil
        holeLeft = Mathf.Floor(holeLeft);
        holeBottom = Mathf.Floor(holeBottom);
        holeRight = Mathf.Ceil(holeRight);
        holeTop = Mathf.Ceil(holeTop);

        // --- Force overlap so panels always touch/overlap ---
        float o = Mathf.Max(0f, overlapPx);   // set overlapPx = 6..10 for brute-force
        holeLeft = Mathf.Clamp(holeLeft - o, r.xMin, r.xMax);
        holeRight = Mathf.Clamp(holeRight + o, r.xMin, r.xMax);
        holeBottom = Mathf.Clamp(holeBottom - o, r.yMin, r.yMax);
        holeTop = Mathf.Clamp(holeTop + o, r.yMin, r.yMax);

        float canvasW = r.width;

        // Heights
        float topH = Mathf.Max(0f, r.yMax - holeTop);
        float bottomH = Mathf.Max(0f, holeBottom - r.yMin);
        float midH = Mathf.Max(0f, holeTop - holeBottom);

        // Widths
        float leftW = Mathf.Max(0f, holeLeft - r.xMin);
        float rightW = Mathf.Max(0f, r.xMax - holeRight);

        // Snap sizes to ints too (prevents fractional seams)
        topH = Mathf.Ceil(topH);
        bottomH = Mathf.Ceil(bottomH);
        midH = Mathf.Ceil(midH);
        leftW = Mathf.Ceil(leftW);
        rightW = Mathf.Ceil(rightW);

        // Positions (center anchored/pivot)
        SetSize(top, canvasW, topH);
        SetPos(top, 0f, Mathf.Round((holeTop + r.yMax) * 0.5f));

        SetSize(bottom, canvasW, bottomH);
        SetPos(bottom, 0f, Mathf.Round((r.yMin + holeBottom) * 0.5f));

        SetSize(left, leftW, midH);
        SetPos(left, Mathf.Round((r.xMin + holeLeft) * 0.5f), Mathf.Round((holeBottom + holeTop) * 0.5f));

        SetSize(right, rightW, midH);
        SetPos(right, Mathf.Round((holeRight + r.xMax) * 0.5f), Mathf.Round((holeBottom + holeTop) * 0.5f));

        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    static void SetSize(RectTransform rt, float w, float h)
    {
        rt.sizeDelta = new Vector2(w, h);
    }

    static void SetPos(RectTransform rt, float x, float y)
    {
        rt.anchoredPosition = new Vector2(x, y);
    }
}
