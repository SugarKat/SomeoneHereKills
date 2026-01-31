using UnityEngine;
using UnityEngine.AI;
public class BaseAI : MonoBehaviour
{
    public enum AgentState
    {
        Wandering,
        Chasing,
        Moving,
        HangingOut
    }

    public enum AgentRole
    {
        Bystander,
        Killer,
        Target
    }

    AgentState state;
    Transform target;
    NavMeshAgent agent;
    float directionTimer;
    Vector3 currentWanderDir;

    Transform hangoutSpot;
    float hangoutTimer;

    bool isAgentAlive = true;

    public bool IsAgentAlive { get { return isAgentAlive; } }

    public float timeBetweenHangouts = 10f;
    public float hangoutStopDistance = 1.2f;

    public float directionUpdateInterval = 3f;
    public float wanderTurnLimit = 125f; // degrees
    public float wanderDistance = 3f;

    public float attackRange = 2f;

    public AgentRole Role { get; private set; }

    public bool isAKiller = false;
    public bool isATarget = false;

    static Transform killerTarget;
    public Transform KillerTarget {  get { return killerTarget; } }

    private void Awake()
    {
        if(isATarget)
        {
            Role = AgentRole.Target;
            killerTarget = GetComponent<Transform>();
        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentWanderDir = RandomDirectionPos();
        
        hangoutTimer = timeBetweenHangouts;
        state = AgentState.Wandering;

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
        if (!isAgentAlive)
            return;

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
                        GameManager.instance.KillEvent(target.GetComponent<BaseAI>());
                    }
                }
                else
                {
                    StateSwitch(AgentState.Wandering);
                }
                break;

            case AgentState.Moving:
                break;
            
            case AgentState.HangingOut:
                HandleHangingOut();
                break;
        }
    }

    void HandleWandering()
    {
        directionTimer += Time.deltaTime;
        hangoutTimer -= Time.deltaTime;

        // Occasionally decide to go hang out somewhere
        if (!isAKiller && hangoutTimer <= 0 + Random.Range(-2, 2))
        {
            TryGetHangoutSpot();
            return;
        }

        if (directionTimer >= directionUpdateInterval)
        {
            directionTimer = 0f;
            currentWanderDir = GetLimitedTurnDirection(currentWanderDir, wanderTurnLimit);
        }

        Vector3 destination = transform.position + currentWanderDir * wanderDistance;
        agent.destination = destination;
    }

    void HandleHangingOut()
    {
        if (hangoutSpot == null || hangoutTimer >= timeBetweenHangouts + Random.Range(-2, 2))
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
            agent.ResetPath(); // Stay at the spot
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

    public void SetHangoutSpot(Transform spot)
    {
        if (spot == null) return;

        hangoutSpot = spot;
        StateSwitch(AgentState.HangingOut);
    }

    public void KillAgent()
    {
        isAgentAlive = false;
        agent.ResetPath();
    }
}
