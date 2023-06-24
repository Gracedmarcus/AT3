using FiniteStateMachine;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private float pursueDistance = 7;
    private float stoppingDistance = 1;
    private bool waiter;
    private Transform player;
    private NavMeshAgent agentNav;
    public Waypoint currentPoint, target;
    [SerializeField]private bool playerFound, isStunned;
    private Animator animator;
    [SerializeField] private AnimationClip animStun, animChase, animDef;
    public GameObject stunBlock;
    public AudioClip stunned, found;
    public AudioSource audioSource;
    public GameManager game;
    private StateMachine StateMachine { get; set; }

    void Awake()
    {
        currentPoint = game.spawnPoint;
        StateMachine = new StateMachine();
        if (agentNav == null)
        { 
            agentNav = GetComponent<NavMeshAgent>();
        }
        transform.position = currentPoint.transform.position;
        if (StateMachine.CurrentState == null)
        {
            StateMachine.SetState(new IdleState(this));
        }
        animator = gameObject.GetComponent<Animator>();
        player = game.player.gameObject.transform;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!isStunned && collision.gameObject == player.gameObject)
        {
            game.GameOver();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == stunBlock)
        {
            StateMachine.SetState(new StunState(this));
            isStunned = true;
        }
    }

    void Start()
    { 
        animator.Play("animDef");
    }
    void FixedUpdate()
    {
        if(!isStunned)
        { 
            StateMachine.CurrentState.OnUpdate();        
            if ((game.goalInt == 2) || (Vector3.Distance(gameObject.transform.position, player.position) <= pursueDistance))
            {
                playerFound = true;
            }
        }
        if(isStunned)
        {
            playerFound = false;
        }
    }

    IEnumerator Timer(string type)
    {
        if (type == "move")
        {
            Debug.Log("timer start");
            int randomNum = Random.Range(2, 8);
            yield return new WaitForSeconds(randomNum);
            Waypointer(currentPoint);
        }
        else if(type == "stun")
        {
            yield return new WaitForSeconds(5f);
            isStunned = false;
        }
    }

    void Waypointer(Waypoint current)
    {
        Debug.Log("start waypoint");
        if (target != null)
        {
            target = null;
        }
        foreach (Waypoint wayx in current.Neighbours) //saves neighbouring points of current
        {
            int wayNum = current.Neighbours.Length;
            if (wayx != game.spawnPoint)
            {
                if (wayNum > 1)
                {
                    int randoNum = Random.Range(0, wayNum);
                    target = current.Neighbours[randoNum];
                    break;
                }
                target = wayx;
                Debug.Log("target");
                break;
            }
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
            instance.StartCoroutine(instance.Timer("move"));
        }
        public override void OnUpdate()
        {
            if (instance.target != null && (Vector3.Distance(instance.agentNav.transform.position, instance.target.transform.position) >= instance.stoppingDistance))
            {
                Debug.Log("moving");
                instance.StateMachine.SetState(new MoveState(instance));//swap to move state
            }
            if(instance.playerFound)
            {
                instance.StateMachine.SetState(new PursueState(instance));
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
            instance.agentNav.SetDestination(instance.target.transform.position);
        }
        public override void OnUpdate()
        {
            if ((Vector3.Distance(instance.agentNav.transform.position, instance.target.transform.position) <= instance.stoppingDistance))
            {
                instance.StateMachine.SetState(new IdleState(instance));
                instance.currentPoint = instance.target;
            }            
            else if (instance.isStunned)
            {
                instance.StateMachine.SetState(new StunState(instance)); // swap to pursue state
            }
            else if (instance.playerFound)
            {
                instance.StateMachine.SetState(new PursueState(instance)); //swap to pursue state
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
            instance.audioSource.PlayOneShot(instance.found);
            instance.animator.Play(instance.animChase.name);
        }

        public override void OnUpdate()
        {
            if (!instance.isStunned)
            {
                instance.agentNav.SetDestination(instance.player.transform.position);
            }
            else
            {                
                if (instance.isStunned)
                {
                    instance.StateMachine.SetState(new StunState(instance));
                }//enemy stunned
                else if (!instance.playerFound && instance.target)
                { 
                    instance.StateMachine.SetState(new MoveState(instance));
                }//return to last target
            }
        }        
        public override void OnExit()
        {
            instance.animator.Play("animDef");
        }
    }
    public class StunState : EnemyMoveState
    {
        public StunState(EnemyMove _instance) : base(_instance)
        {
        }

        public override void OnEnter()
        {
            instance.audioSource.PlayOneShot(instance.stunned);
            instance.animator.Play(instance.animStun.name);           
            instance.StartCoroutine(instance.Timer("stun"));
        }
        public override void OnUpdate()
        {            
            if(instance.playerFound)
            {
                instance.StateMachine.SetState(new PursueState(instance));
            }
            else if (!instance.playerFound)
            {
                instance.StateMachine.SetState(new MoveState(instance));
            }
        }
        public override void OnExit()
        {
            instance.animator.Play(instance.animDef.name);
        }
    }
}



