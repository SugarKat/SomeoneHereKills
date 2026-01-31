using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInputActions inputActions;
    Camera cam;

    public LayerMask agentLayer;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        cam = Camera.main;
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
        inputActions.Gameplay.Interaction.performed += OnClickPerformed;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Interaction.performed -= OnClickPerformed;
        inputActions.Gameplay.Disable();
    }

    private void OnClickPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, agentLayer))
        {
            BaseAI agent = hit.collider.GetComponentInParent<BaseAI>();
            if (agent != null && agent.IsAgentAlive)
            {
                GameManager.instance.KillEvent(agent);
            }
        }
    }
}
