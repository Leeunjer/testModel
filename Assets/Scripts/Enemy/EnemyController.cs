using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : MonoBehaviour
{

    //Ai 관련
    [SerializeField] private float patrolDetectionDistandce = 10f; // 정찰 범위
    [SerializeField] private float patrolWaitTime = 1f; // 정찰 대기 시간
    [SerializeField] private float patrolChance = 30f; // 정찰 확률
    [SerializeField] private LayerMask detectionTargetLayerMask; // 추격 대상의 Layer Mask
    [SerializeField] private float detectionSightAnlge = 30f;
    [SerializeField] private float minimumRunDistans = 1f;
    public float PatrolDetectionDistandce => patrolDetectionDistandce;
    public float PatrolWaitTime => patrolWaitTime;
    public float PatrolChance => patrolChance;
    public float DetectionSightAnlge =>detectionSightAnlge;
    public float MinimumRunDistance => minimumRunDistans;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    //상태
    public enum EEnemyState
    {
        None, Idle , Patrol, Chase, Attack , Hit , Dead
    }
    public EEnemyState State { get; private set; }
    private Dictionary<EEnemyState, ICharacterState> _states;

    //애니메이터 파라미터
    public static readonly int EnemyAniParamIdle = Animator.StringToHash("idle");
    public static readonly int EnemyAniParamPatrol = Animator.StringToHash("patrol");
    public static readonly int EnemyAniParamChase = Animator.StringToHash("chase");
    public static readonly int EnemyAniParamAttack = Animator.StringToHash("attack");
    public static readonly int EnemyAniParamHit = Animator.StringToHash("hit");
    public static readonly int EnemyAniParamDead = Animator.StringToHash("dead");
    public static readonly int EnemyAniParamMoveSpeed = Animator.StringToHash("move_speed");


    // 추격 대상 찾기
    private Transform _targetTransform;
    private Collider[] _detectionResults = new Collider[1];

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        var idleEnemyState = new IdleEnemyState(this,_animator , _navMeshAgent);
        var patrolEnemyState = new PatrolEnemyState(this,_animator ,_navMeshAgent);
        var chaseEnemyState = new ChaseEnemyState(this, _animator, _navMeshAgent);

        _states = new Dictionary<EEnemyState, ICharacterState>
        {
            {EEnemyState.Idle, idleEnemyState},
            {EEnemyState.Patrol, patrolEnemyState},
            {EEnemyState.Chase, chaseEnemyState},
        };
        SetState(EEnemyState.Idle);

        //추격 정보 초기화
        _targetTransform = null;
    }

    public void SetState(EEnemyState state)
    {
        if (State == state) return;
        if (State != EEnemyState.None) _states[State].Exit();
        State = state;
        if (State != EEnemyState.None) _states[State].Enter();
        

    }
    private void Update()
    {
        Debug.Log("Chomper update");
        if (State != EEnemyState.Dead && State != EEnemyState.None)
        {
            _states[State].Update();
        }
    }

    private void OnAnimatorMove()
    {
        var position = _animator.rootPosition;
        _navMeshAgent.nextPosition = position;
        transform.position = position;
    }

    //일정 거리 안에 Player가 있는지 확인 후 Player의 TransForm 정보를 반환, 
    //없으면 null 반환

    public Transform DetectionTargetInCircle()
    {
        if (!_targetTransform)
        { 
            //_targetTransform이 없으면 , 새롭게 찾기
            Physics.OverlapSphereNonAlloc(transform.position, PatrolDetectionDistandce,
                _detectionResults, detectionTargetLayerMask); //?

            //_detectionResults =
            //    Physics.OverlapSphere(transform.position, patrolDetectionDistandce, detectionTargetLayerMask);

            //detectionResults 배열의 0번 인덱스에 값이 있다면 그걸 _targetTransform에 할당해라
            _targetTransform = _detectionResults[0]?.transform;
        }
        else
        {
            //targetTrasnform이 있으면, 그 대상과의 거리를 계산해서 정해지 거리를 벗어나면 _targetTarnsform 정보 초기화
            float playerDistance = Vector3.Distance(transform.position,_targetTransform.position);
            if(playerDistance> patrolDetectionDistandce)
            {
                _targetTransform = null;
                _detectionResults[0] = null;
            }
        }
        return _targetTransform;

    }

    private void OnDrawGizmos()
    {
        //감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,patrolDetectionDistandce);

        //시야각
        Gizmos.color = Color.red;
        Vector3 rightDirection = Quaternion.Euler(0,detectionSightAnlge,0)*transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -detectionSightAnlge, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDirection * patrolDetectionDistandce);
        Gizmos.DrawRay(transform.position, leftDirection * patrolDetectionDistandce);
        Gizmos.DrawRay(transform.position, transform.forward * patrolDetectionDistandce);




        //Agent 목적지
        if (_navMeshAgent && _navMeshAgent.hasPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_navMeshAgent.destination,0.5f);
            Gizmos.DrawLine(transform.position,_navMeshAgent.destination);
        }
    }
}
