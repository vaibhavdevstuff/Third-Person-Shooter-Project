using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Roam,
        Chase,
        Dead

    }

    public State myState;

    public float screamTime;

    [Header("Movement")]
    public float roamSpeed = 0.3f;
    public float chaseSpeed = 2.5f;
    public float onAttackSpeed = 1.5f;

    [Header("Detection Data")]
    public float playerDetectionRadius = 8f;
    public float minRoamPointDistance = 5f;
    public float maxRoamPointDistance = 10f;
    public float roamPointReachedDistance = 1f;

    //Private Data
    private Transform playerPos;
    private Vector3 randomPos;
    private NavMeshHit hit;

    //Bool Data
    private bool canRoam;
    private bool CRRoam_IsRunning;
    private bool canChase;
    //private bool isDead;

    //Components
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    //Animation IDs
    private int _animIDIdle;
    private int _animIDRoam;
    private int _animIDDead;
    private int _animIDChase;
    private int _animIDAttack;    
    private int _animIDIdleState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        AssignAnimationIDs();
        SelectInitialState();
    }

    private void Start()
    {

        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if (playerPos == null)
            Debug.LogWarning(">>----------Player Not Detected----------<<");

        _navMeshAgent.isStopped = true;



    }

    private void Update()
    {
        if(canRoam)
        {
            if(Vector3.Distance(transform.position, randomPos) <= roamPointReachedDistance)
            {
                if (!CRRoam_IsRunning) Roaming();
            }
        }

        if (!canChase) CheckForPlayer();
        else Chasing();
    }

    private void AssignAnimationIDs()
    {
        _animIDIdle = Animator.StringToHash("Idle");
        _animIDRoam = Animator.StringToHash("Roam");
        _animIDDead = Animator.StringToHash("Dead");
        _animIDChase = Animator.StringToHash("Chase");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDIdleState = Animator.StringToHash("IdleState");

    }

    private void UpdateState()
    {
        switch(myState)
        {
            case State.Idle:
                IdleState();
                break;
            
            case State.Roam:
                RoamState();
                break;

            case State.Chase:
                StartCoroutine(ChaseState());
                break;

            case State.Dead:
                DeadState();
                break;

        }
    }

    void SelectInitialState()
    {
        int randomInitialState = UnityEngine.Random.Range(0, 2);

        myState = State.Idle;

        //if (randomInitialState == 0)
        //    myState = State.Idle;
        //else
        //    myState = State.Roam;

        UpdateState();
    }

    #region State Functions

    private void IdleState()
    {
        int idleStateIndex = Random.Range(1, 4);
        _animator.SetTrigger(_animIDIdle);
        _animator.SetInteger(_animIDIdleState, idleStateIndex);
    }

    private void RoamState()
    {
        _animator.SetTrigger(_animIDRoam);
        canRoam = true;
    }

    IEnumerator ChaseState()
    {
        canChase = true;
        _animator.SetTrigger(_animIDChase);        

        yield return new WaitForSeconds(screamTime);

        _navMeshAgent.speed = chaseSpeed;
        _navMeshAgent.isStopped = false;
    }

    private void DeadState()
    {

    }

    #endregion

    void Attack(bool canAttack)
    {
        _animator.SetBool(_animIDAttack, canAttack);
    }

    void Chasing()
    {
        _navMeshAgent.SetDestination(playerPos.position);
    }

    void Roaming()
    {
        CRRoam_IsRunning = true;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.speed = roamSpeed;

        var m_center = transform.position;

        Vector3 randomPosition = GetRoamingPoint();

        randomPosition.y = 0;
        randomPos = m_center + randomPosition;

        //Detecting navMesh Surface
        while (!NavMesh.SamplePosition(randomPos, out hit, 4f, NavMesh.AllAreas))
        {
            randomPosition = GetRoamingPoint();
            randomPosition.y = 0;
            randomPos = m_center + randomPosition;
        }

        randomPos = hit.position;

        _navMeshAgent.SetDestination(randomPos);

        CRRoam_IsRunning = false;
    }

    private Vector3 GetRoamingPoint()
    {
        Vector3 randomPosition = UnityEngine.Random.insideUnitSphere * maxRoamPointDistance;

        while(Vector3.Distance(transform.position, randomPosition) <= minRoamPointDistance)
        {
            randomPosition = UnityEngine.Random.insideUnitSphere * maxRoamPointDistance;
        }

        return randomPosition;

    }

    private void CheckForPlayer()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, playerPos.position);

        if (distanceFromPlayer <= playerDetectionRadius)
        {
            myState = State.Chase;
            UpdateState();
        }

    }

    public void HitPlayer()
    {
        Debug.Log("<color=cyan>>>----------Player Got Hit----------<<</color>");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Attack(true);
            _navMeshAgent.speed = onAttackSpeed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Attack(false);
            _navMeshAgent.speed = chaseSpeed;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        var spherePos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + 0.55f);
        Gizmos.DrawSphere(spherePos, 0.5f);

        if (canChase)
        {
            Gizmos.DrawCube(playerPos.position, new Vector3(1f, 1f, 1f));
            Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), playerPos.position);
        }

        if (canRoam)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawSphere(randomPos, 0.5f);
            Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), randomPos);
        }

        
    }






























}//class
