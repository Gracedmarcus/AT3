using FiniteStateMachine;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    public int pursueDistance = 5;
    private float stoppingDistance;
    private GameObject player;
    private NavMeshAgent agentNav;
    public Waypoint currentPoint, target;
    private StateMachine StateMachine { get; set; }

    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.transform.position, stoppingDistance);
        }
        if (currentPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(currentPoint.transform.position, stoppingDistance);
        }
        if (stoppingDistance != 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gameObject.transform.position, stoppingDistance);
        }
        if (pursueDistance != 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gameObject.transform.position, pursueDistance);
        }
        if(agentNav != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(gameObject.transform.position, agentNav.destination);
        }
    }
    void Awake()
    {
        StateMachine = new StateMachine();
        if (agentNav == null)
        { 
            agentNav = GetComponent<NavMeshAgent>();
            stoppingDistance = agentNav.stoppingDistance;
        }
    }

    void Start()
    {
        Debug.Log("Teleport");
        currentPoint = GameManager.Instance.spawnPoint;
        transform.position = currentPoint.transform.position;
        if(StateMachine.CurrentState == null)
        {
            StateMachine.SetState(new IdleState(this));
        }
    }
    void Update()
    {
        if (agentNav.destination != null)
        {
            Debug.Log(agentNav.remainingDistance);
        }
    }

    void Waypointer(Waypoint current)
    {        
        foreach (Waypoint wayx in current.Neighbours) //saves neighbouring points of current
        {
            int wayNum = current.Neighbours.Length;
            if (target != null && current != GameManager.Instance.spawnPoint)
            {
                if (wayNum <= 1)
                {
                    Debug.Log("Dead end");;
                    target = wayx;
                    break;
                }
                if (wayNum > 1)
                {
                    Debug.Log("Randomizing");
                    int randoNum = Random.Range(0, wayNum);
                    target = current.Neighbours[randoNum];
                    break;
                }
            }
            Debug.Log("Spawned");
            target = wayx;
            break;
        }                
        agentNav.SetDestination(target.transform.position);                    
        Debug.Log("New destination" + target);
    }

    public abstract class EnemyMoveState : IState
    {
        protected EnemyMove instance;
        public EnemyMoveState(EnemyMove _instance)
        {
            instance = _instance;
        }
        public virtual void OnEnter() //enemy on state enter
        {
        }
        public virtual void OnExit() //enemy on state exit
        {
        }
        public virtual void OnUpdate() //enemy during state
        {
        }
    }

    public class MoveState : EnemyMoveState
    {
        public MoveState(EnemyMove _instance) : base(_instance)
        {
        }

        public override void OnEnter()
        {
            if(instance.target != instance.currentPoint)
            {
                instance.currentPoint = instance.target;
            }
        }
        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.transform.position, instance.target.transform.position) < instance.stoppingDistance)
            {
                instance.agentNav.SetDestination(instance.target.transform.position);
            }
            else if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.pursueDistance)
            {
                instance.StateMachine.SetState(new PursueState(instance)); //swap to pursue state
            }
            else
            {
                instance.StateMachine.SetState(new IdleState(instance)); //swap to idle state
            }
        }
    }

    public class IdleState : EnemyMoveState
    {
        public IdleState(EnemyMove _instance) : base(_instance)
        {
        }

        public override void OnEnter()
        {
            if (instance.currentPoint != instance.target) 
            { 
            instance.Waypointer(instance.currentPoint);
            }
        }
        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.transform.position, instance.agentNav.destination) > instance.stoppingDistance)
            {
                instance.StateMachine.SetState(new MoveState(instance)); //swap to move state
            }
            else if (Vector3.Distance(instance.transform.position, instance.agentNav.destination) < instance.pursueDistance)
            {
                instance.StateMachine.SetState(new PursueState(instance)); // swap to pursue state
            }
        }
        public override void OnExit()
        {
            Debug.Log("Entered" + instance.StateMachine.CurrentState);
        }
    }

    public class PursueState : EnemyMoveState
    {
        public PursueState(EnemyMove _instance) : base(_instance)
        {
        }

        public override void OnEnter()
        {
        }
        public override void OnExit()
        {
            Debug.Log("Pursing state exit");
        }
        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.pursueDistance)
            {
                instance.agentNav.SetDestination(instance.player.transform.position);
            }
            else
            {
                instance.StateMachine.SetState(new IdleState(instance)); //swap to idle state
            }
        }
    }
}



