using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class AIAnimator : MonoBehaviour
{
    [Header("Refs")]
    public NavMeshAgent agent;
    public Animator anim;

    [Header("Tuning")]
    public float movingThreshold = 0.02f;

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        UpdateAnimFromAgent();
    }

    void UpdateAnimFromAgent()
    {
        if (agent == null || anim == null) return;

        Vector3 v3 = agent.desiredVelocity;
        Vector2 dir2D = new Vector2(-v3.z, v3.y);

        bool hasMoveIntent =
            agent.hasPath &&
            !agent.pathPending &&
            agent.remainingDistance > agent.stoppingDistance + 0.05f;

        bool moving = hasMoveIntent && dir2D.sqrMagnitude > 0.001f;

        Vector2 dir = moving ? dir2D.normalized : Vector2.zero;

        if (moving)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                dir.x = Mathf.Sign(dir.x);
                dir.y = 0f;
            }
            else
            {
                dir.y = Mathf.Sign(dir.y);
                dir.x = 0f;
            }
        }

        anim.SetBool("isMoving", moving);
        anim.SetFloat("MoveX", dir.x);
        anim.SetFloat("MoveY", dir.y);

        if (moving)
        {
            anim.SetFloat("LastMoveX", dir.x);
            anim.SetFloat("LastMoveY", dir.y);
        }
    }

}
