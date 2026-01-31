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

    [Header("Idle Fidget")]
    public float minFidgetDelay = 3f;
    public float maxFidgetDelay = 7f;
    [Range(0f, 1f)] public float fidgetChance = 0.25f;

    private float fidgetTimer;
    const string DEAD = "Dead";

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        fidgetTimer = Random.Range(minFidgetDelay, maxFidgetDelay);
    }

    void Update()
    {
        UpdateAnimFromAgent();
    }

    void UpdateAnimFromAgent()
    {
        if (agent == null || anim == null) return;

        if (anim.GetBool(DEAD))
        {
            anim.SetBool("isMoving", false);
            return;
        }

        Vector3 vVel = agent.velocity;
        Vector3 vDes = agent.desiredVelocity;

        Vector2 vel2 = new Vector2(vVel.x, vVel.z);
        Vector2 des2 = new Vector2(vDes.x, vDes.z);

        float thr2 = movingThreshold * movingThreshold;

        bool moving =
            (vel2.sqrMagnitude > thr2) ||
            (des2.sqrMagnitude > thr2 && agent.hasPath && !agent.pathPending);

        Vector2 dir2D = (vel2.sqrMagnitude > thr2) ? vel2 : des2;
        Vector2 dir = moving ? dir2D.normalized : Vector2.zero;

        if (moving)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) * 0.6f)
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

        //fidget idle
        if (!moving && !anim.GetBool("Dead"))
        {
            fidgetTimer -= Time.deltaTime;

            if (fidgetTimer <= 0f)
            {
                if (Random.value < fidgetChance)
                    anim.SetTrigger("DoFidget");

                fidgetTimer = Random.Range(minFidgetDelay, maxFidgetDelay);
            }
        }
        else
        {
            fidgetTimer = Random.Range(minFidgetDelay, maxFidgetDelay);
        }
    }

    public void SetDead(bool dead = true)
    {
        if (!anim) return;
        anim.SetBool(DEAD, dead);

        if (dead)
        {
            anim.SetBool("isMoving", false);
        }
    }

    public bool IsDead()
    {
        return anim && anim.GetBool(DEAD);
    }
}
