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
    public GameManager game = GameManager.Instance;
    bool moving;
    private StateMachine StateMachine { get; set; }
    
    private void OnDrawGizmos()
    {
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
        if(agentNav != null && target != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(gameObject.transform.position, agentNav.destination);
        }
    }
    void Awake()
    {
        currentPoint = game.spawnPoint;
        StateMachine = new StateMachine();
        if (agentNav == null)
        { 
            agentNav = GetComponent<NavMeshAgent>();
            agentNav.enabled = false;
        }
        transform.position = currentPoint.transform.position;
    }

    void Start()
    { 
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

    IEnumerator Timer()
    {
        int randomNum = Random.Range(3,7);
        yield return new WaitForSeconds(randomNum);
        Debug.Log("Waited for " + randomNum);
        StateMachine.SetState(new MoveState(this));
    }

    void Waypointer(Waypoint current)
    {
        StartCoroutine(Timer());
        foreach (Waypoint wayx in current.Neighbours) //saves neighbouring points of current
        {
            int wayNum = current.Neighbours.Length;
            if (wayx != game.spawnPoint || wayNum !< 2)
            {
                if (wayNum > 1)
                {
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
            instance.moving = false;
            instance.Waypointer(instance.currentPoint);
        }
        public override void OnUpdate()
        {
            if (instance.agentNav.destination != null)
            {
                if ((Vector3.Distance(instance.agentNav.transform.position, instance.target.transform.position) >= instance.stoppingDistance) && (instance.moving == false))
                {
                    instance.moving = true;//swap to move state
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
            instance.StartCoroutine(instance.Timer());
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



