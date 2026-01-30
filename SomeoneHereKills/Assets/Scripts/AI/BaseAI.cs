using UnityEngine;
using UnityEngine.AI;
public class BaseAI : MonoBehaviour
{
    public enum AgentState
    {
        Wandering,
        Chasing,
        Moving
    }

    AgentState state;
    Transform target;
    NavMeshAgent agent;
    float directionTimer;
    Vector3 currentWanderDir;

    public float directionUpdateInterval = 3f;
    public float wanderTurnLimit = 125f; // degrees
    public float wanderDistance = 3f;

    public float attackRange = 2f;

    public bool isAKiller = false;
    public bool isATarget = false;

    static Transform killerTarget;
    public Transform KillerTarget {  get { return killerTarget; } }

    private void Awake()
    {
        if(isATarget)
        {
            killerTarget = GetComponent<Transform>();
        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentWanderDir = RandomDirectionPos();

        if(isAKiller)
        {
            state = AgentState.Chasing;
            target = killerTarget;
        }
        else
            state = AgentState.Wandering;

    }

    private void Update()
    {
        switch (state)
        {
            case AgentState.Wandering:
                HandleWandering();
                break;

            case AgentState.Chasing:
                if (target == null)
                    target = killerTarget;

                if (target != null)
                {
                    float dist = Vector3.Distance(transform.position, target.position);

                    if (dist > attackRange)
                    {
                        agent.destination = target.position;
                    }
                    else
                    {
                        agent.ResetPath();
                    }
                }
                else
                {
                    StateSwitch(AgentState.Wandering);
                }
                break;

            case AgentState.Moving:
                break;
        }
    }

    void HandleWandering()
    {
        directionTimer += Time.deltaTime;

        if (directionTimer >= directionUpdateInterval)
        {
            directionTimer = 0f;
            currentWanderDir = GetLimitedTurnDirection(currentWanderDir, wanderTurnLimit);
        }

        Vector3 destination = transform.position + currentWanderDir * wanderDistance;
        agent.destination = destination;
    }

    Vector3 GetLimitedTurnDirection(Vector3 currentDir, float maxAngle)
    {
        float randomAngle = Random.Range(-maxAngle, maxAngle);
        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        Vector3 newDir = rotation * currentDir;
        return newDir.normalized;
    }

    Vector3 RandomDirectionPos()
    {
        Vector2 circle = Random.insideUnitCircle.normalized;
        return new Vector3(circle.x, 0f, circle.y);
    }

    public void StateSwitch(AgentState _state)
    {
        state = _state;
    }
}
