using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FiniteStateMachine;

public class EnemyMove : MonoBehaviour
{
    private int stoppingDistance = 1, pursueDistance = 5;
    [SerializeField] private GameObject player, enemyObj;
    private NavMeshAgent agentNav;
    private static Waypoint currentPoint, targetPoint, lastpoint;
    public StateMachine StateMachine { get; private set; }
    public static Waypoint[] waypoints;

    void Awake()
    {
        StateMachine = new StateMachine();
        waypoints = GameManager.Instance.Waypoints;
        currentPoint = GameManager.Instance.SpawnPoint;
        agentNav = GetComponent<NavMeshAgent>();
        enemyObj = this.gameObject;
    }

    void Start()
    {
        agentNav.isStopped = true;
        enemyObj.transform.position = currentPoint.transform.position;
        StateMachine.SetState(new IdleState(this));
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyObj.transform.position, pursueDistance);
        Gizmos.color = Color.blue;
    }

    void Update()
    {
        /*
        if (!agentNav.pathPending && agentNav.remainingDistance < 0.5f)
            GotoNextPoint();
        
        -Patrol state
        -Find waypoints
        -Store waypoints
        -Search for nearest visible waypoint
        -Add waypoint to targetNav
        -Move to waypoint

        =====

        -If player in visual range
        -pursue state
        -if player not visible for <5sec
        -return to last point
        -Patrol state
         
        Debug.Log("Enemy is patrolling!");
        // Returns if no points have been set up
        if (navPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agentNav.destination = navPoints[stoppingDistance].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        stoppingDistance = (stoppingDistance + 1) % navPoints.Length;
    */
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
            lastpoint = currentPoint;
            Debug.Log("Movement state entered");
            instance.agentNav.isStopped = false;
        }
        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.transform.position, targetPoint.transform.position) > instance.stoppingDistance)
            {
                instance.agentNav.SetDestination(targetPoint.transform.position);
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
            Debug.Log("Idle state entered");
            instance.agentNav.isStopped = true;
            foreach (Waypoint wayx in currentPoint.Neighbours)
            {
                if (currentPoint.Neighbours.Length <= 2)
                {
                    int length = currentPoint.Neighbours.Length;
                    {
                        if (wayx != lastpoint)
                        {
                            lastpoint = currentPoint;
                            targetPoint = wayx;
                            break;
                        }
                        
                    }
                    int randoNum = Random.Range(0, length);
                    targetPoint = currentPoint.Neighbours[randoNum];
                    lastpoint = currentPoint;
                    break;
                }
                lastpoint = targetPoint;
            }
        }
        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.transform.position, targetPoint.transform.position) > instance.stoppingDistance)
            {
                instance.StateMachine.SetState(new MoveState(instance)); //swap to move state
            }
            else if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.pursueDistance)
            {
                instance.StateMachine.SetState(new PursueState(instance)); // swap to pursue state
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
            targetPoint = lastpoint;
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



