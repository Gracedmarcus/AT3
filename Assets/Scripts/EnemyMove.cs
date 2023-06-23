using FiniteStateMachine;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private float pursueDistance = 7;
    private float stoppingDistance = 1;
    private Transform player, lastPos;
    private NavMeshAgent agentNav;
    public Waypoint currentPoint, target;
    [SerializeField]private bool playerFound, isStunned;
    private Animator animator;
    [SerializeField] private AnimationClip animStun, animChase, animDef;
    public GameObject stunBlock;
    public AudioClip stunned, found;
    public AudioSource audioSource;
    public GameManager game = GameManager.Instance;
    bool moving;
    private StateMachine StateMachine { get; set; }

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
        if (StateMachine.CurrentState == null)
        {
            StateMachine.SetState(new IdleState(this));
        }
        animator = gameObject.GetComponent<Animator>();
        player = game.player.gameObject.transform;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == stunBlock)
        {
            StateMachine.SetState(new StunState(this));
            isStunned = true;
        }
        else if(collider.gameObject == player.gameObject && !isStunned)
        {
            game.GameOver();
        }
    }

    void Start()
    { 
        agentNav.enabled = true;
        animator.Play("animDef");
    }
    void FixedUpdate()
    {
        if(!isStunned)
        { 
            StateMachine.CurrentState.OnUpdate();        
            if ((Vector3.Distance(gameObject.transform.position, player.position) <= pursueDistance) || (game.goalInt == 2))
            {
                playerFound = true;
            }
            else
            {
                playerFound = false;
            }
        }
    }

    IEnumerator Timer(int timer)
    {
        if (!isStunned)
        {
            int randomNum = Random.Range(2, 8);
            yield return new WaitForSeconds(randomNum);
            Debug.Log("Waited for " + randomNum);
            StateMachine.SetState(new MoveState(this));
        }
        else if(isStunned)
        {
            yield return new WaitForSeconds(timer);
            isStunned = false;
        }
    }

    void Waypointer(Waypoint current)
    {
        StartCoroutine(Timer(0));
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
            else if (instance.isStunned)
            {
                instance.StateMachine.SetState(new StunState(instance));// swap to pursue state
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
            instance.moving = true;
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
                instance.lastPos = null;
                instance.StateMachine.SetState(new IdleState(instance));
            }
            else if (instance.playerFound && !instance.isStunned)
            {
                instance.StateMachine.SetState(new PursueState(instance)); //swap to pursue state
            }
            else if (instance.isStunned)
            {
                instance.StateMachine.SetState(new StunState(instance)); // swap to pursue state
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
            instance.audioSource.PlayOneShot(instance.found);
            instance.moving = true;
            instance.animator.Play(instance.animChase.name);
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
                if (instance.isStunned)
                {
                    instance.StateMachine.SetState(new StunState(instance));
                }//enemy stunned
                if (!instance.isStunned)
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
            instance.moving = false;
            instance.animator.Play(instance.animStun.name);
            instance.lastPos = instance.target.transform;            
            instance.StartCoroutine(instance.Timer(5));
        }
        public override void OnUpdate()
        {            
            if (instance.playerFound)
            {
                instance.StateMachine.SetState(new PursueState(instance));
            }
            else if (!instance.isStunned )
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



