using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FiniteStateMachine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private int pursueDistance = 5;
    private float stoppingDistance;
    private GameObject player;
    private NavMeshAgent agentNav;
    public Waypoint currentPoint, targetPoint, lastpoint;
    private StateMachine StateMachine { get; set; }

    private void OnDrawGizmos()
    {
        if (targetPoint != null)
        {
            Gizmos.DrawWireSphere(targetPoint.transform.position, stoppingDistance);
            Gizmos.color = Color.red;
        }
        if (currentPoint != null)
        {
            Gizmos.DrawWireSphere(currentPoint.transform.position, stoppingDistance);
            Gizmos.color = Color.blue;
        }
        if (lastpoint != null)
        {
            Gizmos.DrawWireSphere(lastpoint.transform.position, stoppingDistance);
            Gizmos.color = Color.green;
        }
        Gizmos.DrawWireSphere(transform.position, pursueDistance);
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        Gizmos.color = Color.red;
    }

    void Awake()
    {
        StateMachine = new StateMachine();
        agentNav = GetComponent<NavMeshAgent>();
        stoppingDistance = agentNav.stoppingDistance;
    }

    void Start()
    {
        agentNav.isStopped = true;
        Debug.Log("Teleport");
        currentPoint = GameManager.Instance.spawnPoint;
        transform.position = currentPoint.transform.position;
    }

    void Update()
    {
        if (agentNav.remainingDistance <= stoppingDistance | targetPoint == null)
        {
            agentNav.isStopped = true;
            foreach (Waypoint wayx in currentPoint.Neighbours)
            {
                int length = currentPoint.Neighbours.Length;
                if (lastpoint != null)
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
                        targetPoint = currentPoint.Neighbours[randoNum];
                        break;
                    }
                }
                agentNav.SetDestination(targetPoint.transform.position);                    
                Debug.Log("New destination");
            }
        }
        if (currentPoint.Neighbours.Length == 0)
        {
            return;
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
            Debug.Log("Movement state entered");
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
            if (instance.targetPoint != null)
            { 
                instance.agentNav.destination = instance.targetPoint.transform.position;
            }
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
            instance.agentNav.isStopped = false;
        }
        public override void OnExit()
        {
            Debug.Log("Pursing state exit");
            instance.agentNav.isStopped = true;
            instance.targetPoint = instance.lastpoint;
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



