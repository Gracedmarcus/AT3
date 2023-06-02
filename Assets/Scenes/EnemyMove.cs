using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FiniteStateMachine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private int pursueDistance = 5;
    [SerializeField] private float stoppingDistance;
    private GameObject player;
    private bool stateSwap;
    private NavMeshAgent agentNav;
    public Waypoint currentPoint, targetPoint, lastpoint;
    private StateMachine StateMachine { get; set; }

    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        if (targetPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetPoint.transform.position, stoppingDistance);
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
    }
    void Awake()
    {
        StateMachine = new StateMachine();
        agentNav = GetComponent<NavMeshAgent>();
        agentNav.isStopped = true;
    }

    void Start()
    {
        Debug.Log("Teleport");
        stateSwap = true;
        currentPoint = GameManager.Instance.spawnPoint;
        transform.position = currentPoint.transform.position;
        if(StateMachine.CurrentState == null)
        {
            StateMachine.SetState(new IdleState(this));
        }
    }

    void Update()
    {
        if (stateSwap == true)
        {
            Debug.Log(StateMachine.GetCurrentStateAsType<IState>());
            stateSwap = false;
        }
        if (agentNav.isStopped == true)
        {
            Waypointer(currentPoint);
        }
    }

    void Waypointer(Waypoint waypoint)
    {        
        if (agentNav.destination == null)
        {
            foreach (Waypoint wayx in waypoint.Neighbours)
            {
                int length = waypoint.Neighbours.Length;
                if (targetPoint != null)
                {
                    if (length <= 1)
                    {
                        Debug.Log("Only one neighbour here");
                        targetPoint = wayx;
                        break;
                    }
                    if (length > 1)
                    {
                        Debug.Log("More than one neighbour here");
                        int randoNum = Random.Range(0, length);
                        targetPoint = waypoint.Neighbours[randoNum];
                        break;
                    }
                }
                targetPoint = wayx;
            }                
            agentNav.SetDestination(targetPoint.transform.position);                    
            Debug.Log("New destination");
        }
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
            instance.stateSwap = true;
            Debug.Log("Moving toward " + instance.targetPoint);
            instance.agentNav.isStopped = false;
        }
        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.transform.position, instance.targetPoint.transform.position) > instance.stoppingDistance)
            {
                instance.agentNav.SetDestination(instance.targetPoint.transform.position);
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
            instance.stateSwap = true;
            Debug.Log("Idle state at " + instance.currentPoint.name);
        }
        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.transform.position, instance.targetPoint.transform.position) > instance.stoppingDistance)
            {
                instance.StateMachine.SetState(new MoveState(instance)); //swap to move state
            }
            else if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.pursueDistance)
            {
                instance.StateMachine.SetState(new PursueState(instance)); // swap to pursue state
            }
        }
        public override void OnExit()
        {
        }
    }

    public class PursueState : EnemyMoveState
    {
        public PursueState(EnemyMove _instance) : base(_instance)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Pursing state entered");
            instance.stateSwap = true;
            instance.agentNav.isStopped = false;
        }
        public override void OnExit()
        {
            Debug.Log("Pursing state exit");
            instance.agentNav.isStopped = true;
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



