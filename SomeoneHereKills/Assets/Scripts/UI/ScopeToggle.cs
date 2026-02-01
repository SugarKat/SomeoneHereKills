using UnityEngine;
using UnityEngine.InputSystem;

public class ScopeToggle : MonoBehaviour
{
    public GameObject scopeUI;
    public Camera scopeCamera;

    void Update()
    {
        bool aiming = Mouse.current != null && Mouse.current.rightButton.isPressed;

        if (scopeUI) scopeUI.SetActive(aiming);
        if (scopeCamera) scopeCamera.enabled = aiming;
    }
}