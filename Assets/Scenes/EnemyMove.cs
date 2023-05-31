using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FiniteStateMachine;

public class EnemyMove : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int stoppingDistance = 1, pursueDistance = 5;
    [SerializeField] private GameObject player, enemyObj;
    private NavMeshAgent agentNav;
    private Transform  targetPoint;
    public StateMachine StateMachine { get; private set; }

    void Awake()
    {
        StateMachine = new StateMachine();
        agentNav = GetComponent<NavMeshAgent>();
        enemyObj = this.gameObject;
    }

    void Start()
    {
        agentNav.autoBraking = false;
        agentNav.isStopped = true;
        StateMachine.SetState(new IdleState(this));
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyObj.transform.position, pursueDistance);
        Gizmos.color = Color.blue;
    }

    // Update is called once per frame
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
        -Attack state
        -Translate position to player
        -Trigger health loss if player collision
        -Return to last waypoint if player not visible for <5sec
        -Patrol state
         
    }
    void PatrolState()
    {
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
                instance.StateMachine.SetState(new PursueState(instance));
            }
            else
            {
                instance.StateMachine.SetState(new IdleState(instance));
            }
        }
    }

    public class IdleState : EnemyMoveState
    {
        public IdleState(EnemyMove _instance) : base(_instance)
        {
            /*if (Physics.SphereCast(enemyObj.transform.position, pursueDistance, transform.forward, out RaycastHit rayHit))
            {
                if (rayHit.collider.gameObject != null)
                {
                    if (rayHit.collider.gameObject == player)
                    {
                        StateMachine.SetState(new PursueState(this));
                    }
                }
                else
                {

                }
            }*/
        }

        public override void OnEnter()
        {
            Debug.Log("Idle state entered");
            instance.agentNav.isStopped = true;
        }
        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.transform.position, instance.targetPoint.transform.position) > instance.stoppingDistance)
            {
                instance.StateMachine.SetState(new MoveState(instance));
            }
            else if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.pursueDistance)
            {
                instance.StateMachine.SetState(new PursueState(instance));
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
        }
        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.pursueDistance)
            {
                instance.agentNav.SetDestination(instance.player.transform.position);
            }
            else
            {
                instance.StateMachine.SetState(new IdleState(instance));
            }
        }
    }
}



