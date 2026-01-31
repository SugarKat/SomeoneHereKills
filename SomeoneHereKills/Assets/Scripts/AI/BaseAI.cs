using UnityEngine;
using UnityEngine.AI;

public class BaseAI : MonoBehaviour
{
    public enum AgentState { Wandering, Chasing, Moving, HangingOut }
    public enum AgentRole { Bystander, Killer, Target }

    AgentState state;
    Transform target;
    NavMeshAgent agent;

    float directionTimer;
    Vector3 currentWanderDir;

    Transform hangoutSpot;
    float hangoutTimer;

    bool isAgentAlive = true;
    public bool IsAgentAlive => isAgentAlive;

    public float timeBetweenHangouts = 10f;
    public float hangoutStopDistance = 1.2f;

    public float directionUpdateInterval = 3f;
    public float wanderTurnLimit = 125f;
    public float wanderDistance = 3f;

    public float attackRange = 2f;
    public float aggressiveChaseRange = 6f;

    public AIAnimator animator;

    [Header("Killer Deception")]
    public float deceptionCheckInterval = 4f;
    public float deceptionChance = 0.4f;
    public float fakeHangoutTime = 3f;
    public float targetOffsetRadius = 2.5f;

    float deceptionTimer;
    float fakeTimer;
    bool isFaking;
    Transform fakeSpot;

    public AgentRole Role { get; private set; }
    public bool isAKiller = false;
    public bool isATarget = false;

    static Transform killerTarget;

    private void Awake()
    {
        if (isATarget)
        {
            Role = AgentRole.Target;
            killerTarget = transform;
        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentWanderDir = RandomDirectionPos();

        hangoutTimer = timeBetweenHangouts;
        state = AgentState.Wandering;

        // Slight speed variation for crowd realism
        agent.speed *= Random.Range(0.9f, 1.1f);

        if (isAKiller)
        {
            Role = AgentRole.Killer;
            state = AgentState.Chasing;
            target = killerTarget;
        }
        else if (!isATarget)
        {
            Role = AgentRole.Bystander;
        }
    }

    private void Update()
    {
        if (!isAgentAlive) return;

        switch (state)
        {
            case AgentState.Wandering:
                HandleWandering();
                break;

            case AgentState.Chasing:
                HandleChasing();
                break;

            case AgentState.HangingOut:
                HandleHangingOut();
                break;
        }
    }

    void HandleChasing()
    {
        if (target == null)
            target = killerTarget;

        if (target == null)
        {
            StateSwitch(AgentState.Wandering);
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        // If far away, blend into crowd
        if (dist > aggressiveChaseRange)
        {
            HandleKillerDeception();
            return;
        }

        // Close = real pursuit
        Vector3 offset = Random.insideUnitSphere * targetOffsetRadius;
        offset.y = 0;
        Vector3 noisyTargetPos = target.position + offset;

        if (dist > attackRange)
        {
            agent.destination = noisyTargetPos;
        }
        else
        {
            agent.ResetPath();
            GameManager.instance.KillEvent(target.GetComponent<BaseAI>());
        }
    }

    void HandleKillerDeception()
    {
        if (!isFaking)
        {
            deceptionTimer += Time.deltaTime;

            if (deceptionTimer >= deceptionCheckInterval)
            {
                deceptionTimer = 0f;

                if (Random.value < deceptionChance && LevelManager.instance != null)
                {
                    Transform spot = LevelManager.instance.GetAnInterestPoint();
                    if (spot != null)
                    {
                        fakeSpot = spot;
                        fakeTimer = 0f;
                        isFaking = true;
                    }
                }
            }

            // If not faking, just wander like normal NPC
            HandleWandering();
        }
        else
        {
            fakeTimer += Time.deltaTime;

            if (fakeSpot == null)
            {
                isFaking = false;
                return;
            }

            float dist = Vector3.Distance(transform.position, fakeSpot.position);

            if (dist > hangoutStopDistance)
            {
                agent.destination = fakeSpot.position;
            }
            else
            {
                agent.ResetPath();
            }

            if (fakeTimer >= fakeHangoutTime)
            {
                isFaking = false;
            }
        }
    }

    void HandleWandering()
    {
        directionTimer += Time.deltaTime;
        hangoutTimer -= Time.deltaTime;

        if (!isAKiller && hangoutTimer <= 0f)
        {
            TryGetHangoutSpot();
            return;
        }

        if (directionTimer >= directionUpdateInterval)
        {
            directionTimer = 0f;
            currentWanderDir = GetLimitedTurnDirection(currentWanderDir, wanderTurnLimit);
        }

        Vector3 rawDestination = transform.position + currentWanderDir * wanderDistance;

        if (NavMesh.SamplePosition(rawDestination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
        }
    }

    void HandleHangingOut()
    {
        if (hangoutSpot == null)
        {
            StateSwitch(AgentState.Wandering);
            return;
        }

        float dist = Vector3.Distance(transform.position, hangoutSpot.position);

        if (dist > hangoutStopDistance)
        {
            agent.destination = hangoutSpot.position;
        }
        else
        {
            hangoutTimer += Time.deltaTime;
            agent.ResetPath();

            if (hangoutTimer >= timeBetweenHangouts)
            {
                hangoutTimer = timeBetweenHangouts;
                StateSwitch(AgentState.Wandering);
            }
        }
    }

    void TryGetHangoutSpot()
    {
        if (LevelManager.instance == null) return;

        Transform spot = LevelManager.instance.GetAnInterestPoint();
        if (spot == null) return;

        hangoutSpot = spot;
        hangoutTimer = 0f;
        StateSwitch(AgentState.HangingOut);
    }

    Vector3 GetLimitedTurnDirection(Vector3 currentDir, float maxAngle)
    {
        float randomAngle = Random.Range(-maxAngle, maxAngle);
        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        return (rotation * currentDir).normalized;
    }

    Vector3 RandomDirectionPos()
    {
        Vector2 circle = Random.insideUnitCircle.normalized;
        return new Vector3(circle.x, 0f, circle.y);
    }

    public void StateSwitch(AgentState newState)
    {
        state = newState;
    }

    public void KillAgent()
    {
        isAgentAlive = false;
        agent.ResetPath();
        agent.isStopped = true;
        animator?.SetDead();
    }
}
