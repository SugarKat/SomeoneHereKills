using UnityEngine;
using UnityEngine.InputSystem;

public class ScopeCamera2_5D : MonoBehaviour
{
    public Camera worldCamera;
    public Camera scopeCamera;
    [Header("Zoom")]
    public float zoomFactor = 3f;

    public Vector3 planeNormal = new Vector3(0, 1, 0);
    public float planeOffset = 0f;

    void LateUpdate()
    {
        if (Mouse.current == null || worldCamera == null || scopeCamera == null) return;

        float effectiveAspect = worldCamera.pixelRect.width / worldCamera.pixelRect.height;
        scopeCamera.aspect = effectiveAspect;


        scopeCamera.orthographicSize = worldCamera.orthographicSize / zoomFactor;

        Vector2 mouse = Mouse.current.position.ReadValue();
        Vector3 vp = worldCamera.ScreenToViewportPoint(new Vector3(mouse.x, mouse.y, 0f));
        vp.x = Mathf.Clamp01(vp.x);
        vp.y = Mathf.Clamp01(vp.y);

        Ray ray = worldCamera.ViewportPointToRay(new Vector3(vp.x, vp.y, 0f));

        Plane plane = new Plane(planeNormal.normalized, planeNormal.normalized * planeOffset);

        if (!plane.Raycast(ray, out float enter)) return;

        Vector3 hit = ray.GetPoint(enter);

        float camPlaneDist = DistanceAlongForwardToPlane(worldCamera, plane);
        scopeCamera.transform.position = hit - worldCamera.transform.forward * camPlaneDist;
    }

    float DistanceAlongForwardToPlane(Camera cam, Plane plane)
    {
        Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);
        if (!plane.Raycast(forwardRay, out float enter)) return 10f;
        return enter;
    }
}
