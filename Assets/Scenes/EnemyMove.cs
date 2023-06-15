using FiniteStateMachine;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private float pursueDistance = 5;
    private float stoppingDistance = 2;
    private Transform player, lastPos;
    private NavMeshAgent agentNav;
    public Waypoint currentPoint, target;
    [SerializeField]private bool playerFound, isStunned;
    private LayerMask playerMask;
    public Collider stunBlock;
    public bool toggle, toggleMove;
    private StateMachine StateMachine { get; set; }
    
    private void OnDrawGizmos()
    {
        if (stoppingDistance != 0 && toggleMove)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gameObject.transform.position, stoppingDistance);
        }
        if (pursueDistance != 0 && toggle)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gameObject.transform.position, pursueDistance);
        }
        if(agentNav != null && target != null)
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
            agentNav.enabled = false;
        }
    }

    void Start()
    { 
        currentPoint = GameManager.Instance.spawnPoint;
        transform.position = currentPoint.transform.position;
        Debug.Log("Teleported");
        agentNav.enabled = true;
        if (StateMachine.CurrentState == null)
        {
            StateMachine.SetState(new IdleState(this));
        }
    }
    void FixedUpdate()
    {
        playerFound = Physics.CheckSphere(transform.position, pursueDistance, playerMask);
        StateMachine.CurrentState.OnUpdate();
        if (playerFound)
        {
            Debug.Log("Player found");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == stunBlock)
        {
            isStunned = true;
        }
    }

    private IEnumerator Timer(float Number)
    {
        yield return new WaitForSeconds(Number);
        {
            Debug.Log("Stun finished");
            isStunned = false;
        }
    }

    void Waypointer(Waypoint current)
    {
        foreach (Waypoint wayx in current.Neighbours) //saves neighbouring points of current
        {
            int wayNum = current.Neighbours.Length;
            if (wayx != GameManager.Instance.spawnPoint || wayNum !< 2)
            {
                if (wayNum > 1)
                {
                    Debug.Log("Randomizing");
                    int randoNum = Random.Range(0, wayNum);
                    target = current.Neighbours[randoNum];
                    break;
                }
                target = wayx;
                break;
            }
        }
        Debug.Log("New destination at " + target);
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
        public virtual void OnUpdate() //enemy during state
        {
        }        
        public virtual void OnExit() //enemy on state exit
        {
        }
    }

    public class IdleState : EnemyMoveState
    {
        public IdleState(EnemyMove _instance) : base(_instance)
        {
        }
        public override void OnEnter()
        {
            Debug.Log("Idle start");
            instance.Waypointer(instance.currentPoint);
        }
        public override void OnUpdate()
        {
            if (instance.agentNav.destination != null)
            {
                if (Vector3.Distance(instance.agentNav.transform.position, instance.target.transform.position) >= instance.stoppingDistance)
                {
                    instance.StateMachine.SetState(new MoveState(instance)); //swap to move state
                }
            }
            else if (instance.playerFound && !instance.isStunned)
            {
                instance.StateMachine.SetState(new PursueState(instance)); // swap to pursue state
            }
        }
        public override void OnExit()
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
            Debug.Log("Move start");
            if (instance.target != null || instance.target != instance.currentPoint)
            {
                if (instance.lastPos != null)
                {
                    instance.agentNav.SetDestination(instance.lastPos.transform.position);
                }
                instance.agentNav.SetDestination(instance.target.transform.position);
            }
        }
        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.agentNav.transform.position, instance.target.transform.position) <= instance.stoppingDistance)
            {
                instance.StateMachine.SetState(new IdleState(instance));
                instance.lastPos = null;
            }
            else if (instance.playerFound)
            {
                instance.StateMachine.SetState(new PursueState(instance)); //swap to pursue state
            }
        }
        public override void OnExit()
        {
            if (instance.playerFound == false)
            { 
            instance.currentPoint = instance.target;
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
            Debug.Log("Pursue entered!");
            instance.lastPos = instance.target.transform;
        }

        public override void OnUpdate()
        {
            if (instance.playerFound && !instance.isStunned)
            {
                instance.agentNav.SetDestination(instance.player.transform.position);
            }
            else
            {
                instance.StateMachine.SetState(new MoveState(instance)); //return to last target
            }
        }        
        public override void OnExit()
        {
            
        }
    }
    public class StunState : EnemyMoveState
    {
        public StunState(EnemyMove _instance) : base(_instance)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Enemy Stunned");
            instance.isStunned = true;
            instance.lastPos = instance.target.transform;            
            instance.StartCoroutine("Timer", 3f);
        }
        public override void OnUpdate()
        {
            if(instance.isStunned != true)
            {
                instance.StateMachine.SetState(new MoveState(instance));
            }
        }
        public override void OnExit()
        {

        }
    }
}



